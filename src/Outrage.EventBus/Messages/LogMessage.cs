﻿using System.Collections.Generic;

namespace Outrage.EventBus.Messages
{
    public class LogMessage:IMessage
    {
        public LogMessage(string message)
        {
            this.Message = message;
        }

        public LogMessage(string message, params string[] details): this(message)
        {
            this.Details = details;
        }
        public string Message { get; set; }
        public ICollection<string> Details { get; set; }
    }
}
