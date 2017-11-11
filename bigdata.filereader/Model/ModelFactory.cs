using bigdata.filereader.Model.Recalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
  public static class ModelFactory
    {
     public static IArtifact  CreateModel(string type)
        {
            switch (type)
            {
                case "Recall":
                    return new Recall();
                case "RecallDelimited":
                    return new RecallDelimited();
                case "RecallReduced":
                    return new RecallReduced();
                case "NeissReport":
                    return new NeissReport();
                case "IncidentReport":
                    return new IncidentReport();
                
            }

            return null;
        }
    }
}
