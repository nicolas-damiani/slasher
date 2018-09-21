using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class AttackOutOfBoundsException : Exception
    {
        public AttackOutOfBoundsException()
        {
        }

        public AttackOutOfBoundsException(string message) : base(message)
        {
        }

        public AttackOutOfBoundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AttackOutOfBoundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}