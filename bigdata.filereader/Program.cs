using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader
{
    class Program
    {
        private static string jsonPath;

        static void Main(string[] args)
        {
            
            for (int i = 1; i < 4000; i++)
            {
                string recallsUrl = $"http://www.saferproducts.gov/restwebservices/recall?RecallID={i}&format=json";
                string incidentDataUrl = $"https://www.saferproducts.gov/incidentdata/api/incidentreports?page={i}";

                jsonPath = incidentDataUrl;

                var recalls = new IncidentReport().GetDataFromPublicApi(jsonPath);
                recalls.ForEach(r =>
                        Console.WriteLine(r.IncidentDescription)
                    
            );
            }
            
            Console.ReadKey();
        }

       
    }

    public class IncidentReport : IDataCategoryType
    {
        public IncidentReport()
        {
            this.Type = "IncidentReport";
        }
        public string Type { get; set; }
        public string IncidentReportNumber  { get; set; }
        public DateTime IncidentReportDate { get; set; }
        public DateTime IncidentReportPublicationDate { get; set; }
        public DateTime ManufacturerNotificationDate { get; set; }
        public DateTime IncidentDate { get; set; }
        public string IncidentDescription { get; set; }
        public string CompanyCommentsExpended { get; set; }
        public string IncidentProductDescription { get; set; }
        public int ProductManufacturerComapnyId { get; set; }
        public string ProductManufacturerName { get; set; }
        public DateTime ProductManufacturedDate { get; set; }
        public int ProductRetailCompanyId { get; set; }
        public string ProductRetailCompanyName { get; set; }
        public DateTime ProductPuchasedDate { get; set; }
        public string ProductBrandName { get; set; }
        public string ProductModelName { get; set; }
        public string ProductSerialNumber { get; set; }
        public string ProductUPCCode { get; set; }
        public bool UserStillHasProduct { get; set; }
        public bool ProductDamagedBeforeIncident{ get; set; }
        public bool ProductModifiedBeforeIncident { get; set; }
        public bool UserContactedManufacturer { get; set; }
        public string AnswerExplanation { get; set; }
        public bool UserPlansToContactManufacturer { get; set; }
        public int VictimAgeInMonths { get; set; }
        public RelationShipType Relationship { get; set; }
        public Gender VictimGender { get; set; }
        public Locale IncidentLocale { get; set; }
        public SeverityType IncidentSeverityType { get; set; }
        public SourceType IncidentSourceType { get; set; }
        public ProductCategory IncidentProductCategory { get; set; }
        public List<IncidentDocument> IncidentDocuments { get; set; }
        public class RelationShipType
        {
            public int RelationshipTyeId { get; set; }
            public  string  RelationshipTypePublicName { get; set; }
        }

        public class Gender
        {
            public int GenderId { get; set; }
            public string GenderPublicName { get; set; }
        }

        public class Locale
        {
            public int LocalId { get; set; }
            public string LocalPublicName { get; set; }
        }

        public class SeverityType
        {
            public int SeverityTypeId { get; set; }
            public string SeverityTypePublicName { get; set; }
            public string SeverityTypeDescription { get; set; }
        }

        public class SourceType
        {
            public int SourceTypeId { get; set; }
            public string SourceTypePublicName  { get; set; }
        }

        public class ProductCategory
        {
            public int ProductCategoryId { get; set; }
            public string ProductCategoryPublicName { get; set; }
        }

        public class IncidentDocument
        {
            public int IncidentReportId { get; set; }
            public int DocumentId { get; set; }
            public string DocumentLocation { get; set; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }

        }

        public List<IncidentReport> GetDataFromPublicApi(string uri)
        {
            List<IncidentReport> dataTypes = new List<IncidentReport>();
            using (HttpClient client = new HttpClient())
            using (StreamReader sr = new StreamReader(client.GetStreamAsync(new Uri(uri)).Result))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                dataTypes = serializer.Deserialize<List<IncidentReport>>(reader);

            }

            return dataTypes;
        }
    }
    public class Recall:IDataCategoryType
    {
        public Recall()
        {
            this.Type = "Recall";
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
            public string  NumberOfUnits { get; set; }

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

        public  List<Recall> GetDataFromPublicApi(string uri)
        {
            List<Recall> dataTypes = new List<Recall>();
            using (HttpClient client = new HttpClient())
            using (StreamReader sr = new StreamReader(client.GetStreamAsync(new Uri(uri)).Result))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                dataTypes = serializer.Deserialize<List<Recall>>(reader);

            }

            return dataTypes;
        }
    }
}
