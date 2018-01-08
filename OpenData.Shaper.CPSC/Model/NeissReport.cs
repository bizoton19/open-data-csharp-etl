
using OpenData.Shaper.Contracts;
namespace CPSC.OpenData.Shaper.Model
{
   

    /// <summary>
    /// Defines the <see cref="LookupBase" />
    /// </summary>
    public abstract class LookupBase : ILookupBase
    {
        /// <summary>
        /// Gets or sets the Code
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        public string Type
        {
            get
            {
                return BuildType(this.GetType().Name);
            }
            set { }
        }

        /// <summary>
        /// The BuildType
        /// </summary>
        /// <param name="typeName">The <see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
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

namespace CPSC.OpenData.Shaper.Model
{
    using OpenData.Shaper.Contracts;
    
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="NeissReport" />
    /// </summary>
    public class NeissReport : IArtifact
    {
        private const string SOURCE = "CPSC";
        private const string CATEGORY = "Consumer Products";
        /// <summary>
        /// Defines the _repo
        /// </summary>
        internal INeissCodeLookupRepository _repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeissReport"/> class.
        /// </summary>
        public NeissReport()
        {
        }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        public string Type
        {
            get { return BuildType(this.GetType().Name); }
            set { }
        }

        /// <summary>
        /// The GetFieldCodeValue
        /// </summary>
        /// <param name="fieldValue">The <see cref="string"/></param>
        /// <returns>The <see cref="int?"/></returns>
        private int? GetFieldCodeValue(string fieldValue)
        {
            return string.IsNullOrEmpty(fieldValue) ? 0 : Int32.Parse(fieldValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NeissReport"/> class.
        /// </summary>
        /// <param name="tsvrecord">The <see cref="string"/></param>
        /// <param name="repo">The <see cref="INeissCodeLookupRepository"/></param>
        public NeissReport(string tsvrecord, INeissCodeLookupRepository repo = null)
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
                    Stratum = string.IsNullOrEmpty(fields[4]) ? string.Empty : fields[4]

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

        /// <summary>
        /// Defines the <see cref="Race" />
        /// </summary>
        public class Race : OpenData.Shaper.Model.LookupBase
        {
        }

        /// <summary>
        /// Defines the <see cref="InjuryDiagnonis" />
        /// </summary>
        public class InjuryDiagnonis : LookupBase
        {
        }

        /// <summary>
        /// Defines the <see cref="BodyPart" />
        /// </summary>
        public class BodyPart : LookupBase
        {
        }

        /// <summary>
        /// Defines the <see cref="InjuryDisposition" />
        /// </summary>
        public class InjuryDisposition : LookupBase
        {
        }

        /// <summary>
        /// Defines the <see cref="EventLocale" />
        /// </summary>
        public class EventLocale : LookupBase
        {
        }

        /// <summary>
        /// Defines the <see cref="Fire" />
        /// </summary>
        public class Fire : LookupBase
        {
        }

        /// <summary>
        /// Defines the <see cref="Product" />
        /// </summary>
        public class Product : LookupBase
        {
        }

        /// <summary>
        /// Defines the <see cref="Hospital" />
        /// </summary>
        public class Hospital 
        {
            /// <summary>
            /// Gets or sets the PSU
            /// </summary>
            public int PSU { get; set; }

            /// <summary>
            /// Gets or sets the Stratum
            /// </summary>
            public String Stratum { get; set; }

            /// <summary>
            /// Gets or sets the Type
            /// </summary>
            public string Type
            {
                get { return this.GetType().Name; }
                set { }
            }
        }

        /// <summary>
        /// Defines the <see cref="Gender" />
        /// </summary>
        public class Gender : LookupBase
        {
        }

        /// <summary>
        /// Gets or sets the CpscCaseNumber
        /// </summary>
        public int CpscCaseNumber { get; set; }

        /// <summary>
        /// Gets or sets the TreatmentDate
        /// </summary>
        public DateTime TreatmentDate { get; set; }

        /// <summary>
        /// Gets or sets the NeissHospital
        /// </summary>
        public Hospital NeissHospital { get; set; }

        /// <summary>
        /// Gets or sets the NeissGender
        /// </summary>
        public Gender NeissGender { get; set; }

        /// <summary>
        /// Gets or sets the NeissRace
        /// </summary>
        public Race NeissRace { get; set; }

        /// <summary>
        /// Gets or sets the StatisticalWeight
        /// </summary>
        public decimal StatisticalWeight { get; set; }

        /// <summary>
        /// Gets or sets the RaceOther
        /// </summary>
        public string RaceOther { get; set; }

        /// <summary>
        /// Gets or sets the DiagnosisOther
        /// </summary>
        public string DiagnosisOther { get; set; }

        /// <summary>
        /// Gets or sets the Narrative
        /// </summary>
        public List<string> Narrative { get; set; }

        /// <summary>
        /// Gets or sets the Products
        /// </summary>
        public List<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the NeissBodyPart
        /// </summary>
        public BodyPart NeissBodyPart { get; set; }

        /// <summary>
        /// Gets or sets the NeissFire
        /// </summary>
        public Fire NeissFire { get; set; }

        /// <summary>
        /// Gets or sets the NeissEventLocale
        /// </summary>
        public EventLocale NeissEventLocale { get; set; }

        /// <summary>
        /// Gets or sets the InjuryDiagnosis
        /// </summary>
        public InjuryDiagnonis InjuryDiagnosis { get; set; }

        /// <summary>
        /// Gets or sets the NeissInjuryDisposition
        /// </summary>
        public InjuryDisposition NeissInjuryDisposition { get; set; }

        /// <summary>
        /// Gets or sets the Age
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Gets the UUID
        /// </summary>
        public string UUID
        {
            get
            {
                return string.Concat(ArtifactSource, "-", Type, "-", CpscCaseNumber);
            }
        }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title
        {
            get
            {
                return
                   BuildTitle();
            }
            set { }
        }

        /// <summary>
        /// Gets the Description
        /// </summary>
        public string Description
        {
            get
            {
                return
                string.Join("-", Narrative);
            }
        }

        /// <summary>
        /// Gets or sets the FullTextSearch
        /// </summary>
        public string FullTextSearch { get; set; }

        /// <summary>
        /// Gets the ArtifactDate
        /// </summary>
        public DateTime ArtifactDate
        {
            get
            {
                return this.TreatmentDate;
            }
        }

        /// <summary>
        /// Gets the ArtifactSource
        /// </summary>
        public string ArtifactSource
        {
            get
            {
                return SOURCE;
            }
        }

        public string Category
        {
            get
            {
                return CATEGORY;
            }
        }

        /// <summary>
        /// The BuildTitle
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        private string BuildTitle()
        {
            string title = $"Emergency room injury involving products: ";
            for (var i = 0; i < Products.Count; i++)
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

        /// <summary>
        /// The BuildType
        /// </summary>
        /// <param name="typeName">The <see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
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
