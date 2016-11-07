using System.Threading.Tasks;
using Hutch.Controllers;
using Hutch.Extensions.RawRabbit;
using Microsoft.Extensions.Logging;
using RawRabbit;
using RawRabbit.Context;
using System;

namespace Hutch.Services
{
    public class EmailSender : IMessageHandler<EmailMessage>
    {
        public ILogger logger;
        IBusClient<AdvancedMessageContext> _busClient;

        public EmailSender(IBusClient<AdvancedMessageContext> busClient, ILoggerFactory loggerFactory)
        {
            _busClient = busClient;
            logger = loggerFactory.CreateLogger<EmailSender>();
        }

        public Task<bool> HandleMessageAsync(EmailMessage message, AdvancedMessageContext context)
        {
            logger.LogInformation($"Sending '{message.Body}' to '{message.To}'.");

            _busClient.PublishAsync<EmailMessage>(new EmailMessage() { To = "subscriber@gmail.com", Body = "Message published from subscriber." }, default(Guid), action => {
                action.WithExchange(e => {
                    e.WithName("email");
                })
                .WithRoutingKey("email")
                .WithProperties(p => {
                    p.Persistent = true;
                });
            });

            return Task.FromResult(true);
        }
    }
}