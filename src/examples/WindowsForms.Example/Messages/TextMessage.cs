using Outrage.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms.Example.Messages
{
    public class TextMessage: IMessage
    {
        public TextMessage(string msg)
        {
            this.Message = msg;
        }

        public string Message { get; init; } = String.Empty;
    }
}
