using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
    
    public class Recall : IDataCategoryType
    {
        public Recall()
        {
            this.Type = "recall";
        }
        public int RecallID { get; set; }
        public string RecallNumber { get; set; }
        public DateTime RecallDate { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string ConsumerContact { get; set; }
        public DateTime LastPublishDate { get; set; }
        public List<Product> Products { get; set; }
        public Inconjunction<string> Inconjuctions { get; set; }
        public List<ManufacturerCountry> ManufacturerCountries { get; set; }
        public List<Manufacturer> Manufacturers { get; set; }
        public List<Image> Images { get; set; }
        public List<Injury> Injuries { get; set; }
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
            public int CompanyID { get; set; }
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
            public int CompanyID { get; set; }
        }

        public List<Recall> GetDataFromPublicApi(string uri)
        {
            List<Recall> dataTypes = new List<Recall>();
            using (HttpClient client = new HttpClient())
            using (System.IO.StreamReader sr = new StreamReader(client.GetStreamAsync(new Uri(uri)).Result))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                dataTypes = serializer.Deserialize<List<Recall>>(reader);

            }

            return dataTypes;
        }
    }
}
