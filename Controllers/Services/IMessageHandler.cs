using System.Threading.Tasks;
using Hutch.Extensions.RawRabbit;
using RawRabbit.Context;

namespace Hutch.Services
{
    public interface IMessageHandler<T>
    {
        Task<bool> HandleMessageAsync(T message, AdvancedMessageContext context);
    }
}