using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader
{
    public interface IDataCategoryType
    {
     string Type { get; set; }
    }

    public interface IData:IDataCategoryType
    {
        String UUID { get; set; }
        String Title { get; set; }
        string Description { get; set; }
        string FullTextSearch { get;set;}
    }
}
