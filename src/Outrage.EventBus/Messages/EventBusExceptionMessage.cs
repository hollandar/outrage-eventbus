using System;
using System.Collections.Generic;
using System.Text;

namespace Outrage.EventBus.Messages
{
    public class EventBusExceptionMessage: IMessage
    {
        public AggregateException Exception { get; set; }
    }
}
