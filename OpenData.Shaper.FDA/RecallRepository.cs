using OpenData.Shaper.Default.Repositories;
using Nest;
using Elasticsearch.Net;
using OpenData.Shaper.Model;
using OpenData.Shaper.Model.FDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.FDA.Repositories.Elasticsearch
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
                      .String(t => t
                        .Name(title => title.Title)
                        .Index(FieldIndexOption.Analyzed)
                      )
                      .String(category => category
                        .Name(cat => cat.Category)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                       )
                       .String(country => country
                        .Name(c => c.country)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )
                         .String(city => city
                        .Name(ct => ct.city)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )
                       .String(state => state
                        .Name(st => st.state)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )
                        .String(status => status
                        .Name(sta => sta.status)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )
                         .String(pt => pt
                        .Name(pty => pty.product_type)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )
                         .String(cl => cl
                        .Name(cla => cla.classification)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )
                          .String(ifn => ifn
                        .Name(fn => fn.initial_firm_notification)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )
                         .String(recf =>recf
                        .Name(f => f.recalling_firm)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("keyword")
                                .NotAnalyzed()
                             )
                         )
                         )


                ).AutoMap()
                ));
            ElasticClient clien = new ElasticClient(config);

            var result = Nest.Indices.Index(_indexname);

            bool exist = clien.IndexExists(new IndexExistsRequest(result)).Exists;
            if (!exist)
            {
                ICreateIndexResponse index = clien.CreateIndex(_indexname, x => descriptor);
            }

            var indexResult = clien.Index<Recall>(recall, i =>
             i.Index(_indexname)
             .Id(recall.UUID.ToString().ToLowerInvariant())
             .Type(recall.Type.ToLowerInvariant())
             .Refresh());
        
        Console.WriteLine($"Added{recall.Title} to {_indexname}");


        }

        public Recall Get(string Number)
        {
            throw new NotImplementedException();
        }

        public void Remove(Recall recall)
        {
            throw new NotImplementedException();
        }
    }
}
