using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outrage.EventBus.Messages
{
    public class EventBusLogMessage: IMessage
    {
        public LogLevel Level { get; set; } = LogLevel.Debug;
        public string Message { get; set; }
    }
}
