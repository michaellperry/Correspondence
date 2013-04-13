using System.IO;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICorrespondenceFactFactory
    {
        CorrespondenceFact CreateFact(FactMemento memento);
        void WriteFactData(CorrespondenceFact obj, BinaryWriter output);
        CorrespondenceFact GetUnloadedInstance();
        CorrespondenceFact GetNullInstance();
    }
}
