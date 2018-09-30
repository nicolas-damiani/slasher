using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class InvalidSurvivorAttackException : Exception
    {
        public InvalidSurvivorAttackException()
        {
        }

        public InvalidSurvivorAttackException(string message) : base(message)
        {
        }

        public InvalidSurvivorAttackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSurvivorAttackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}