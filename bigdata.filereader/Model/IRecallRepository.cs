using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
   public interface IRecallRepository
    {
        
        Recall Get(int Number);
        void Add(Recall recall);
        void Remove(Recall recall);

    }
}
