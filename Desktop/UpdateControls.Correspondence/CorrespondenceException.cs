using System;

namespace UpdateControls.Correspondence
{
    public class CorrespondenceException : Exception
    {
        public CorrespondenceException(string message)
            : base(message)
        {
        }

        public CorrespondenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
