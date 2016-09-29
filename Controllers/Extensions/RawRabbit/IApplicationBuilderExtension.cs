using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Hutch.Services;
using System;
using RawRabbit.Configuration.Subscribe;
using RawRabbit;

namespace Hutch.Extensions.RawRabbit
{
    public static partial class IApplicationBuilderExtension
	{
        public static IApplicationBuilder AddMessageHandler<TMessage, TMessageHandler>(this IApplicationBuilder app, Action<ISubscriptionConfigurationBuilder> configuration = null) where TMessageHandler : IMessageHandler<TMessage>
        {
            var busClient = app.ApplicationServices.GetRequiredService<IBusClient<ApplicationMessageContext>>();

            busClient.SubscribeAsync<TMessage>(async (message, context) => {
                var handler = app.ApplicationServices.GetRequiredService<TMessageHandler>();

                await handler.HandleMessageAsync(message, context);
            }, configuration);

            return app;
        }
    }
}