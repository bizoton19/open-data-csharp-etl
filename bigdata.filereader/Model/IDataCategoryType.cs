using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Contracts
{
    public interface IDataCategoryType
    {
     string Type { get; set; }
    }

    public interface IArtifact:IDataCategoryType
    {
        String UUID { get;  }
        String Title { get; set; }
        DateTime ArtifactDate { get;  }
        string ArtifactSource { get; }
        string Description { get;  }
        string FullTextSearch { get;set;}
    }
}
