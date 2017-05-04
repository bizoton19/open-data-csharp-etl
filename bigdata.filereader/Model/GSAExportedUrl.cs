using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
    public class GSAExportedUrl : IDataCategoryType

    {
        
        public GSAExportedUrl(string line=null)
        {
            this.Type = "GSAExportedURL";
            

        }
       
            public string Type { get; set; }
            public string Url { get; set; }
            public string State { get; set;}
            public string Description { get; set; }
    
    }
}
