using OpenData.Shaper.Model;

namespace OpenData.Shaper.Contracts
{
    public interface IRecallRepository
    {
        
        Recall Get(int Number);
        void Add(Recall recall);
        void Remove(Recall recall);

    }
}
