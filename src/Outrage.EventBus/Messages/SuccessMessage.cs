#pragma warning disable S2326 // Unused type parameters should be removed

namespace Outrage.EventBus.Messages
{
    public class SuccessMessage<TTarget>: IMessage where TTarget: IMessage
    {
    }
}
