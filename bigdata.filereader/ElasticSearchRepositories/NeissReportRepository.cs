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
    public class NeissReportRepository : INeissReportRepository
    {
        private IConnectionSettingsValues config;
        private const string indexprefix = "cpsc-";
        Settings settings;
        public NeissReportRepository()
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
               
               .DisableAutomaticProxyDetection(false)
               .BasicAuthentication(settings.UserName, settings.Password)
               .MaximumRetries(3)
               .RequestTimeout(TimeSpan.FromSeconds(60))
               ;
        }
       
        public  void Add(IEnumerable<NeissReport> reports,string indexsuffix=null)
        {
            string type = "neissresolvedlookup";
            string indexname = string.Concat(indexprefix, type,"-",string.IsNullOrEmpty(indexsuffix)?string.Empty:indexsuffix);
            ElasticClient clien = BootstrapClient(indexname);
            if (reports.Any())
            {
                var indexResult = clien.IndexMany<NeissReport>(reports, indexname, type);
                Console.WriteLine($"added 1 more batch @ {DateTime.Now} with a request body of {Environment.NewLine} {indexResult.CallDetails.Uri} the response was {indexResult.CallDetails.HttpStatusCode}");
                
            }
                
          
        }
        public async void Add(NeissReport report)
        {
            string indexname = string.Concat(indexprefix, report.GetType().Name.ToLowerInvariant());
            ElasticClient clien = BootstrapClient(indexname);
            Console.WriteLine($"adding {report.CpscCaseNumber} to elastic search");
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
                .Map<NeissReport>(m => m.AutoMap())
                .Map<NeissReport.BodyPart>(m => m.AutoMap())
                .Map<NeissReport.Gender>(m => m.AutoMap())
                .Map<NeissReport.EventLocale>(m => m.AutoMap())
                .Map<NeissReport.Fire>(m => m.AutoMap())
                .Map<NeissReport.Hospital>(m => m.AutoMap())
                .Map<NeissReport.InjuryDiagnonis>(m => m.AutoMap())
                .Map<NeissReport.InjuryDisposition>(m => m.AutoMap())
                .Map<NeissReport.Product>(m => m.AutoMap())
                .Map<NeissReport.Race>(m => m.AutoMap())
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
