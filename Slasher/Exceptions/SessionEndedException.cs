using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class SessionEndedException : Exception
    {
        public SessionEndedException()
        {
        }

        public SessionEndedException(string message) : base(message)
        {
        }

        public SessionEndedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SessionEndedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}