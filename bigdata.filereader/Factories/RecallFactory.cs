using OpenData.Shaper.Enums;
using OpenData.Shaper.Model;

namespace OpenData.Shaper.Factories
{
   
    public class RecallFactory
    {
      public  static RecallBase CreateRecall(RECALL_SOURCE recallSource)
        {
            if (recallSource == RECALL_SOURCE.FDA ) return new FDARecall();
            if (recallSource == RECALL_SOURCE.CPSC) return new Recall();
            return new Recall();
        }
    }
}