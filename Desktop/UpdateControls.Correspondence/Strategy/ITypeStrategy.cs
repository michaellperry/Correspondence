using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ITypeStrategy
    {
        CorrespondenceFactType GetTypeOfFact(CorrespondenceFact fact);
    }
}
