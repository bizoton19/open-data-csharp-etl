using OpenData.Shaper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Contracts
{
   public interface INeissReportRepository
    {
        NeissReport Get(int CpscCaseNumber);
        void Add(NeissReport report);
        void Add(IEnumerable<NeissReport> reports,string suffix);
        void Remove(NeissReport report);
    }
}
