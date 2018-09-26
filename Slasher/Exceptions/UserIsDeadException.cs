using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class UserIsDeadException : Exception
    {
        public UserIsDeadException()
        {
        }

        public UserIsDeadException(string message) : base(message)
        {
        }

        public UserIsDeadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserIsDeadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}