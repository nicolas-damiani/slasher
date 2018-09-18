using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class BoundsException : Exception
    {
        public BoundsException()
        {
        }

        public BoundsException(string message) : base(message)
        {
        }

        public BoundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BoundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}