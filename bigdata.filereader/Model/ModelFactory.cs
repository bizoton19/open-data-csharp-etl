using OpenData.Shaper.Contracts;
using OpenData.Shaper.Model;

namespace OpenData.Shaper.Factories
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
