using neiss.lookup.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
  public  interface INeissCodeLookupRepository
    {
        ILookupBase Get(int code, string entityType);
        void Add(ILookupBase code);
        void Remove(ILookupBase code);
        IEnumerable<ILookupBase> Get(List<ILookupBase> lookupcodes);
    }
}
