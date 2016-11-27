using neiss.lookup.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace neiss.lookup.model
{
    public abstract class ProductCodeEvent
    {
        public DateTime EventDate { get; set; }
        public Product Product { get; set; }


    }
    public class ProductCodeDeletedEvent : ProductCodeEvent
    {

    }

    public class ProductCodeDeactivatedEvent : ProductCodeEvent
    {
        

    }
    public class ProductCodeReinstatedEvent: ProductCodeEvent { }
    public class ProductCodeInitiationEvent: ProductCodeEvent
    {

    }

    public class ProductCodeExpandedEvent : ProductCodeEvent
    {
        List<Product> ExpendedInto { get; set; }
    }
    public abstract class LookupBase: ILookupBase
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsValid()
        {
            return this.EndDate.Date >= DateTime.UtcNow.Date;
        }


    }
    public class Race : LookupBase
    {

    }

    public class InjuryDiagnonis : LookupBase
    {

    }

    public class BodyPart : LookupBase { }
    public class InjuryDisposition : LookupBase { }
    public class EventLocale : LookupBase
    {

    }
    public class Fire : LookupBase { }

    public class Product : LookupBase {
        
    }
    public class Hospital
    {
        public int PSU { get; set; }
        public String Stratum { get; set; }
    }

    public class Gender : LookupBase
    {


    }
}
namespace bigdata.filereader.Model
{
    

    public class NeissReport
    {
        public int CpscCaseNumber { get; set; }
        public DateTime TreatmentDate { get; set; }
        public Hospital Hospital { get; set; }
        public Gender Gender { get; set; }
        public Race Race { get; set; }
        public int StatisticalWeight { get; set; }
        public string RaceOther { get; set; }
        public string DiagnosisOther { get; set; }
        public List<string> Narrative { get; set; }
        public List<Product> Products { get; set; }
        public BodyPart BodyPart { get; set; }
        public Fire Fire { get; set; }
        public EventLocale EventLocale { get; set; }
        public InjuryDiagnonis InjuryDiagnosis { get; set; }
        public InjuryDisposition InjuryDisposition { get; set; }

        public int Age { get; set; }
        
    }
}
