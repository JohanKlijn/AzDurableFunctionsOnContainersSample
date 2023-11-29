namespace FunctionApp1
{
    using System;
    using System.Linq;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal class Program
    {
        private static void Main(string[] args)
        {
            ILoggerFactory? loggerFactory = null;
            ILogger? logger = null;

            try
            {

                loggerFactory = CreateLoggerFactory();

                logger = loggerFactory.CreateLogger<Program>();

                logger.LogInformation("Starting host...");

                IHost host = CreateHostBuilder(args).Build();
                host.Run();

                logger.LogInformation("Host started.");
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                if (logger != null)
                {
                    logger.LogCritical(ex, "The host terminated unexpectedly");
                }

                // Re-trow the exception, because Azure Function is responsible for what to do next. (I thought it will try to restart a couple of times, but I am not sure about this).
                throw;
            }
            finally
            {
                if (loggerFactory != null)
                {
                    loggerFactory.Dispose();
                }
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.Services.Configure<LoggerFilterOptions>(options =>
                    {
                        LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                            == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                        if (defaultRule is not null)
                        {
                            options.Rules.Remove(defaultRule);
                        }
                    });
                });

            return host;
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddApplicationInsights();
            });

            return loggerFactory;
        }
    }
}