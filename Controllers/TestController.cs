using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hutch.Controllers
{
    using RawRabbit;
    using RawRabbit.Context;

    public class EmailMessage
    {
        public string To {get; set;}
        public string Body {get; set;}
    }

	public class TestController : Controller
	{
		private readonly IBusClient<AdvancedMessageContext> _busClient;
		private readonly ILogger<TestController> _logger;

		public TestController(IBusClient<AdvancedMessageContext> busClient, ILoggerFactory loggerFactory)
		{
			_busClient = busClient;
			_logger = loggerFactory.CreateLogger<TestController>();
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAsync()
		{

            await _busClient.PublishAsync(new EmailMessage() { To = "test@gmail.com", Body = "Message body."}, default(Guid), action => {
                action.WithExchange(e => {
                    e.WithName("email");
                })
                .WithRoutingKey("email")
                .WithProperties(p => {
                    p.Persistent = true;
                });
            });

			_logger.LogDebug("Published email message.");

			return Ok();
		}
	}
}