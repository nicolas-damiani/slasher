using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class UserTurnLimitException : Exception
    {
        public UserTurnLimitException()
        {
        }

        public UserTurnLimitException(string message) : base(message)
        {
        }

        public UserTurnLimitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserTurnLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}