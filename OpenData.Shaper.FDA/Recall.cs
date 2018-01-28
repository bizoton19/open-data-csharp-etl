namespace OpenData.Shaper.Model.FDA
{
    using Newtonsoft.Json;
    using OpenData.Shaper.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;

    /// <summary>
    /// Defines the <see cref="Recall" />
    /// </summary>
    public class Recall:IArtifact
    {
        public Recall()
        {
            Type = this.GetType().Name.ToLowerInvariant();
        }
        private const string SOURCE = "FDA";
        private const string CATEGORY = "Foods, Medicines, Cosmetics";
        /// <summary>
        /// Gets or sets the classification
        /// </summary>
        public string classification { get; set; }

        /// <summary>
        /// Gets or sets the center_classification_date
        /// </summary>
        public string center_classification_date { get; set; }

        /// <summary>
        /// Gets or sets the report_date
        /// </summary>
        public string report_date { get; set; }

        /// <summary>
        /// Gets or sets the postal_code
        /// </summary>
        public string postal_code { get; set; }

        /// <summary>
        /// Gets or sets the recall_initiation_date
        /// </summary>
        public string recall_initiation_date { get; set; }

        /// <summary>
        /// Gets or sets the recall_number
        /// </summary>
        public string recall_number { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// Gets or sets the event_id
        /// </summary>
        public string event_id { get; set; }

        /// <summary>
        /// Gets or sets the distribution_pattern
        /// </summary>
        public string distribution_pattern { get; set; }

        /// <summary>
        /// Gets or sets the recalling_firm
        /// </summary>
        public string recalling_firm { get; set; }

        /// <summary>
        /// Gets or sets the voluntary_mandated
        /// </summary>
        public string voluntary_mandated { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// Gets or sets the reason_for_recall
        /// </summary>
        public string reason_for_recall { get; set; }

        /// <summary>
        /// Gets or sets the initial_firm_notification
        /// </summary>
        public string initial_firm_notification { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the product_type
        /// </summary>
        public string product_type { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// Gets or sets the product_description
        /// </summary>
        public string product_description { get; set; }

        /// <summary>
        /// Gets or sets the code_info
        /// </summary>
        public string code_info { get; set; }

        /// <summary>
        /// Gets or sets the address_1
        /// </summary>
        public string address_1 { get; set; }

        /// <summary>
        /// Gets or sets the address_2
        /// </summary>
        public string address_2 { get; set; }

        /// <summary>
        /// Gets or sets the product_quantity
        /// </summary>
        public string product_quantity { get; set; }

        /// <summary>
        /// Gets the UUID
        /// </summary>
        public string UUID
        {
            get
            {
                return string.Concat(
                     ArtifactSource,
                     "-", Type.Substring(0, 1).ToUpper(),
                     Type.Substring(1, Type.Length-1),
                     "-", recall_number
                     );
            }
        }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title
        {
            get
            {
                return reason_for_recall;
            }

            set
            {
                ;
            }
        }

        /// <summary>
        /// Gets the ArtifactDate
        /// </summary>
        public DateTime ArtifactDate
        {
            get
            {
                return  DateTime.ParseExact(recall_initiation_date, "yyyyMMdd",
                                           new CultureInfo("en-US"),
                                           DateTimeStyles.None); ;
            }
        }

        /// <summary>
        /// Gets the ArtifactSource
        /// </summary>
        public string ArtifactSource
        {
            get
            {
                return SOURCE.ToLower();
            }
        }

        /// <summary>
        /// Gets the Description
        /// </summary>
        public string Description
        {
            get
            {
                return this.reason_for_recall;
            }
        }

        /// <summary>
        /// Gets or sets the FullTextSearch
        /// </summary>
      
       public string Type { get; set; }
        

       public string FullTextSearch
        {
            get
            {
                return string.Concat(reason_for_recall,Title, product_description);
            }

            set
            {
                throw new NotImplementedException();
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
        /// The GetDataFromPublicApi
        /// </summary>
        /// <param name="uri">The <see cref="string"/></param>
        /// <returns>The <see cref="List{FDARecall}"/></returns>
        public List<Recall> GetDataFromPublicApi(string uri)
        {
            List<Recall> dataTypes = new List<Recall>();
            using (HttpClient client = new HttpClient())
            using (StreamReader sr = CreateReader(uri))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                reader.SupportMultipleContent = true;
                dataTypes = serializer.Deserialize<List<Recall>>(reader);

            }

            return dataTypes;
        }
        public StreamReader CreateReader(string uri)
        {

            return uri.StartsWith("http")
                ? new StreamReader(new HttpClient().GetStreamAsync(new Uri(uri)).Result)
                : new StreamReader(uri);
        }
        
    }
}
