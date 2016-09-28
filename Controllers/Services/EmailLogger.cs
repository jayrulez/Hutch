using System.Threading.Tasks;
using Hutch.Controllers;
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

        public Task<bool> HandleMessageAsync(EmailMessage message, MessageContext context)
        {
            logger.LogInformation($"Logging '{message.Body}' for '{message.To}'.");
            
            return Task.FromResult(true);
        }
    }
}