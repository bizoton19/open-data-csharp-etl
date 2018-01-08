using OpenData.Shaper.Contracts;
using CPSC.OpenData.Shaper.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CPSC.OpenData.Shaper.Contracts;

namespace CPSC.OpenData.Shaper.Services
{
    public class NeissService
    {
        INeissCodeLookupRepository _neisslookuprepo;
        INeissReportRepository _neissrepo;
        System.Collections.Concurrent.ConcurrentBag<NeissReport> _bag;

        public NeissService(INeissCodeLookupRepository neisslookuprepo, INeissReportRepository neissrepo)
        {
            _neissrepo = neissrepo;
            _neisslookuprepo = neisslookuprepo;
            _bag = new ConcurrentBag<NeissReport>();
        }

        public ILookupBase GetNeissLookupValues(string partitionkey, int rowkey)
        {
            ILookupBase keyvalue = _neisslookuprepo.Get(rowkey, partitionkey);
            return keyvalue;

        }
        public void TransferData(dynamic from, dynamic to)
        {

        }
        public void TranferDataFromCsvFileToElasticSearch(string sourcefolderPath, string elasticConnectionString = null)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Console.WriteLine("Reading, Mapping and loading for Neiss report starting...");
            Console.WriteLine($"Current start Time {System.DateTime.Now}");
            //GenerateMassivedataFromTemplate(sourcefolderPath);
           
            LoadCsvData(sourcefolderPath);

            sw.Stop();
            Console.WriteLine($"Load to elasticsearch ended in {sw.Elapsed} minutes");
            Console.WriteLine($"Current End Time {System.DateTime.Now}");

        }
     
        private int? GetFieldCodeValue(string[] fields, int fieldposition)
        {
            return string.IsNullOrEmpty(fields[fieldposition]) ? default(int) : Int32.Parse(fields[fieldposition]);
        }
        
        
        private IEnumerable<string> ReadLines(string filename)
        { 
       
            using (TextReader reader = File.OpenText(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private IEnumerable<string> ReadLinesParallel(string filename)
        {

            using (StreamReader reader = File.OpenText(filename))
            {
                
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine(); ;
                }
            }
        }
      
        private void BatchLoad(string file)
        {
            IEnumerable<IEnumerable<NeissReport>> contentfix = ReadRecords(file);
            Console.WriteLine($"preparing to process groups of {contentfix.ToList().Count} neiss reports");
            foreach (var t in contentfix)
            {
                Console.WriteLine($"preparing to add {t.ToList().Count} indexes");
                var fileinfo = new FileInfo(file);
                var name = fileinfo.Name.Substring(0, fileinfo.Name.IndexOf('.')); //naming convention 

                // Task.Delay(1500);
                Console.WriteLine($"passing {t.ToList().Count} to neiss data store");
                _neissrepo.Add(t, name);
            }
        }
        public void LoadCsvData(string neissfilelocation = "D:\\neissinjurydata")
        {
            var dirFiles = Directory.GetFiles(neissfilelocation, "*.tsv");
            Console.WriteLine($"found {dirFiles.Length} files");
            Parallel.ForEach(dirFiles, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1
            },
            (oneFile) =>

             BatchLoadParallel(oneFile)

            );
            //Console.WriteLine($"Starting elastic bulk load at {DateTime.Now}");
            //_neissrepo.Add(_bag, "neiss");
           // Console.WriteLine($"done with elastic bulk load at {DateTime.Now}");
        }

        private IEnumerable<IEnumerable<NeissReport>> ReadRecords(string file)
        {
            return (
                      from line in ReadLinesParallel(file)
                                             .Skip(1)
                                             .Where(l => l
                                             .Split('\t')
                                             .Length >= 19)
                                             let neissreport = new NeissReport(line, _neisslookuprepo)
                                             select neissreport).PartitionCollection(5000);
        }
        private void BatchLoadParallel(string file)
        {
            var lines = ReadLinesParallel(file).Skip(1)
                                    .Where(l => l.Split('\t')
                                    .Length >= 19);
                        
            Console.WriteLine($"lines is now equal to {lines.ToList().Count}");
            foreach(var line in lines)
            {
                ProcessSingleLineDelegate(line);
            }
          

        }
        private string ResolveLookupValue(IList<Repositories.Csv.NeissEntity> lookupRec,string partitionKey,string lookupCode)
        {
            return lookupRec
                            .Where(item => item.PartitionKey.ToLowerInvariant() == partitionKey.ToLowerInvariant()
                                                               && item.RowKey == lookupCode)
                                                        .Select(x => x.Description).FirstOrDefault();

        }
       
        private void ProcessSingleLineDelegate(string line)
        {

            var report = new NeissReport(line, _neisslookuprepo);
            Repositories.Csv.NeissCodeLookupRepository lookupRepo = new Repositories.Csv.NeissCodeLookupRepository();
            var lookupRec = lookupRepo.ReadRecords();
             

            report.InjuryDiagnosis.Description = ResolveLookupValue(lookupRec,report.InjuryDiagnosis.GetType().Name,report.InjuryDiagnosis.Code.ToString());
            report.NeissBodyPart.Description = ResolveLookupValue(lookupRec, report.NeissBodyPart.GetType().Name, report.NeissBodyPart.Code.ToString());
            for (var i = 0; i < report.Products.Count; i++)
            {
                report.Products[i].Description = ResolveLookupValue(lookupRec, report.Products[i].GetType().Name, report.Products[i].Code.ToString());
            }
            
            report.NeissEventLocale.Description = ResolveLookupValue(lookupRec,report.NeissEventLocale.GetType().Name, report.NeissEventLocale.Code.ToString());
            report.NeissGender.Description = ResolveLookupValue(lookupRec, report.NeissGender.GetType().Name, report.NeissGender.Code.ToString());
            report.NeissInjuryDisposition.Description = ResolveLookupValue(lookupRec, report.NeissInjuryDisposition.GetType().Name, report.NeissInjuryDisposition.Code.ToString());
            report.NeissRace.Description = ResolveLookupValue(lookupRec, report.NeissRace.GetType().Name, report.NeissRace.Code.ToString());


            Console.WriteLine($"preparing to add {report.CpscCaseNumber} indexes");
            _bag.Add(report);
            
            Console.WriteLine($"Concurrent bag now has {_bag.Count} neiss reports");
            if (_bag.Count == 25000)
            {
                _neissrepo.Add(_bag, "neiss");
                _bag = new ConcurrentBag<NeissReport>();
            }

        }
        











    }
    
    }
