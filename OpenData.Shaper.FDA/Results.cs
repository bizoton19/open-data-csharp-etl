using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Model.FDA
{
    public class Meta
    {
        public DateTime last_updated { get; set; }
        public string terms { get; set; }
        public Results results { get; set; }
        public string license { get; set; }
        public string disclaimer { get; set; }
        public List<Recall> Results { get; set; }






    }
    public class Results
    {
        public int skip { get; set; }
        public int total { get; set; }
        public int limit { get; set; }
    }
}
