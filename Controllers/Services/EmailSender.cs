using System.Threading.Tasks;
using Hutch.Controllers;
using Hutch.Extensions.RawRabbit;
using Microsoft.Extensions.Logging;

namespace Hutch.Services
{
    public class EmailSender : IMessageHandler<EmailMessage>
    {
        public ILogger logger;

        public EmailSender(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<EmailSender>();
        }

        public Task<bool> HandleMessageAsync(EmailMessage message, ApplicationMessageContext context)
        {
            logger.LogInformation($"Sending '{message.Body}' to '{message.To}'.");

            //context.RetryLater(TimeSpan.FromMinutes(5));

            //context.Nack();

            return Task.FromResult(true);
        }
    }
}