using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ITypeStrategy
    {
        CorrespondenceFactType GetTypeOfFact(CorrespondenceFact fact);
        IEnumerable<CorrespondenceFactType> GetAllTypesOfFact(CorrespondenceFact fact);
    }
}
