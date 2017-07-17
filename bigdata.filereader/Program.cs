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
using System.Configuration;
using bigdata.filereader.SolrRepositories;
using SolrNet;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Impl;
using SolrNet.Utils;
using SolrNet.Commands;
using SolrNet.DSL;
using SolrNet.Mapping;

namespace bigdata.filereader
{
    class Program
    {
        private static string jsonPath;

        static void Main(string[] args)
        {
            Startup.Init<Recall>(ConfigurationManager.AppSettings["solrConnection"]);
            //var headerParser = ServiceLocator.Current.GetInstance<ISolrHeaderResponseParser>();
            //var statusParser = ServiceLocator.Current.GetInstance<ISolrStatusResponseParser>();
              
            //ISolrCoreAdmin solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(ConfigurationManager.AppSettings["solrConnection"]), headerParser, statusParser);
            
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            int artifactCount = ExeculateETLOfRecallsToElasticSearch();

            Startup.Init<Recall>(ConfigurationManager.AppSettings["solrConnection"]);
            //NeissService neissservice = new NeissService(new NeissCodeLookupRepository(), new ElasticSearchRepositories.NeissReportRepository());
            // neissservice.TranferDataFromCsvFileToElasticSearch(@"E:\sparkData\input");
            //neissservice.GenerateMassivedataFromTemplate(@"G:\USERS\EXIS\ASalomon\BigData\neiss-raw-tsv\test");
            //neissservice.GenerateMassiveNeissDataSet(ConfigurationManager.AppSettings["NeissData"]);
            sw.Stop();
            Console.WriteLine("loaded NEISS Reports ES in {0}", sw.Elapsed.Minutes);
            Console.WriteLine("loaded {0} in {1}", artifactCount, sw.Elapsed.Minutes);
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
            IRecallRepository ir = new RecallSolrRepository();
            var artifactCount = 0;
          for (int i = 1; i < 5000; i++)
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
            Console.WriteLine("Added artifact of type {0} and id of {1} to elasticcloud", r.Type,r.RecallID );
        }

    }

    
}
