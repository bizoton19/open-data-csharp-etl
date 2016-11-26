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

namespace bigdata.filereader
{
    class Program
    {
        private static string jsonPath;

        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            IIncidentRepository ir = new IncidentRepository();
            IRecallRepository rr = new RecallRepository();
            var artifactCount = 0;
            for (int i =0 ; i < 20000; i++)
            {
                if (artifactCount == 1000)
                {
                    Task.Delay(500);
                }
                string recallsUrl = $"http://www.saferproducts.gov/restwebservices/recall?RecallID={i}&format=json";
                string incidentDataUrl = $"https://www.saferproducts.gov/incidentdata/api/incidentreports?page={i}";

                jsonPath = recallsUrl;// (incidentDataUrl;

                var artifacts = new Recall().GetDataFromPublicApi(jsonPath);
                artifactCount += artifacts.Count;
                artifacts.ForEach(r =>
                        AddArtifact(r, rr)
            );
                artifacts.Clear();
                Console.WriteLine($"page {i} is last page loaded");
            }
            sw.Stop();
            Console.WriteLine("loaded {0} in {1}", artifactCount , sw.Elapsed.Minutes);
            Console.ReadKey();
        }

        private static void AddArtifact(Recall r, IRecallRepository ir)
        {
            ir.Add(r);
            Console.WriteLine("Added artifact of type {0} and id of {1} to elasticcloud", r.Type, r.RecallID);
        }

    }

    
}
