using Newtonsoft.Json;
using OpenData.Shaper.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Model
{
    public abstract class RecallBase: IArtifact
    {
        public string RecallDate { get; set; }
        public string LastPublishDate { get; set; }
        public string RecallNumber { get; set; }
        public int RecallID { get; set; }
        public string URL { get; set; }
        public string ConsumerContact { get; set; }
        public string Description { get; set; }
        public string Type  { get; set; }

        public string UUID
        {
            get
            {
                return string.Concat(ArtifactSource,"-",Type,"-",RecallNumber);
            }

            
        }

        public string Title
        {
            get; set;
        }

        public string FullTextSearch
        {
            get; set;
           
        }

        public DateTime ArtifactDate
        {
           
                get { return DateTime.Parse(this.RecallDate); }
                

            
        }

        public string ArtifactSource
        {
            get
            {
                return "CPSC" ;
            }
        }
    }

    public class Hazard
    {
        public string Name { get; set; }
        public string HazardTypeID { get; set; }
    }

    public class Product
    {
        public string Type { get; set; }
        public string CategoryID { get; set; }
    }

    public class RecallReduced: RecallBase,IArtifact
    {
        public RecallReduced()
        {
            this.Type = "recallReduced";
        }

        public List<Hazard> Hazards { get; set; }
        public List<Product> Products { get; set; }
        
    }

    public class RecallDelimited:RecallBase, IArtifact
    {
        public RecallDelimited()
        {
            this.Type = "recallDelimited";
        } 
        public List<string> ProductName { get; set; }
        public List<string> ProductDescription { get; set; }
        public List<string> ProductModel { get; set; }
        public List<string> ProductType { get; set; }
        public List<string> ProductCategoryID { get; set; }
        public List<string> NumberOfUnit { get; set; }
        public List<object> InconjunctionCountry { get; set; }
        public List<string> ImageURL { get; set; }
        public List<string> Injury { get; set; }
        public List<string> ManufacturerCountry { get; set; }
        public List<object> UPC { get; set; }
        public List<string> Hazard { get; set; }
        public List<string> HazardTypeID { get; set; }
        public List<string> Manufacturer { get; set; }
        public List<string> ManufacturerCompanyID { get; set; }
        public List<string> Remedy { get; set; }
        public List<object> Retailer { get; set; }
        public List<object> RetailerCompanyID { get; set; }
        
        public List<RecallDelimited> GetDataFromPublicApi(string uri)
        {
            List<RecallDelimited> dataTypes = new List<RecallDelimited>();
            using (HttpClient client = new HttpClient())
            using (System.IO.StreamReader sr = new StreamReader(client.GetStreamAsync(new Uri(uri)).Result))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                reader.SupportMultipleContent = true;
                dataTypes = serializer.Deserialize<List<RecallDelimited>>(reader);

            }

            return dataTypes;
        }
    }
    
    public class Recall : RecallBase,IArtifact
    {
        public Recall()
        {
            this.Type = "recall";
        }
        
        
        public List<Product> Products { get; set; }
       
        public Inconjunction<string> Inconjuctions { get; set; }
        
        public ICollection<ManufacturerCountry> ManufacturerCountries { get; set; }
   
        public ICollection<Manufacturer> Manufacturers { get; set; }
        
        
        public ICollection<Image> Images { get; set; }
        
        public ICollection<Injury> Injuries { get; set; }
        public ICollection<Retailer> Retailers { get; set; }

        
        public class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Model { get; set; }
            public string Type { get; set; }
            public string CategoryID { get; set; }
            public string NumberOfUnits { get; set; }

        }
        public class Inconjunction<T> : List<T> { }


        public class Image
        {
            public string URL { get; set; }
        }
        public class Injury
        {
            public string Name { get; set; }
        }
        public class Manufacturer
        {
            public string Name { get; set; }
            public int? CompanyID { get; set; }
        }

        public class ManufacturerCountry
        {
            public string Country { get; set; }
        }

        public class ProductUPC
        {
            public string UPC { get; set; }
        }

        public class Hazard
        {
            public string Name { get; set; }
            public string HazardTypeID { get; set; }
        }

        public class Remedy
        {
            public string Name { get; set; }
        }

        public class Retailer
        {
            public string Name { get; set; }
            public int? CompanyID { get; set; }
        }

        public List<Recall> GetDataFromPublicApi(string uri)
        {
            List<Recall> dataTypes = new List<Recall>();
            using (HttpClient client = new HttpClient())
            using (System.IO.StreamReader sr = new StreamReader(client.GetStreamAsync(new Uri(uri)).Result))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                reader.SupportMultipleContent = true;
                dataTypes = serializer.Deserialize<List<Recall>>(reader);

            }

            return dataTypes;
        }
    }
}
