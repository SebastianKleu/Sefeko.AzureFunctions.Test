using System;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Sefeko.AzureFunctionsTest.Common
{
    public static class LoggingHelper
    {
        private static bool _serilogConfigured;

        /// <summary>
        /// Configures serilog as part of the logging setup
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="enableEventflow"></param>
        /// <param name="applicationName"></param>
        /// <param name="serviceName"></param>
        /// <param name="level"></param>
        /// <param name="aiKey"></param>
        /// <param name="configuration"></param>
        private static void ConfigureSerilog(string environment, string applicationName,
            string serviceName, LogEventLevel level, string aiKey, IConfiguration configuration)
        {
            if (_serilogConfigured)
                return;

            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = aiKey;

            var config = new LoggerConfiguration()
                .MinimumLevel.Is(level)
                .Filter.ByExcluding(logEvent =>
                    logEvent.Exception != null &&
                    logEvent.Exception.GetType() == typeof(System.IO.IOException) &&
                    logEvent.Exception.Message.StartsWith("Authentication failed because the remote party has closed the transport stream.")
                )
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("Application.ServiceName", serviceName)
                .Enrich.WithProperty("Environment", environment)
                .Enrich.WithProperty("ServiceDataLayer", serviceName)
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName();

            var logLevels = configuration?.GetSection("Logging:LogLevel")?.GetChildren()?.ToArray();
            if (logLevels != null && logLevels.Any())
            {
                //For each source to override, translate the level and add it to Serilog minimum level
                foreach (var logLevel in logLevels)
                {
                    var logLevelEnum = LogLevel.Trace;
                    try
                    {
                        var logLevelToParse = logLevel.Value;
                        logLevelEnum = (LogLevel)Enum.Parse(typeof(LogLevel), logLevelToParse);
                    }
                    catch
                    {
                        // ignored
                    }

                    var translatedLevel = TranslateLevel(logLevelEnum);
                    var source = logLevel.Key;

                    //Override the source level
                    config = config.MinimumLevel.Override(source, translatedLevel);
                }
            }

            //Log to AI only if not development environment, if dev log to console only
            if (environment.Equals("Development") || System.Diagnostics.Debugger.IsAttached)
            {
                config = config.WriteTo.Async(a => a.Console(level));
                config = config.WriteTo.Async(a => a.Debug(level));
            }
            else
                config = config.WriteTo.Async(a => a.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces, level));

            Log.Logger = config.CreateLogger();
            _serilogConfigured = true;
        }


        /// <summary>
        /// Configures the logging for an application.
        /// Creates a event flow pipeline: Sefeko-{Application Name}{ServiceName}-DiagnosticsPipeline
        /// </summary>
        /// <param name="builder">ILoggingBuilder now used rather than ILoggerFactory</param>
        /// <param name="configuration">Application configuration needed for environment config</param>
        /// <param name="appName">Application Name like: Sefeko.Subscriptions</param>
        /// <param name="serviceName">Name of the particular service like: Sefeko.Subscriptions.Portal</param>
        public static ILoggingBuilder ConfigureAll(this ILoggingBuilder builder, IConfiguration configuration, string appName, string serviceName)
        {
            try
            {
                var key = configuration["Logging:AppInsightsInstrumentationKey"] ??
                      configuration["ApplicationInsights:InstrumentationKey"] ??
                      string.Empty;

                var environment = configuration["Environment"] ?? "Development";
                var minimumLevelText = configuration["Logging:MinimumLevel"] ?? "Debug";

                return ConfigureLogging(minimumLevelText, builder, environment, key, appName, serviceName, configuration);
            }
            catch
            {
                return builder;
            }
        }

        private static ILoggingBuilder ConfigureLogging(string minimumLevelText, ILoggingBuilder builder, string environment, string key, string appName, string serviceName, IConfiguration configuration)
        {
            if (!Enum.TryParse(minimumLevelText, true, out LogLevel level))
                level = LogLevel.Information;

            //Add serilog and other providers
            ConfigureSerilog(environment, appName, serviceName, TranslateLevel(level), key, configuration);
            return builder.AddSerilog()
                .SetMinimumLevel(level);
        }

        private static LogEventLevel TranslateLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                case LogLevel.None:
                    return LogEventLevel.Verbose;
                default:
                    return LogEventLevel.Debug;
            }
        }
    }
}
