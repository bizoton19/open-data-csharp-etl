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
using bigdata.filereader.Model.Recalls;
using bigdata.filereader.Services;

namespace bigdata.filereader
{
    class Program
    {
        

        static void Main(string[] args)
        {
            
             System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //int artifactCount = ExeculateETLOfRecallsToElasticSearch();
            NeissService neissservice = new NeissService(new NeissCodeLookupRepository(), new ElasticSearchRepositories.NeissReportRepository());
            neissservice.TranferDataFromCsvFileToElasticSearch(@"E:\sparkData\input");
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

        private static int ExeculateETLOfRecallsToElasticSearch()
        {
            IRecallRepository ir = new ElasticSearchRepositories.RecallRepository();
            var artifactCount = 0;
          for (int i = 1; i < 7000; i++)
            {
                
                string recallsUrl = $"https://www.saferproducts.gov/RestWebServices/Recall?RecallID={i}&format=json";
                // string incidentDataUrl = $"https://www.saferproducts.gov/incidentdata/api/incidentreports?page={i}";

                //jsonPath = incidentDataUrl;

                var artifacts = new Recall().GetDataFromPublicApi(recallsUrl);
                artifactCount += artifacts.Count;
                artifacts.ForEach(r =>
                        AddArtifact(r, ir)
            );
                artifacts.Clear();
                Console.WriteLine($"page {i} is last page loaded");
            }

            return artifactCount;
        }



        private static void AddArtifact(Recall r, IRecallRepository ir)
        {
            ir.Add(r);
            Console.WriteLine("Added artifact of type {0} and id of {1} to elasticSearch", r.Type,r.RecallID );
        }

    }

    
}
