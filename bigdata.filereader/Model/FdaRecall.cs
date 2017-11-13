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
   public class FDARecall:RecallBase,IArtifact
        {
            public string classification { get; set; }
            public string center_classification_date { get; set; }
            public string report_date { get; set; }
            public string postal_code { get; set; }
            public string recall_initiation_date { get; set; }
            public string recall_number { get; set; }
            public string city { get; set; }
            public string event_id { get; set; }
            public string distribution_pattern { get; set; }
            public string recalling_firm { get; set; }
            public string voluntary_mandated { get; set; }
            public string state { get; set; }
            public string reason_for_recall { get; set; }
            public string initial_firm_notification { get; set; }
            public string status { get; set; }
            public string product_type { get; set; }
            public string country { get; set; }
            public string product_description { get; set; }
            public string code_info { get; set; }
            public string address_1 { get; set; }
            public string address_2 { get; set; }
            public string product_quantity { get; set; }


        public List<FDARecall> GetDataFromPublicApi(string uri)
        {
            List<FDARecall> dataTypes = new List<FDARecall>();
            using (HttpClient client = new HttpClient())
            using (System.IO.StreamReader sr = new StreamReader(client.GetStreamAsync(new Uri(uri)).Result))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                reader.SupportMultipleContent = true;
                dataTypes = serializer.Deserialize<List<FDARecall>>(reader);

            }

            return dataTypes;
        }
    }
    
}
