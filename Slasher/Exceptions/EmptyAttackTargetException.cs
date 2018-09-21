using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class EmptyAttackTargetException : Exception
    {
        public EmptyAttackTargetException()
        {
        }

        public EmptyAttackTargetException(string message) : base(message)
        {
        }

        public EmptyAttackTargetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyAttackTargetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}