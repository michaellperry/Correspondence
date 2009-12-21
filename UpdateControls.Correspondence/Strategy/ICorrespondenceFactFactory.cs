using System.IO;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICorrespondenceFactFactory
    {
        CorrespondenceFact CreateFact(Memento memento);
        void WriteFactData(CorrespondenceFact obj, BinaryWriter output);
    }
}
