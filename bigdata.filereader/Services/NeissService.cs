using bigdata.filereader.Model;
using neiss.lookup.model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static bigdata.filereader.Model.NeissReport;

namespace bigdata.filereader.Services
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
     

     
        public void TranferDataFromCsvFileToMongoDb(string folderPath, string elasticConnectionString)
        {

        }

        public void TranferDataFromCsvFileToCassandra(string folderPath, string elasticConnectionString)
        {

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
           // Parallel.ForEach(lines, new ParallelOptions()
           // {
           //     MaxDegreeOfParallelism = 1
           // },
          // (line) =>

              //ProcessSingleLineDelegate(line)
           // );

        }
        private void ProcessSingleLineDelegate(string line)
        {

            var report = new NeissReport(line, _neisslookuprepo);
            CsvRepositories.NeissCodeLookupRepository lookupRepo = new CsvRepositories.NeissCodeLookupRepository();
            var lookupRec = lookupRepo.ReadRecords();

          
          
            report.InjuryDiagnosis.Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.InjuryDiagnosis.GetType().Name.ToLowerInvariant() 
                                                               && item.RowKey == report.InjuryDiagnosis.Code.ToString())
                                                        .Select(x=>x.Description).FirstOrDefault();

            report.NeissBodyPart.Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.NeissBodyPart.GetType().Name.ToLowerInvariant()
                                                               && item.RowKey == report.NeissBodyPart.Code.ToString())
                                                        .Select(x => x.Description).FirstOrDefault();
            report.Products[0].Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.Products[0].GetType().Name.ToLowerInvariant()
                                                               && item.RowKey == report.Products[0].Code.ToString())
                                                        .Select(x => x.Description).FirstOrDefault();
            report.Products[1].Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.Products[1].GetType().Name.ToLowerInvariant()
                                                               && item.RowKey == report.Products[1].Code.ToString())
                                                        .Select(x => x.Description).FirstOrDefault();
            report.NeissEventLocale.Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.NeissEventLocale.GetType().Name.ToLowerInvariant()
                                                               && item.RowKey == report.NeissEventLocale.Code.ToString())
                                                        .Select(x => x.Description).FirstOrDefault();
           
            report.NeissGender.Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.NeissGender.GetType().Name.ToLowerInvariant()
                                                               && item.RowKey == report.NeissGender.Code.ToString())
                                                        .Select(x => x.Description).FirstOrDefault();
            report.NeissInjuryDisposition.Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.NeissInjuryDisposition.GetType().Name.ToLowerInvariant()
                                                               && item.RowKey == report.NeissInjuryDisposition.Code.ToString())
                                                        .Select(x => x.Description).FirstOrDefault();
            report.NeissRace.Description = lookupRec
                                                        .Where(item => item.PartitionKey.ToLowerInvariant() == report.NeissRace.GetType().Name.ToLowerInvariant()
                                                               && item.RowKey == report.NeissRace.Code.ToString())
                                                        .Select(x => x.Description).FirstOrDefault();


            Console.WriteLine($"preparing to add {report.CpscCaseNumber} indexes");
            _bag.Add(report);
            Console.WriteLine($"Concurrent bag now has {_bag.Count} neiss reports");
            if (_bag.Count == 4000)
            {
                _neissrepo.Add(_bag, "neiss");
                _bag = new ConcurrentBag<NeissReport>();
            }

        }
        











    }
    
    }
