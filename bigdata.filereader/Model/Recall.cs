using Newtonsoft.Json;
using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
    public abstract class RecallBase
    {
        public string RecallDate { get; set; }
        public string LastPublishDate { get; set; }
        public string RecallTitle { get; set; }
        public string RecallNumber { get; set; }
        public int RecallID { get; set; }
        public string RecallURL { get; set; }
        public string ConsumerContact { get; set; }
        public string RecallDescription { get; set; }
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

    public class RecallReduced: RecallBase,IDataCategoryType
    {
        public RecallReduced()
        {
            this.Type = "recallReduced";
        }

        public List<Hazard> Hazards { get; set; }
        public List<Product> Products { get; set; }
        public string Type { get; set; }
    }

    public class RecallDelimited:RecallBase, IDataCategoryType
    {
        public RecallDelimited()
        {
            this.Type = "recallDelimited";
        }
        public string Type { get; set; }
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
    }
    public class Recall : RecallBase,IDataCategoryType
    {
        public Recall()
        {
            this.Type = "recall";
        }
       
        [SolrField("products")]
        public List<Product> Products { get; set; }
        [SolrField("inconjuctions")]
        public Inconjunction<string> Inconjuctions { get; set; }
        [SolrField("manufacturerCountries")]
        public ICollection<ManufacturerCountry> ManufacturerCountries { get; set; }
        [SolrField("manufacturers")]
        public ICollection<Manufacturer> Manufacturers { get; set; }
        [SolrField("images")]
        public ICollection<Image> Images { get; set; }
        [SolrField("injuries")]
        public ICollection<Injury> Injuries { get; set; }
        [SolrField("type")]
        public string Type { get; set; }
       
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
