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

            IIncidentRepository ir = new IncidentRepository();
            for (int i = 1; i < 615; i++)
            {
                string recallsUrl = $"http://www.saferproducts.gov/restwebservices/recall?RecallID={i}&format=json";
                string incidentDataUrl = $"https://www.saferproducts.gov/incidentdata/api/incidentreports?page={i}";

                jsonPath = incidentDataUrl;

                var recalls = new IncidentReport().GetDataFromPublicApi(jsonPath);
                
                recalls.ForEach(r =>
                        ir.Add(r)
                        
                        
                    
            );
            }
            
            Console.ReadKey();
        }

       
    }

    
}
