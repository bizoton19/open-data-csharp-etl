
using CPSC.OpenData.Shaper.Model;
using OpenData.Shaper.Default.Repositories;
using OpenData.Shaper.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC.OpenData.Shaper.Services
{
   public class RecallService
    {
        
      
        public int ExtractMapLoadTo(IGenericRepository<Recall> repo)
        {
            
            var artifactCount = 0;
            for (int i = 1; i < 7000; i++)
            {

                string recallsUrl = $"https://www.saferproducts.gov/RestWebServices/Recall?RecallID={i}&format=json";
                // string incidentDataUrl = $"https://www.saferproducts.gov/incidentdata/api/incidentreports?page={i}";

                //jsonPath = incidentDataUrl;

                var artifacts = new Recall().GetDataFromPublicApi(recallsUrl);
                artifactCount += artifacts.Count;
                artifacts.ForEach(r =>
                        AddArtifact(r, repo)
            );
                artifacts.Clear();
                Console.WriteLine($"page {i} is last page loaded");
            }

            return artifactCount;
        }



        private static void AddArtifact(Recall r, IGenericRepository<Recall> repo)
        {
            repo.Add(r);
            Console.WriteLine("Added artifact of type {0} and id of {1} to elasticSearch", r.Type, r.RecallID);
        }

    }
}
