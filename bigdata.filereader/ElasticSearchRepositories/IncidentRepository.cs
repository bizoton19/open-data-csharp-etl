using bigdata.filereader.Model;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;

namespace bigdata.filereader.ElasticSearchRepositories
{
    public  struct Settings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
    }
    public class IncidentRepository : IIncidentRepository
    {
        private IConnectionSettingsValues config;
        private Node node;
        private SniffingConnectionPool connectionPool;
        Settings settings;
        public IncidentRepository()
        {
            Init();

        }

        private void Init()
        {
            settings = new Settings()
            {
                UserName = ConfigurationManager.AppSettings["username"],
                Password = ConfigurationManager.AppSettings["password"],
                ConnectionString = ConfigurationManager.AppSettings["elasticloundconnection"]
            };
   
            var node = new Uri(settings.ConnectionString);

            var connectionPool = new SniffingConnectionPool(new[] { node });

            config = new ConnectionSettings(new Uri(settings.ConnectionString))
               .DisableDirectStreaming()
               .BasicAuthentication(settings.UserName, settings.Password)
               .RequestTimeout(TimeSpan.FromSeconds(7));
        }

        public void Add(IncidentReport incident)
        {
           
            var descriptor = new CreateIndexDescriptor(incident.Type.ToLowerInvariant())
                .Mappings(ms => ms
                .Map<IncidentReport>(m => m.AutoMap())
                .Map<IncidentReport.RelationShipType>(m => m.AutoMap())
                .Map<IncidentReport.Gender>(m => m.AutoMap())
                .Map<IncidentReport.Locale>(m => m.AutoMap())
                .Map<IncidentReport.SeverityType>(m => m.AutoMap())
                .Map<IncidentReport.SourceType>(m => m.AutoMap())
                .Map<IncidentReport.ProductCategory>(m => m.AutoMap())
                .Map<IncidentReport.IncidentDocument>(m => m.AutoMap())
                );
            ElasticClient clien = new ElasticClient(config);
            var result =Nest.Indices.Index(incident.Type);
            if (!clien.IndexExists(new IndexExistsRequest(result)).Exists)
            {
                ICreateIndexResponse index = clien.CreateIndex(incident.Type.ToLowerInvariant(), x =>descriptor);
            };
            
            var indexResult = clien.Index<IncidentReport>(incident, i =>
             i.Index(incident.Type.ToLowerInvariant())
             .Id(incident.IncidentReportId.ToString())
             .Type(incident.Type.ToLowerInvariant())
             .Refresh());
    
        }

        public IncidentReport Get(int IncidentReporId)
        {
           
            var elastic = new ElasticClient(config);

            var resGet = elastic.GetAsync<IncidentReport>(new GetRequest("IncidentReport", "IncidentReport", IncidentReporId));
            return resGet.Result.Source;
        }

        public void Remove(IncidentReport incident)
        {
            throw new NotImplementedException();
        }
    }
}
