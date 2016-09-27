using Hutch.Controllers;
using Microsoft.Extensions.Logging;
using RawRabbit.Context;

namespace Hutch.Services
{
    public class EmailMessageHandler : QueueMessageHandler<EmailMessage>
    {
        public ILogger logger;

        public EmailMessageHandler(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<EmailMessageHandler>();
        }

        public bool HandleMessageAsync(EmailMessage message, MessageContext context)
        {
            logger.LogInformation($"Sending '{message.Body}' to '{message.To}'.");

            return true;
        }
    }
}