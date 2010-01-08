using System;

namespace UpdateControls.Correspondence.WebService
{
    public class WebServiceException : Exception
    {
        public WebServiceException()
        {
        }

        public WebServiceException(string message)
            : base(message)
        {
        }

        public WebServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WebServiceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
