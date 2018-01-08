using OpenData.Shaper.Default.Repositories;
using CPSC.OpenData.Shaper.Model;
using System.Collections.Generic;

namespace CPSC.OpenData.Shaper.Contracts
{
    public interface INeissReportRepository:IGenericRepository<NeissReport>
    {
      
        void Add(IEnumerable<NeissReport> reports,string suffix);
        
    }
}
