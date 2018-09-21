using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class InvalidAttackException : Exception
    {
        public InvalidAttackException()
        {
        }

        public InvalidAttackException(string message) : base(message)
        {
        }

        public InvalidAttackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAttackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}