using System;
using Nest;
using Elasticsearch.Net;
using OpenData.Shaper.Default.Repositories;
using CPSC.OpenData.Shaper.Model;

namespace CPSC.OpenData.Shaper.Repositories.ElasticSearch
{

    public class IncidentRepository : IGenericRepository<IncidentReport>
    {
        private IConnectionSettingsValues config;
        private const string indexNameSplitter = "-";
        Settings settings;
        public IncidentRepository()
        {
            Init();

        }

        private void Init()
        {
            settings = new Settings();
            
   
            var node = new Uri(settings.ConnectionString);

            var connectionPool = new SniffingConnectionPool(new[] { node });

            config = new ConnectionSettings(new Uri(settings.ConnectionString))
               .DisableDirectStreaming()
               .DisableAutomaticProxyDetection(false)
               .BasicAuthentication(settings.UserName, settings.Password)
               .RequestTimeout(TimeSpan.FromSeconds(10));
        }

        public void Add(IncidentReport incident)
        {
            string indexname = string.Concat(incident.ArtifactSource.ToLowerInvariant(), indexNameSplitter, incident.Type.ToLowerInvariant());
            var descriptor = new CreateIndexDescriptor(indexname)
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
            var result =Nest.Indices.Index(indexname);
            
            bool exist = clien.IndexExists(new IndexExistsRequest(result)).Exists;
            if(!exist) 
            {
                ICreateIndexResponse index = clien.CreateIndex(indexname, x =>descriptor);
            }
            
            var indexResult = clien.Index<IncidentReport>(incident, i =>
             i.Index(indexname)
             .Id(incident.IncidentReportId.ToString())
             .Type(incident.Type.ToLowerInvariant())
             .Refresh());
    
        }

        public IncidentReport Get(string IncidentReporId)
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
