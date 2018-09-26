using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class MonsterWinsException : Exception
    {
        public MonsterWinsException()
        {
        }

        public MonsterWinsException(string message) : base(message)
        {
        }

        public MonsterWinsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MonsterWinsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}