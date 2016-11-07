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
using RawRabbit.vNext;
using RawRabbit;
using RawRabbit.Context.Enhancer;
using RawRabbit.Context;

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
                    container =>
                    {
                        container.AddSingleton(LoggingFactory.ApplicationLogger);
                    })
                .AddSingleton<IConfigurationEvaluator, AttributeConfigEvaluator>()
                //.AddSingleton<IContextEnhancer, ApplicationContextEnhancer>()
                .AddMvc();

            services.AddSingleton<EmailSender, EmailSender>();
            services.AddSingleton<EmailLogger, EmailLogger>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddSerilog(GetConfiguredSerilogger())
                .AddConsole(Configuration.GetSection("Logging"));

            app.AddMessageHandler<EmailMessage, EmailSender>();

            app.AddMessageHandler<EmailMessage, EmailLogger>();

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
