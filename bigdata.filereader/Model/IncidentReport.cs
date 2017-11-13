using Newtonsoft.Json;
using OpenData.Shaper.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace OpenData.Shaper.Model
{
    public class IncidentReport : IArtifact
    {
        public IncidentReport()
        {
            this.Type = "incidentreport";
        }
        public string Type { get; set; }
        public int IncidentReportId { get; set; }
        public string IncidentReportNumber { get; set; }
        public DateTime IncidentReportDate { get; set; }
        public DateTime IncidentReportPublicationDate { get; set; }
        public DateTime ManufacturerNotificationDate { get; set; }
        public DateTime IncidentDate { get; set; }
        public string IncidentDescription { get; set; }
        public string CompanyCommentsExpended { get; set; }
        public string IncidentProductDescription { get; set; }
        public int ProductManufacturerComapanyId { get; set; }
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
        public bool ProductDamagedBeforeIncident { get; set; }
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

        public string UUID
        {
            get
            {
                return string.Concat(ArtifactSource,"-",this.Type,"-",this.IncidentReportNumber);
            }
        }

        public string Title
        {
            get
            {
                return $"Incident report involving {IncidentProductCategory}-{IncidentProductDescription}" ;
            }
            set { }
        }

        public string Description
        {
            get
            {
                return IncidentDescription;
            }
        }

        public string FullTextSearch
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ArtifactDate
        {
            get
            {
                return this.IncidentReportDate;//this date is chosen because incident date is not mandatory for a user to input
            }
        }

        public string ArtifactSource
        {
            get
            {
                throw new NotImplementedException();
            }
        }

       

        public class RelationShipType
        {
            public int RelationshipTyeId { get; set; }
            public string RelationshipTypePublicName { get; set; }
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
            public string SourceTypePublicName { get; set; }
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
            using (Newtonsoft.Json.JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                dataTypes = serializer.Deserialize<List<IncidentReport>>(reader);

            }

            return dataTypes;
        }
    }
}
