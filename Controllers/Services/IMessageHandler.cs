using System.Threading.Tasks;
using Hutch.Extensions.RawRabbit;

namespace Hutch.Services
{
    public interface IMessageHandler<T>
    {
        Task<bool> HandleMessageAsync(T message, ApplicationMessageContext context);
    }
}