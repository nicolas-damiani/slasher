using System;
using System.Runtime.Serialization;

namespace Exceptions
{
    [Serializable]
    public class SurvivorsWinException : Exception
    {
        public SurvivorsWinException()
        {
        }

        public SurvivorsWinException(string message) : base(message)
        {
        }

        public SurvivorsWinException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SurvivorsWinException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}