
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
   
    public class IncidentService
    {


        public int ExtractMapLoadTo(IGenericRepository<IncidentReport> repo)
        {

            var artifactCount = 0;
            for (int i = 1; i < 600; i++)
            {

                
                string incidentDataUrl = $"http://rmsprodweb1/incidentdata/api/incidentreports?page={i}";

                var jsonPath = incidentDataUrl;

                var artifacts = new IncidentReport().GetDataFromPublicApi(incidentDataUrl);
                artifactCount += artifacts.Count;
                artifacts.ForEach(r =>
                        AddArtifact(r, repo)
            );
                artifacts.Clear();
                Console.WriteLine($"page {i} is last page loaded");
            }

            return artifactCount;
        }



        private static void AddArtifact(IncidentReport r, IGenericRepository<IncidentReport> repo)
        {
            repo.Add(r);
            Console.WriteLine("Added artifact of type {0} and id of {1} to elasticSearch", r.Type, r.IncidentReportNumber);
        }

    }
}
