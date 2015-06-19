using System.IO;
using Correspondence.Mementos;

namespace Correspondence.Strategy
{
    public interface ICorrespondenceFactFactory
    {
        CorrespondenceFact CreateFact(FactMemento memento);
        void WriteFactData(CorrespondenceFact obj, BinaryWriter output);
        CorrespondenceFact GetUnloadedInstance();
        CorrespondenceFact GetNullInstance();
    }
}
