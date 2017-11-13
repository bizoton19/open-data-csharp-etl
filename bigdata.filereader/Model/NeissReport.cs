
using OpenData.Shaper.Contracts;
using OpenData.Shaper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OpenData.Shaper.Model
{
   
    public abstract class LookupBase: ILookupBase
    {
        
        public int? Code { get; set; }
        public string Description { get; set; }

        public string Type { get {
                return BuildType(this.GetType().Name);
            } set { } }

        protected string BuildType(string typeName)
        {
            foreach (var item in typeName)
            {
                if (typeName.IndexOf(item) > 0)
                {
                    if (char.IsUpper(item))
                    {
                        var firstNode = typeName.Substring(0, typeName.IndexOf(item));
                        typeName = string.Concat(firstNode, " ", typeName.Remove(0, firstNode.Length));
                    }

                }
            }
            return typeName;
        }

    }
   
}
namespace OpenData.Shaper.Model
{
    

    public class NeissReport: IArtifact
    {
        INeissCodeLookupRepository _repo;

        
        public NeissReport()
        {
            
        }
        public string Type { get { return  BuildType(this.GetType().Name); } set{ } }  
   
        private int? GetFieldCodeValue(string fieldValue)
        {
            return string.IsNullOrEmpty(fieldValue) ? 0 : Int32.Parse(fieldValue);
        }
       
        public NeissReport(string tsvrecord, INeissCodeLookupRepository repo=null)
        {
            _repo = repo;
            if (!string.IsNullOrEmpty(tsvrecord))
            {
                var fields = tsvrecord.Split('\t');
                CpscCaseNumber = Int32.Parse(fields[0]);
                TreatmentDate = DateTime.Parse(fields[1]);
                NeissHospital = new Hospital()
                {
                    PSU = string.IsNullOrEmpty(fields[2]) ? 0 : Int32.Parse(fields[2]),
                    Stratum = string.IsNullOrEmpty(fields[4]) ? string.Empty: fields[4]

                };
                StatisticalWeight = decimal.Parse(fields[3]);
                Age = Int32.Parse(fields[5]);
                NeissGender = new Gender()
                {
                    Code = GetFieldCodeValue(fields[6]).Value
                    

                };
               
                NeissRace = new Race()
                {
                    Code = GetFieldCodeValue(fields[7]).Value
                   


                };
                RaceOther = fields[8];
                InjuryDiagnosis = new InjuryDiagnonis()
                {
                    Code = GetFieldCodeValue(fields[9]).Value
                   

                };
                DiagnosisOther = fields[10];
                NeissBodyPart = new BodyPart()
                {
                    Code = GetFieldCodeValue(fields[11]).Value,
                  
                };
                NeissInjuryDisposition = new InjuryDisposition()
                {
                    Code = GetFieldCodeValue(fields[12]).Value,
                    
                };
                NeissEventLocale = new EventLocale()
                {
                    Code = GetFieldCodeValue(fields[13]).Value,
                    

                };
                Products = new List<Product>()
                    {
                        new Product()
                        {
                            Code = GetFieldCodeValue(fields[15]).Value,
                            

                        },
                        new Product()
                        {
                            Code=GetFieldCodeValue(fields[16]).Value,
                           

                        }
                    };
                Narrative = new List<string>()
                    {
                    
                    string.IsNullOrEmpty(fields[17]) ? string.Empty : fields[17],
                        string.IsNullOrEmpty(fields[18]) ? string.Empty : fields[18]
                       
                    };
            }
        }
        public class Race : OpenData.Shaper.Model.LookupBase
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

        public class Product : LookupBase
        {

        }
        public class Hospital:IDataCategoryType
        {
            public int PSU { get; set; }
            public String Stratum { get; set; }
            public string Type { get { return this.GetType().Name; } set{ } }

        }

        public class Gender : LookupBase
        {
            

        }
        public int CpscCaseNumber { get; set; }
        public DateTime TreatmentDate { get; set; }
        public Hospital NeissHospital { get; set; }
        public Gender NeissGender { get; set; }
        public Race NeissRace { get; set; }
        public decimal StatisticalWeight { get; set; }
        public string RaceOther { get; set; }
        public string DiagnosisOther { get; set; }
        public List<string> Narrative { get; set; }
        public List<Product> Products { get; set; }
        public BodyPart NeissBodyPart { get; set; }
        public Fire NeissFire { get; set; }
        public EventLocale NeissEventLocale { get; set; }
        public InjuryDiagnonis InjuryDiagnosis { get; set; }
        public InjuryDisposition NeissInjuryDisposition { get; set; }
        public int? Age { get; set; }

        public string UUID
        {
            get
            {
                return string.Concat(ArtifactSource, "-",Type,"-",CpscCaseNumber);
            }
        }

        public string Title
        {
            get
            {
                return 
                   BuildTitle();
            }
            set { }
        }

        public string Description
        {
            get
            {
                return 
                string.Join("-",Narrative);
            }
        }

        public string FullTextSearch { get; set; }
        

        public DateTime ArtifactDate
        {
            get
            {
              return this.TreatmentDate  ;
            }

           
        }

        public string ArtifactSource
        {
            get
            {
                return "CPSC";
            }
        }
       
        private string BuildTitle()
        {
            string title = $"Emergency room injury involving products: ";
            for (var i= 0; i< Products.Count;i++)
            {
               title = string.Concat(title,
                    Products[i] == null
                ? "N/A"
                : Products[i].Description == null
                ? Products[i].Code.ToString()
                : Products[i].Description.ToLowerInvariant());
            }

            return title;
        }
        private string BuildType(string typeName)
        {
            foreach (var item in typeName)
            {
                if (typeName.IndexOf(item) > 0)
                {
                    if (char.IsUpper(item))
                    {
                        var firstNode = typeName.Substring(0, typeName.IndexOf(item));
                        typeName = string.Concat(firstNode, " ", typeName.Remove(0, firstNode.Length));
                    }

                }
            }
            return typeName;
        }

    }
        
    

}
