
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;
using OpenData.Shaper.Contracts;
using OpenData.Shaper.Model;

namespace OpenData.Shaper.Repositories.ElasticSearch
{
    public class NeissReportRepository : INeissReportRepository
    {
        private IConnectionSettingsValues config;
        private const string indexNameSplitter = "-";
        Settings settings;
        public NeissReportRepository()
        {
            Init();

        }

        private void Init()
        {
            settings = new Settings();
           

            var node = new Uri(settings.ConnectionString);

            var connectionPool = new SniffingConnectionPool(new[] { node });

            config = new ConnectionSettings(new Uri(settings.ConnectionString))
               
               .DisableAutomaticProxyDetection(false)
               .BasicAuthentication(settings.UserName, settings.Password)
               .MaximumRetries(3)
               .RequestTimeout(TimeSpan.FromSeconds(60))
               ;
        }
       
        public  void Add(IEnumerable<NeissReport> reports,string indexsuffix=null)
        {
            string type = reports.First().Type.ToLowerInvariant().Replace(" ","");
            var source = reports.First().ArtifactSource.ToLowerInvariant();
            string indexname = string.Concat(source,indexNameSplitter, type);
            ElasticClient clien = BootstrapClient(indexname);
            if (reports.Any())
            {
                var indexResult = clien.IndexMany<NeissReport>(reports, indexname, type);
                
                
            }
                
          
        }
        public async void Add(NeissReport report)
        {
            string indexname = string.Concat(report.ArtifactSource,indexNameSplitter, report.Type.ToLowerInvariant());
            ElasticClient clien = BootstrapClient(indexname);
           
            var indexResult = await clien.IndexAsync<NeissReport>(report, i =>
             i.Index(indexname)
             .Id(report.CpscCaseNumber)
             .Type(report.GetType().Name.ToLowerInvariant())
             .Refresh());
        }

        private ElasticClient BootstrapClient(string indexname)
        {
            var descriptor = new CreateIndexDescriptor(indexname)
                .Mappings(ms => ms
                .Map<NeissReport>(m => m
                   .Properties(r => r
                           .String(type => type
                           .Name(nt => nt.Type)
                           .NotAnalyzed()
                           .Fields(tf => tf
                                .String(t => t
                                    .Name("raw")
                                    .NotAnalyzed()
                                    )
                                )
                            )

                    .String(artifactSource => artifactSource
                        .Name(source => source.ArtifactSource)
                        .NotAnalyzed()
                        .Fields(sf => sf
                            .String(t => t
                                .Name("raw")
                                .NotAnalyzed()
                             )
                         )
                       )

                      .String(rdesc => rdesc
                        .Name(desc => desc.Description)
                        .Index(FieldIndexOption.Analyzed)
                        

                      )
                       .String(rdesc => rdesc
                        .Name(desc => desc.Narrative)
                        .Index(FieldIndexOption.No)
                        .Store(false)

                      )
                       .String(t => t
                        .Name(title => title.Title)
                        .Index(FieldIndexOption.Analyzed)
                      )
                      .Nested<NeissReport.BodyPart>(nbp => nbp
                        .Name(b => b.NeissBodyPart)
                        .AutoMap()
                        .Properties(bp => bp
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                                .Index(FieldIndexOption.NotAnalyzed)
                                )
                            .String(type => type
                                .Name(t => t.Type)
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )
                                )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                    .Nested<NeissReport.Gender>(ng => ng
                        .Name(g => g.NeissGender)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                    .Nested<NeissReport.EventLocale>(ng => ng
                        .Name(g => g.NeissEventLocale)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                    .Nested<NeissReport.Fire>(ng => ng
                        .Name(g => g.NeissFire)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                     .Nested<NeissReport.Hospital>(ng => ng
                        .Name(g => g.NeissFire)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.PSU)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(s => s
                                .Name(st => st.Stratum)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                    .Nested<NeissReport.InjuryDiagnonis>(ng => ng
                        .Name(g => g.InjuryDiagnosis)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                     .Nested<NeissReport.InjuryDisposition>(ng => ng
                        .Name(g => g.NeissInjuryDisposition)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                     .Nested<NeissReport.Product>(ng => ng
                        .Name(g => g.Products)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                         )
                    )
                    .Nested<NeissReport.Race>(ng => ng
                        .Name(g => g.NeissRace)
                        .AutoMap()
                        .Properties(gend => gend
                            .String(code => code
                                .Name(c => c.Code)
                                .NullValue("0")
                            )
                            .String(desc => desc
                                .Name(d => d.Type)
                                .NullValue("N/A")
                                .Fields(f => f
                                    .String(sf => sf
                                        .Name("raw")
                                        .NotAnalyzed()
                                        )
                                    )

                             )
                            .String(desc => desc
                                .Name(d => d.Description)
                                .NullValue("N/A")
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
        );







                
            ElasticClient clien = new ElasticClient(config);
            var result = Nest.Indices.Index(indexname);

            bool exist = clien.IndexExists(new IndexExistsRequest(result)).Exists;
            if (!exist)
            {
                
                ICreateIndexResponse index = clien.CreateIndex(indexname, x => descriptor);
            }

            return clien;
        }

        public NeissReport Get(int CpscCaseNumber)
        {
            throw new NotImplementedException();
        }

        public void Remove(NeissReport report)
        {
            throw new NotImplementedException();
        }
    }
}
