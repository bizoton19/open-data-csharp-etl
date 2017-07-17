using bigdata.filereader.Model;
using neiss.lookup.model;
using System;
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


        public NeissService(INeissCodeLookupRepository neisslookuprepo, INeissReportRepository neissrepo)
        {
            _neissrepo = neissrepo;
            _neisslookuprepo = neisslookuprepo;
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
        public void GenerateMassivedataFromTemplate(string neissfilelocation = "D:\\neissinjurydata")
        {
            var file = Directory.GetFiles(neissfilelocation, "*.tsv").FirstOrDefault();
            
                IEnumerable<IEnumerable<NeissReport>> records = ReadRecords(file);

                foreach (var rec in records)
                {
                    var fileinfo = new FileInfo(file);
                  
                    var name = fileinfo.Name.Substring(0, fileinfo.Name.IndexOf('.')); //naming convention 

                var age = rec.Select(n => n.Age);
                Console.WriteLine(age);
                   
                }
            

            //Parallel.ForEach(contentfix.PartitionCollection(660), new ParallelOptions()
            //{
            //    MaxDegreeOfParallelism = 1

            //},
            //(reportlist) =>


            //     _neissrepo.Add(reportlist)



            //);




        }

        public void LoadCsvData(string neissfilelocation = "D:\\neissinjurydata")
        {
            foreach (var file in Directory.GetFiles(neissfilelocation, "*.tsv"))
            {
                IEnumerable<IEnumerable<NeissReport>> contentfix = ReadRecords(file);

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

            //Parallel.ForEach(contentfix.PartitionCollection(660), new ParallelOptions()
            //{
            //    MaxDegreeOfParallelism = 1

            //},
            //(reportlist) =>


            //     _neissrepo.Add(reportlist)



            //);

        }

        private IEnumerable<IEnumerable<NeissReport>> ReadRecords(string file)
        {
            return (
                      from line in ReadLines(file)
                                             .Skip(1)
                                             .Where(l => l
                                             .Split('\t')
                                             .Length >= 19)
                                             let neissreport = new NeissReport(line, _neisslookuprepo)
                                             select neissreport).PartitionCollection(5000);
        }











    }
    
    }
