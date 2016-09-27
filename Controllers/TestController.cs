using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Hutch.Controllers
{
	using IBusClient = RawRabbit.Extensions.Client.IBusClient;

    public class EmailMessage
    {
        public string To {get; set;}
        public string Body {get; set;}
    }

	public class TestController : Controller
	{
		private readonly IBusClient _busClient;
		private readonly ILogger<TestController> _logger;

		public TestController(IBusClient busClient, ILoggerFactory loggerFactory)
		{
			_busClient = busClient;
			_logger = loggerFactory.CreateLogger<TestController>();
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAsync()
		{

            await _busClient.PublishAsync(new EmailMessage() { To = "test@gmail.com", Body = "Message body."});

			_logger.LogDebug("Published email message.");

			return Ok();
		}
	}
}