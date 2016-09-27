using System.Threading.Tasks;
using Hutch.Controllers;
using Microsoft.Extensions.Logging;
using RawRabbit.Context;

namespace Hutch.Services
{
    public class EmailMessageHandler : IMessageHandler<EmailMessage>
    {
        public ILogger logger;

        public EmailMessageHandler(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<EmailMessageHandler>();
        }

        public Task<bool> HandleMessageAsync(EmailMessage message, MessageContext context)
        {
            logger.LogInformation($"Sending '{message.Body}' to '{message.To}'.");

            return Task.FromResult(true);
        }
    }
}