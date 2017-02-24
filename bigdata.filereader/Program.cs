using bigdata.filereader.ElasticSearchRepositories;
using bigdata.filereader.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using bigdata.filereader.AzureStorageRepositories;
using neiss.lookup.model;
using bigdata.filereader.Services;

namespace bigdata.filereader
{
    class Program
    {
        private static string jsonPath;

        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            // int artifactCount = ExeculateETLOfPublicData();

            NeissService neissservice = new NeissService(new NeissCodeLookupRepository(), new NeissReportRepository());
            neissservice.TranferDataFromCsvFileToElasticSearch(@"G:\USERS\EXIS\ASalomon\BigData\neiss-raw-tsv\test");
            sw.Stop();
            Console.WriteLine("loaded NEISS Reports ES in {0}", sw.Elapsed.Minutes);
            // Console.WriteLine("loaded {0} in {1}", artifactCount, sw.Elapsed.Minutes);
            Console.ReadKey();
        }

        private static void LoadNeissLookUpValuesToKeyValueStorage()
        {
            INeissCodeLookupRepository neiss = new NeissCodeLookupRepository();
            var lookupsFromNeissSASFile = new Utilities.NeissSasFormatsReader("D:\\NEISS_SAS_formats.txt").ReadAll();
            foreach (var lookup in lookupsFromNeissSASFile)
            {
                Console.WriteLine($"adding {lookup.Description}-{lookup.Code} to azure table ");
                neiss.Add(lookup);
                Console.WriteLine("complete");
            }

            var neisquery = neiss.Get(550, "Product");
            Console.WriteLine($"{neisquery.Code}-{neisquery.Description}");

        }

        private static int ExeculateETLOfIncidentDataToElasticSearch()
        {
            IIncidentRepository ir = new IncidentRepository();
            var artifactCount = 0;
          for (int i = 298; i < 615; i++)
            {
                Task.Delay(500);
                string recallsUrl = $"http://www.saferproducts.gov/restwebservices/recall?RecallID={i}&format=json";
                string incidentDataUrl = $"https://www.saferproducts.gov/incidentdata/api/incidentreports?page={i}";

                jsonPath = incidentDataUrl;

                var artifacts = new IncidentReport().GetDataFromPublicApi(jsonPath);
                artifactCount += artifacts.Count;
                artifacts.ForEach(r =>
                        AddArtifact(r, ir)
            );
                artifacts.Clear();
                Console.WriteLine($"page {i} is last page loaded");
            }

            return artifactCount;
        }

        private static void AddArtifact(IncidentReport r, IIncidentRepository ir)
        {
            ir.Add(r);
            Console.WriteLine("Added artifact of type {0} and id of {1} to elasticcloud", r.Type, r.IncidentReportId);
        }

    }

    
}
