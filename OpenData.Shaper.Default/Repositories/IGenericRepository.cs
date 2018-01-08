using OpenData.Shaper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Default.Repositories
{
    public interface IGenericRepository<T> where T : IArtifact
    {
        T Get(string Number);
        void Add(T data);
        void Remove(T data);
    }
}
