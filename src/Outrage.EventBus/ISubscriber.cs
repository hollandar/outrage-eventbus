using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public interface ISubscriber
    {
        Task HandleAsync(IMessage message);
    }

    public interface ISubscriber<TMessage> where TMessage: IMessage
    {

    }
}
