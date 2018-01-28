using Elasticsearch.Net;
using Nest;
using OpenData.Shaper.Contracts;
using OpenData.Shaper.Default.Repositories;
using CPSC.OpenData.Shaper.Model;
using System;

namespace CPSC.OpenData.Shaper.Repositories.ElasticSearch
{
    public class RecallRepository : IGenericRepository<Recall>
    {
        private IConnectionSettingsValues config;
        private const string indexprefix = "-";
        private Settings settings;
        private string _indexname;

        public RecallRepository()
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

        public void Add(Recall recall)
        {
            _indexname = string.Concat(recall.ArtifactSource.ToLowerInvariant(), indexprefix, recall.Type.ToLowerInvariant());
            var descriptor = new CreateIndexDescriptor(_indexname)
                .Mappings(ms => ms
                .Map<Recall>(m => m
                   .AutoMap()
                    .Properties(r => r
                     .String(type => type
                        .Name(rt => rt.Type)
                        .NotAnalyzed()
                        .Fields(tf => tf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                            )
                        )

                      )
                      .String(category => category
                        .Name(cat => cat.Category)
                        .NotAnalyzed()
                        .Fields(csf => csf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                       )
                      .String(artifactSource => artifactSource
                        .Name(source => source.ArtifactSource)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                       )

                      .String(rdesc => rdesc
                        .Name(desc => desc.Description)
                        .Index(FieldIndexOption.Analyzed)

                      )
                      .String(cc => cc
                        .Name(cont => cont.ConsumerContact)
                        .Index(FieldIndexOption.Analyzed)
                        .Store(false)
                      )
                      .String(t => t
                        .Name(title => title.Title)
                        .Index(FieldIndexOption.Analyzed)
                      )
                     .Nested<Recall.Product>(p => p
                        .Name(c => c.Products)
                        .AutoMap()
                        .Properties(prods => prods
                            .String(id => id
                                .Name(i => i.CategoryID)
                                .NullValue("0")
                                .Index(FieldIndexOption.NotAnalyzed)
                            )
                            .String(type => type
                                .Name(t => t.Type)
                                .Fields(f => f // fields are additional props
                                    .String(sf => sf
                                        .Name("keyword") //es will store as raw field for aggregation purposes
                                        .NotAnalyzed()  //do not analyze this field just store it as raw text
                            )

                          )

                        )
                    )
                )
                .Nested<Recall.Manufacturer>(rm => rm
                    .Name(mu => mu.Manufacturers)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.CompanyID)
                            .NullValue("0")
                        )
                        .String(name => name
                            .Name(n => n.Name)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                )
                .Nested<Recall.Retailer>(rr => rr
                    .Name(ret => ret.Retailers)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.CompanyID)
                            .NullValue("0")
                        )
                        .String(name => name
                            .Name(n => n.Name)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                )
                .Nested<Recall.ManufacturerCountry>(mc => mc

                    .Name(rmc => rmc.ManufacturerCountries)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.Country)
                            .NullValue("none")
                        )
                        .String(name => name
                            .Name(n => n.Country)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("raw")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                )

              )
            )
         .Map<Recall.Image>(m => m.AutoMap())
         .Map<Recall.Injury>(m => m.AutoMap()
                .Properties(i => i
                    .String(inj => inj
                        .Name(injury => injury.Name)
                        .Store(false)
                        .Index(FieldIndexOption.Analyzed)
                    )
                )
            )
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

        public Recall Get(string recallId)
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