using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class OccupiedSlotException : Exception
    {
        public OccupiedSlotException()
        {
        }

        public OccupiedSlotException(string message) : base(message)
        {
        }

        public OccupiedSlotException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OccupiedSlotException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}