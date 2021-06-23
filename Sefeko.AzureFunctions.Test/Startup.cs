using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sefeko.AzureFunctions.Test;
using Sefeko.AzureFunctionsTest.Common;
using Sefeko.AzureFunctionsTest.Common.Serialisation;

[assembly: InternalsVisibleTo("Sefeko.Device.ServerlessFunctions.Tests")]
[assembly: FunctionsStartup(typeof(Startup))]
namespace Sefeko.AzureFunctions.Test
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var tracingKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            var environment = Environment.GetEnvironmentVariable("Environment") ?? "Development";
            var loggingLevel = Environment.GetEnvironmentVariable("LoggingLevel");

            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.Add(new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Logging:AppInsightsInstrumentationKey",tracingKey),
                    new KeyValuePair<string, string>("Environment",environment),
                    new KeyValuePair<string, string>("Logging:MinimumLevel",loggingLevel),
                    new KeyValuePair<string, string>("TenantInfo:SefekoTenantClientId","fd1189b2-731a-4c85-86ca-6e3cede67096"),
                    new KeyValuePair<string, string>("TenantInfo:SefekoTenantClientSecret","ES6HVsmohZyPhADQ78u7YXTgwpK32nJhlTrPqE42EtI=")
                }
            });
            configuration.AddEnvironmentVariables();
            var config = configuration.Build();

            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ConfigureAll(config, "Sefeko.DeviceTest", "Sefeko.DeviceTest.SendToLegacySystem");
            });
            builder.Services.AddTransient<IRegistryManagerFactory, RegistryManagerFactory>();
            builder.Services.RegisterSerialisationClients();

            //Add a singleton EventHubClient instance to ensure all invocations use the same connection and ensure we dont reach our connection limit
            //See documentation: https://docs.microsoft.com/en-us/azure/azure-functions/manage-connections
            var eventHubNamespaceConnection = Environment.GetEnvironmentVariable("EventHubNamespaceConnection");
            builder.Services.AddSingleton(
                s => new EventHubProducerClient(eventHubNamespaceConnection, "v2eventhub"));

        }
    }
}
