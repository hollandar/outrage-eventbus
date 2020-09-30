using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public interface ISubscriber
    {
        Task HandleAsync(EventContext context, IMessage message);
    }

    public interface ISubscriber<TMessage> where TMessage: IMessage
    {

    }
}
