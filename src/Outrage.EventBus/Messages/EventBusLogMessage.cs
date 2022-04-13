using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outrage.EventBus.Messages
{
    public class EventBusLogMessage: IMessage
    {
        public EventBusLogMessage()
        {
            this.Level = LogLevel.Debug;
            this.Message = String.Empty;
        }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
    }
}
