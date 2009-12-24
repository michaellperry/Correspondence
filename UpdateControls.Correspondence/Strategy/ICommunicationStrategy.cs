using System;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICommunicationStrategy
    {
        void AttachFactRepository(IFactRepository repository);
    }
}
