using bigdata.filereader.Model;
using bigdata.filereader.Model.Recalls;
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
    public class RecallSolrRepository //: IRecallRepository
    {
        private SolrNet.ISolrOperations<RecallDelimited> solr;
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

        //RecallBase IRecallRepository.Get(int Number)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(RecallBase recall)
        //{
           
        //    solr = ServiceLocator.Current.GetInstance<ISolrOperations<RecallDelimited>>();
        //    var mapper = new AllPropertiesMappingManager();
        //    //mapper.SetUniqueKey(typeof(Recall).GetProperty("RecallNumber"));
           
        //    solr.Add((RecallDelimited)recall);
        //    solr.Commit();
        //}

        //public void Remove(RecallBase recall)
        //{
        //    throw new NotImplementedException();
        //}

       
    }
    }
