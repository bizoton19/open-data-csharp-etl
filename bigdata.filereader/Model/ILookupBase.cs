using System;

namespace neiss.lookup.model
{
    public interface ILookupBase
    {
        int Code { get; set; }
        string Description { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate  { get; set; }
        bool IsValid();

    }
}