using OpenData.Shaper.Enums;
using OpenData.Shaper.Factories;
using OpenData.Shaper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Services
{
   public class RecallService
    {
        private RecallBase _recall;
        public RecallService(RECALL_SOURCE source)
        {
            _recall = RecallFactory.CreateRecall(source);
            
        }

    }
}
