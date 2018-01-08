using OpenData.Shaper.Contracts;
using System.Collections.Generic;

namespace CPSC.OpenData.Shaper.Contracts
{
    public  interface INeissCodeLookupRepository
    {
        ILookupBase Get(int? code, string entityType);
        void Add(ILookupBase code);
        void Remove(ILookupBase code);
        IEnumerable<ILookupBase> Get(List<ILookupBase> lookupcodes);
    }
}
