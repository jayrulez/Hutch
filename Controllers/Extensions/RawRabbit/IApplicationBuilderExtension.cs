using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.Extensions.Client;
using RawRabbit.Context;
using Hutch.Services;
using System.Threading.Tasks;

namespace Hutch.Extensions.RawRabbit
{
    public static partial class IApplicationBuilderExtension
	{
        public static IApplicationBuilder UseSubscriber<TMessage, TMessageHandler>(IApplicationBuilder app) where TMessageHandler : QueueMessageHandler<TMessage>
        {
            var busClient = app.ApplicationServices.GetRequiredService<IBusClient<MessageContext>>();

            busClient.SubscribeAsync<TMessage>((message, context) => {
                var handler = app.ApplicationServices.GetRequiredService<TMessageHandler>();

                var result = handler.HandleMessageAsync(message, context);
                
                return Task.FromResult(result);
            });

            return app;
        }
    }
}