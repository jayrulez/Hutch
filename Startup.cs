using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawRabbit.Attributes;
using RawRabbit.Common;
using RawRabbit.vNext.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;
using Hutch.Controllers;
using Hutch.Services;
using Hutch.Extensions.RawRabbit;
using RawRabbit.Context;
using RawRabbit.vNext;
using RawRabbit;

namespace Hutch
{
	public class Startup
	{
		private readonly string _rootPath;

		public Startup(IHostingEnvironment env)
		{
			_rootPath = env.ContentRootPath;
			var builder = new ConfigurationBuilder()
				.SetBasePath(_rootPath)
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddRawRabbit<AdvancedMessageContext>(
                    config => config.SetBasePath(_rootPath)
                        .AddJsonFile("rawrabbit.json"),
					container => { 
                        container.AddSingleton(LoggingFactory.ApplicationLogger);
                    })
				.AddSingleton<IConfigurationEvaluator, AttributeConfigEvaluator>()
				.AddMvc();

                //services.AddSingleton<IBusClient<AdvancedMessageContext>>(BusClientFactory.CreateDefault<AdvancedMessageContext>());
                services.AddSingleton<EmailSender, EmailSender>();
                services.AddSingleton<EmailLogger, EmailLogger>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory
				.AddSerilog(GetConfiguredSerilogger())
				.AddConsole(Configuration.GetSection("Logging"));
            
            app.AddMessageHandler<EmailMessage, EmailSender>(c => 
            {
                c.WithExchange(e => {
                    e.WithName("email");
                    e.WithAutoDelete(false);
                })
                .WithRoutingKey("email")
                .WithNoAck(false)
                .WithQueue(q => {
                    q.WithName("email_sender");
                    q.WithAutoDelete(false);
                    q.WithDurability(true);
                    q.WithExclusivity(false);
                });
            });
            
            app.AddMessageHandler<EmailMessage, EmailLogger>(c => 
            {
                c.WithExchange(e => {
                    e.WithName("email");
                    e.WithAutoDelete(false);
                })
                .WithRoutingKey("email")
                .WithNoAck(false)
                .WithQueue(q => {
                    q.WithName("email_logger");
                    q.WithAutoDelete(false);
                    q.WithDurability(true);
                    q.WithExclusivity(false);
                });
            });
            
			app.UseMvc();
		}

		private ILogger GetConfiguredSerilogger()
		{
			return new LoggerConfiguration()
				.WriteTo.File($"{_rootPath}/Logs/serilog.log", LogEventLevel.Debug)
				.CreateLogger();
		}
	}
}
