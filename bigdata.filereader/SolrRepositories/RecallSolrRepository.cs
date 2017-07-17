using bigdata.filereader.Model;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.SolrRepositories
{
    public class RecallSolrRepository : IRecallRepository
    {
        private SolrNet.ISolrOperations<Recall> solr;
        private const string indexprefix = "cpsc-";
        //Settings settings;
        public RecallSolrRepository()
        {
            Init();

        }

        private void Init()
        {
           
            //var pingResponse = solr.Ping().Status;
            

        }

        Recall IRecallRepository.Get(int Number)
        {
            throw new NotImplementedException();
        }

        public void Add(Recall recall)
        {
            
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Recall>>();
            var mapper = new AllPropertiesMappingManager();
            //mapper.SetUniqueKey(typeof(Recall).GetProperty("RecallNumber"));
           
            solr.Add(recall);
            solr.Commit();
        }

        public void Remove(Recall recall)
        {
            throw new NotImplementedException();
        }
    }
    }
