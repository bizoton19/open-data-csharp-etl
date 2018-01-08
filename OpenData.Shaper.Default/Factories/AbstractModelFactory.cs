using OpenData.Shaper.Contracts;


namespace OpenData.Shaper.Factories
{
    public abstract class AbstractModelFactory
    {
        public abstract IArtifact CreateModel<T>(T type)where T: IArtifact;
        
    }
}
