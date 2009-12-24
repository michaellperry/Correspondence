using System;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class NetworkSimulatorException : Exception
    {
        public NetworkSimulatorException()
        {
        }

        public NetworkSimulatorException(string message)
            : base(message)
        {
        }

        public NetworkSimulatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NetworkSimulatorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
