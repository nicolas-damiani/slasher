using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class UserDeletedException : Exception
    {
        public UserDeletedException()
        {
        }

        public UserDeletedException(string message) : base(message)
        {
        }

        public UserDeletedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}