using CPSC.OpenData.Shaper.Model;
using OpenData.Shaper.Default.Repositories;
using OpenData.Shaper.FDA.Repositories.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.FDA
{
   public class RecallService
    {
        private IGenericRepository<Recall>  _repo;
        public RecallService(IGenericRepository<Recall>  repo)
        {
            _repo = repo;
        }
    }
}
