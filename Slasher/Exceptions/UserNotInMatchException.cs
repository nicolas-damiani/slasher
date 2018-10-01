using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class UserNotInMatchException : Exception
    {
        public UserNotInMatchException()
        {
        }

        public UserNotInMatchException(string message) : base(message)
        {
        }

        public UserNotInMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNotInMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}