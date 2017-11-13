using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Repositories.ElasticSearch
{
    internal struct Settings
    {
        private const string ES_USERNAME = "esusername";
        private const string ES_PASSWORD = "espassword";
        private const string ES_URL = "esurl";
        
        public string UserName {
            get
            {
                var username = ConfigurationManager.AppSettings[ES_USERNAME];
                return string.IsNullOrEmpty(username)
                    ? Environment.GetEnvironmentVariable(ES_USERNAME)
                    : username;

            }
    
            }
        public string Password
        {
            get
            {
                var password = ConfigurationManager.AppSettings[ES_PASSWORD];
                return string.IsNullOrEmpty(password)
                    ? Environment.GetEnvironmentVariable(ES_USERNAME)
                    : password;

            }
        }
        public string ConnectionString
        {
            get
            {
                var url = ConfigurationManager.AppSettings[ES_URL];
                return string.IsNullOrEmpty(url)
                    ? Environment.GetEnvironmentVariable(ES_URL)
                    : url;

            }
        }
    }
}
