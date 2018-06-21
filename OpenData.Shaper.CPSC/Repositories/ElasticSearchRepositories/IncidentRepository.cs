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
                .Map<IncidentReport>(m =>
                    m.AutoMap()
                        .Properties(i => i
                            .String(type => type
                                .Name(rt => rt.Type)
                                .NotAnalyzed()
                                .Fields(tf => tf
                                    .String(t => t
                                        .Name("keyword")
                                        .NotAnalyzed()
                                    )
                                )


                ).String(category => category
                    .Name(cat => cat.Category)
                    .NotAnalyzed()
                    .Fields(csf => csf
                        .String(t => t
                            .Name("keyword")
                            .NotAnalyzed()
                            )
                        )
                 ).String(asrc => asrc
                    .Name(source => source.ArtifactSource)
                    .NotAnalyzed()
                    .Fields(sf => sf
                        .String(t => t
                            .Name("keyword")
                            .NotAnalyzed()
                            )
                        )
                    )
                .String(idesc => idesc
                    .Name(desc => desc.Description)
                    .Index(FieldIndexOption.Analyzed)
                    )
                 .String(cc => cc
                    .Name(cont => cont.CompanyCommentsExpended)
                    .Index(FieldIndexOption.Analyzed)
                    .Store(false)
                    )
                    .String(ti => ti
                        .Name(title => title.Title)
                        .Index(FieldIndexOption.Analyzed)
                        )


                .Nested<IncidentReport.RelationShipType>(rt => rt
                  .Name(c => c.Relationship)
                  .AutoMap()
                  .Properties(rel => rel
                   .String(typeId => typeId
                       .Name(r => r.RelationshipTyeId)
                       .NullValue("0")
                       .Index(FieldIndexOption.NotAnalyzed)
               )
                .String(type => type
                               .Name(t => t.RelationshipTypePublicName)
                               .Fields(f => f // fields are additional props
                                   .String(sf => sf
                                       .Name("keyword") //es will store as raw field for aggregation purposes
                                       .NotAnalyzed()  //do not analyze this field just store it as raw text
                           )

                         )

                       )
                        )
                        ).Nested<IncidentReport.ProductCategory>(pc => pc
                        .Name(p => p.IncidentProductCategory)
                        .AutoMap()
                        .Properties(cat => cat
                        .String(name => name
                        .Name(n => n.ProductCategoryPublicName)
                        .Fields(f => f
                        .String(sf => sf
                        .Name("keyword")
                        .NotAnalyzed()
                        )
                        )
                        )
                        )
                        ).Nested<IncidentReport.Gender>(rm => rm
                    .Name(mu => mu.VictimGender)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.GenderId)
                            .NullValue("0")
                        )
                        .String(name => name
                            .Name(n => n.GenderPublicName)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                )
               .Nested<IncidentReport.Locale>(rm => rm
                    .Name(mu => mu.IncidentLocale)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.LocalId)
                            .NullValue("0")
                        )
                        .String(name => name
                            .Name(n => n.LocalPublicName)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                )
                .Nested<IncidentReport.SeverityType>(rm => rm
                    .Name(mu => mu.IncidentSeverityType)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.SeverityTypeId)
                            .NullValue("0")
                        )
                        .String(name => name
                            .Name(n => n.SeverityTypePublicName)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                ).Nested<IncidentReport.SourceType>(rm => rm
                    .Name(mu => mu.IncidentSourceType)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.SourceTypeId)
                            .NullValue("0")
                        )
                        .String(name => name
                            .Name(n => n.SourceTypePublicName)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                ).Nested<IncidentReport.IncidentDocument>(rm => rm
                    .Name(mu => mu.IncidentDocuments)
                    .AutoMap()
                    .Properties(manu => manu
                        .String(id => id
                            .Name(n => n.DocumentId)
                            .NullValue("0")
                        )
                        .String(name => name
                            .Name(n => n.DocumentLocation)
                            .Fields(f => f
                                .String(sf => sf
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                        .String(name1 => name1
                            .Name(n1 => n1.FileName)
                            .Fields(f1 => f1
                                .String(sf1 => sf1
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )

                        .String(name2 => name2
                            .Name(n2 => n2.FileExtension)
                            .Fields(f2 => f2
                                .String(sf2 => sf2
                                    .Name("keyword")
                                    .NotAnalyzed()
                                 )
                            )
                        )
                   )
                ))));
               
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
             .Refresh(Refresh.True));

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
