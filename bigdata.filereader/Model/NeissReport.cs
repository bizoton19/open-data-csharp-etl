using neiss.lookup.model;
using System;
using System.Collections.Generic;
using System.IO;
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
        public decimal StatisticalWeight { get; set; }
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
        public void GetDataFromYearlyFiles(string neissfilelocation = "D:\\neissinjurydata")
        {
            string[] files = Directory.GetFiles(neissfilelocation, "*.tsv");
            foreach (var file in files)
            {
                var content = ReadAsLines(file);
                foreach(var record in content)
                {
                    string[] fields = record.Split('\t');
                    //map with neiss object
                    //get lookup label from lookupservice
                    CpscCaseNumber = Convert.ToInt32(fields[0]);
                    TreatmentDate = DateTime.Parse(fields[1]);
                    this.Hospital = new Hospital()
                    {
                        PSU = Convert.ToInt32(fields[2]),
                        Stratum = fields[3]

                    };
                    StatisticalWeight = decimal.Parse(fields[4]);
                    Age = Convert.ToInt32(fields[5]);
                    Gender = new Gender()
                    {
                        Code = Convert.ToInt32(fields[6]),
                        // Description = call service to get label

                    };
                    Race = new Race()
                    {
                        Code = Convert.ToInt32(fields[7]),

                    };
                    RaceOther = fields[8];
                    InjuryDiagnosis = new InjuryDiagnonis()
                    {
                        Code = Convert.ToInt32(fields[9])
                    };
                    DiagnosisOther = fields[10];
                    BodyPart = new BodyPart()
                    {
                        Code = Convert.ToInt32(fields[11])
                    };
                    InjuryDisposition = new InjuryDisposition()
                    {
                        Code = Convert.ToInt32(fields[12])
                    };
                    EventLocale = new EventLocale()
                    {
                        Code = Convert.ToInt32(fields[13])
                    };
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            Code = Convert.ToInt32(fields[15])
                        },
                        new Product()
                        {
                            Code=Convert.ToInt32(fields[16])
                        }
                    };
                    Narrative = new List<string>()
                    {
                        fields[17],
                        fields[18]
                    };
                    

                    


                }
            }
        }
        private IEnumerable<string> ReadAsLines(string file)
        {
            using (var reader = new StreamReader(file))
                while (!reader.EndOfStream)
                    yield return reader.ReadLine();
        }
           
        }
        
    

}
