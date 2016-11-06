using System.Threading.Tasks;
using Hutch.Controllers;
using Hutch.Extensions.RawRabbit;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace Hutch.Services
{
    public class EmailSender : IMessageHandler<EmailMessage>
    {
        public ILogger logger;
        IBusClient<ApplicationMessageContext> _busClient;

        public EmailSender(IBusClient<ApplicationMessageContext> busClient, ILoggerFactory loggerFactory)
        {
            _busClient = busClient;
            logger = loggerFactory.CreateLogger<EmailSender>();
        }

        public Task<bool> HandleMessageAsync(EmailMessage message, ApplicationMessageContext context)
        {
            logger.LogInformation($"Sending '{message.Body}' to '{message.To}'.");

            //context.RetryLater(TimeSpan.FromMinutes(5));

            //context.Nack();

            _busClient.PublishAsync<object>(new
            {
                id = 1
            });

            return Task.FromResult(true);
        }
    }
}