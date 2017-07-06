using bigdata.filereader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;
using Microsoft.Practices.ServiceLocation;
using System.Configuration;
using SolrNet.Mapping;

namespace bigdata.filereader.SolrRepositories
{
    public class NeissSolrRepository : INeissReportRepository
    {
        private SolrNet.ISolrOperations<NeissReport> solr;
        private const string indexprefix = "cpsc-";
        //Settings settings;
        public NeissSolrRepository()
        {
            Init();

        }

        private void Init()
        {
            //settings = new Settings()
            //{
                //UserName = ConfigurationManager.AppSettings["username"],
                //Password = ConfigurationManager.AppSettings["password"],
                //ConnectionString = //ConfigurationManager.AppSettings["elasticcloudconnection"]
           // };
           
            //var node = new Uri("ConfigurationManager.AppSettings["elasticcloudconnection"]");
            Startup.Init<Dictionary<string, object>>(ConfigurationManager.AppSettings["solrConnection"]);
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<NeissReport>>();
          
        }

        public void Add(NeissReport report)
        {
            var mapper = new AllPropertiesMappingManager();
            mapper.SetUniqueKey(typeof(NeissReport).GetProperty("CpscCaseNumber"));
            solr.Add(report);
            solr.Commit();
           
        
        }

        public void Add(IEnumerable<NeissReport> reports, string suffix=null)
        {
            var mapper = new AllPropertiesMappingManager();
            mapper.SetUniqueKey(typeof(NeissReport).GetProperty("CpscCaseNumber"));
            solr.AddRange(reports);
            solr.Commit();
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
