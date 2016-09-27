using System.Threading.Tasks;
using RawRabbit.Context;

namespace Hutch.Services
{
    public interface IMessageHandler<T>
    {
        Task<bool> HandleMessageAsync(T message, MessageContext context);
    }
}