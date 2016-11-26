using bigdata.filereader.Model;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.ElasticSearchRepositories
{
   
    public class RecallRepository:IRecallRepository
    {

            private IConnectionSettingsValues config;
            private const string indexprefix = "cpsc-";
            Settings settings;
        private string _indexname;

        public RecallRepository()
            {
                Init();

            }

            private void Init()
            {
                settings = new Settings()
                {
                    UserName = ConfigurationManager.AppSettings["username"],
                    Password = ConfigurationManager.AppSettings["password"],
                    ConnectionString = ConfigurationManager.AppSettings["elasticcloudconnection"]
                };

                var node = new Uri(settings.ConnectionString);

                var connectionPool = new SniffingConnectionPool(new[] { node });

                config = new ConnectionSettings(new Uri(settings.ConnectionString))
                   .DisableDirectStreaming()
                   .DisableAutomaticProxyDetection(false)
                   .BasicAuthentication(settings.UserName, settings.Password)
                   .RequestTimeout(TimeSpan.FromSeconds(10));
            }

            public void Add(Recall recall)
            {
                _indexname = string.Concat(indexprefix, recall.Type.ToLowerInvariant());
                var descriptor = new CreateIndexDescriptor(_indexname)
                    .Mappings(ms => ms
                    .Map<Recall>(m => m.AutoMap())
                    .Map<Recall.Hazard>(m => m.AutoMap())
                    .Map<Recall.Image>(m => m.AutoMap())
                    .Map<Recall.Injury>(m => m.AutoMap())
                    .Map<Recall.Manufacturer>(m => m.AutoMap())
                    .Map<Recall.ManufacturerCountry>(m => m.AutoMap())
                    .Map<Recall.Product>(m => m.AutoMap())
                    .Map<Recall.ProductUPC>(m => m.AutoMap())
                    .Map<Recall.Remedy>(m => m.AutoMap())
                    .Map<Recall.Retailer>(m => m.AutoMap())
                    .Map<Recall.Inconjunction<string>>(m => m.AutoMap())
                    );
                ElasticClient clien = new ElasticClient(config);
                var result = Nest.Indices.Index(_indexname);

                bool exist = clien.IndexExists(new IndexExistsRequest(result)).Exists;
                if (!exist)
                {
                    ICreateIndexResponse index = clien.CreateIndex(_indexname, x => descriptor);
                }

                var indexResult = clien.Index<Recall>(recall, i =>
                 i.Index(_indexname)
                 .Id(recall.RecallID.ToString())
                 .Type(recall.Type.ToLowerInvariant())
                 .Refresh());

            }

            public Recall Get(int recallId)
            {

                var elastic = new ElasticClient(config);

                var resGet = elastic.GetAsync<Recall>(new GetRequest(_indexname, "recall", recallId));
                return resGet.Result.Source;
            }

            public void Remove(Recall recall)
            {
                throw new NotImplementedException();
            }
        }
    


}
