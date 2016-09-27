using RawRabbit.Context;

namespace Hutch.Services
{
    public interface QueueMessageHandler<T>
    {
        bool HandleMessageAsync(T message, MessageContext context);
    }
}