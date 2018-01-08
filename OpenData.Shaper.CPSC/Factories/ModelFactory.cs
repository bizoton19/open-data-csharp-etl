using OpenData.Shaper.Contracts;
using OpenData.Shaper.Factories;
using CPSC.OpenData.Shaper.Model;

namespace CPSC.OpenData.Shaper.Factories
{
    
    

    /// <summary>
    /// Defines the <see cref="ModelFactory" />
    /// </summary>
    public class ModelFactory : AbstractModelFactory
    {
        /// <summary>
        /// The CreateModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The <see cref="T"/></param>
        /// <returns>The <see cref="IArtifact"/></returns>
        public override IArtifact CreateModel<T>(T type)
        {
            switch (type.GetType().Name)
            {
                case "Recall":
                    return new Recall();
                case "RecallDelimited":
                    return new RecallDelimited();
                case "RecallReduced":
                    return new RecallReduced();
                case "NeissReport":
                    return new NeissReport();
                case "IncidentReport":
                    return new IncidentReport();

            }
            return default(T);
        }
    }
}
