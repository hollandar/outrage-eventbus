using System;
using System.Runtime.Serialization;

namespace Outrage.EventBus
{
    [Serializable]
    internal class LoggerNotInjectedException : Exception
    {
        public LoggerNotInjectedException()
        {
        }

        public LoggerNotInjectedException(string message) : base(message)
        {
        }

        public LoggerNotInjectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LoggerNotInjectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}