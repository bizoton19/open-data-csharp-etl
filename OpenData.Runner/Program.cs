using System;
using OpenData.Shaper.Utilities;
using CPSC.OpenData.Shaper.Services;
using CPSC.OpenData.Shaper.Contracts;
using CPSC.OpenData.Shaper.Repositories.Csv;
using CPSC.OpenData.Shaper.Repositories.ElasticSearch;
using CPSC.OpenData.Shaper.Model;
using CPSC.OpenData.Shaper.Repositories;
using System.Globalization;

namespace OpenData.Runner
{
    class Program
    {
        

        static void Main(string[] args)
        {
            //TODO: seprate code into two services
            //int artifactCount = new RecallService().ExtractMapLoadTo(new CPSC.OpenData.Shaper.Repositories.ElasticSearch.RecallRepository());
            //NeissService neissservice = new NeissService(new NeissCodeLookupRepository(), new NeissReportRepository());
            //neissservice.TranferDataFromCsvFileToElasticSearch(@"E:\sparkData\input");

            //var cpscRecall = new Recall();
            //int artifactCount = new RecallService().ExtractMapLoadTo(new CPSC.OpenData.Shaper.Repositories.ElasticSearch.RecallRepository());
            int art = new IncidentService().ExtractMapLoadTo(new CPSC.OpenData.Shaper.Repositories.ElasticSearch.IncidentRepository());
            Console.ReadKey();
        }

        private static void LoadNeissLookUpValuesToKeyValueStorage()
        {
            INeissCodeLookupRepository neiss = new NeissCodeLookupRepository();
            var lookupsFromNeissSASFile = new NeissSasFormatsReader("D:\\NEISS_SAS_formats.txt").ReadAll();
            foreach (var lookup in lookupsFromNeissSASFile)
            {
                Console.WriteLine($"adding {lookup.Description}-{lookup.Code} to azure table ");
                neiss.Add(lookup);
                Console.WriteLine("complete");
            }

            var neisquery = neiss.Get(550, "Product");
            Console.WriteLine($"{neisquery.Code}-{neisquery.Description}");

        }

      

    }

    
}
