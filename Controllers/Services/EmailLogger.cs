using System.Threading.Tasks;
using Hutch.Controllers;
using Hutch.Extensions.RawRabbit;
using Microsoft.Extensions.Logging;
using RawRabbit.Context;

namespace Hutch.Services
{
    public class EmailLogger : IMessageHandler<EmailMessage>
    {
        public ILogger logger;

        public EmailLogger(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<EmailLogger>();
        }

        public Task<bool> HandleMessageAsync(EmailMessage message, ApplicationMessageContext context)
        {
            logger.LogInformation($"Logging '{message.Body}' for '{message.To}'.");

            context.Nack(false);
            
            return Task.FromResult(true);
        }
    }
}