using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class ServerSystemException : Exception
    {
        public ServerSystemException()
        {
        }

        public ServerSystemException(string message) : base(message)
        {
        }

        public ServerSystemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerSystemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}