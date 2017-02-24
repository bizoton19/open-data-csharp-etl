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
            GetDataFromYearlyFiles(sourcefolderPath);

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
        public void GetDataFromYearlyFiles(string neissfilelocation = "D:\\neissinjurydata")
        {
            foreach (var file in Directory.GetFiles(neissfilelocation, "*.tsv"))
            {

                var contentfix = (
                                 from line in ReadLines(file).Skip(1).Where(l=>l.Split('\t').Length>=19)
                                 let neissreport = new NeissReport(line,_neisslookuprepo)
                                 select neissreport).PartitionCollection(5000);

                foreach (var t in contentfix)
                {
                    var fileinfo = new FileInfo(file);
                    var name = fileinfo.Name.Substring(0, fileinfo.Name.IndexOf('.')); //naming convention 
                    
                    Task.Delay(1500);
                    _neissrepo.Add(t,name);
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

      

        
     
        






    }
    
    }
