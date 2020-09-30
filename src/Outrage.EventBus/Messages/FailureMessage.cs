using System;
using System.Collections.Generic;
#pragma warning disable S2326 // Unused type parameters should be removed


namespace Outrage.EventBus.Messages
{

    public class FailureMessage : IMessage
    {
        public List<string> Messages { get; } = new List<string>();
        public Exception InnerException { get; set; }

        public FailureMessage(params string[] messages)
        {
            this.Messages.AddRange(messages);
        }

        public FailureMessage(Exception innerException, params string[] messages) : this(messages)
        {
            this.InnerException = innerException;
        }

    }

    public class FailureMessage<TExceptionType>: FailureMessage
    {
        public FailureMessage(params string[] messages) : base(messages) { }
        public FailureMessage(Exception innerException, params string[] messages) : base(innerException, messages) { }
    }
}
