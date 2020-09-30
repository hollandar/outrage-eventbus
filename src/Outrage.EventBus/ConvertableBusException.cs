using System;
using System.Runtime.Serialization;

namespace Outrage.EventBus
{
    [Serializable]
    public abstract class ConvertableBusException: Exception
    {
        protected ConvertableBusException(string message) : base(message)
        {
        }

        protected ConvertableBusException(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected ConvertableBusException(
           SerializationInfo info,
           StreamingContext context) : base(info, context)
        {

        }

        public abstract IMessage Convert(IMessage source);
    }
}
