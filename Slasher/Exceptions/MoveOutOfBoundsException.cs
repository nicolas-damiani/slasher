using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class MoveOutOfBoundsException : Exception
    {
        public MoveOutOfBoundsException()
        {
        }

        public MoveOutOfBoundsException(string message) : base(message)
        {
        }

        public MoveOutOfBoundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoveOutOfBoundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}