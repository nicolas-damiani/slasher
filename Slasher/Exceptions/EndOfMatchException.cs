using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class EndOfMatchException : Exception
    {
        public EndOfMatchException()
        {
        }

        public EndOfMatchException(string message) : base(message)
        {
        }

        public EndOfMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EndOfMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}