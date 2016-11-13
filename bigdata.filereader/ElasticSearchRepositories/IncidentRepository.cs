using bigdata.filereader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;

namespace bigdata.filereader.ElasticSearchRepositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private ConnectionConfiguration config;
        private Node node;
        private SniffingConnectionPool connectionPool;
        public IncidentRepository()
        {
            var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>("elastic-config.json");
            var node = new Uri(settings.connectionstring);

            var connectionPool = new SniffingConnectionPool(new[] { node });

            var config = new ConnectionConfiguration(connectionPool)
                .DisableDirectStreaming()
                .BasicAuthentication(settings.username, settings.password)
                .RequestTimeout(TimeSpan.FromSeconds(5));
        }
        public void Add(IncidentReport incident)
        {
            var client = new ElasticLowLevelClient(config);
            var descriptor = new CreateIndexDescriptor("IncidentReport")
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
            
        }

        public IncidentReport Get(int IncidentReporId)
        {
            throw new NotImplementedException();
        }

        public void Remove(IncidentReport incident)
        {
            throw new NotImplementedException();
        }
    }
}
