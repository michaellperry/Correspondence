using System;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICommunicationStrategy
    {
        void AttachMessageRepository(IMessageRepository repository);
    }
}
