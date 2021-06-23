using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sefeko.AzureFunctionsTest.Common.Serialisation;
//using Sefeko.Common.Types.Events.Helpers.Serialisation;
using EventDataBatch = Azure.Messaging.EventHubs.Producer.EventDataBatch;


namespace Sefeko.AzureFunctions.Test
{
    public class SendToLegacySystem
    {

        /// <summary>
        /// Name of the Event Hub.
        /// </summary>
        private readonly string EventHubName = "v2eventhub";

        private readonly EventHubProducerClient _eventHubProducerClient;

        private readonly ISefekoJsonSerialiseSettings _serialiseSettings;


        public SendToLegacySystem(ISefekoJsonSerialiseSettings serialiseSettings, EventHubProducerClient eventHubProducerClient)
        {
            _serialiseSettings = serialiseSettings;
            _eventHubProducerClient = eventHubProducerClient;
        }



        [FunctionName("SendToLegacySystem")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "SefekoDevices",
            collectionName: "sefekotelemetry",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "leases",
            LeaseCollectionPrefix = "testlegacyeventhub",
            CreateLeaseCollectionIfNotExists = true,
            StartFromBeginning = false
        )]IReadOnlyList<Document> sefekoEvents, ILogger log)
        {
            log.LogInformation($"Sending {sefekoEvents.Count} Sefeko events from Cosmos DB change feed trigger to EventHub: ");

            var exceptions = new List<Exception>();
            using (var jsonSettings = new JsonSerializerConfig(_serialiseSettings.DefaultSettings()))
            {
                // Iterate through modified documents from change feed.
                foreach (var doc in sefekoEvents)
                {
                    try
                    {
                        EventDataBatch eventDataBatch = await _eventHubProducerClient.CreateBatchAsync();
                        // Convert documents to json.
                        string json = JsonConvert.SerializeObject(doc, jsonSettings.CurrentSettings);
                        eventDataBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes(json)));
                        // Use Event Hub client to send the change events to event hub.
                        await _eventHubProducerClient.SendAsync(eventDataBatch).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        // We need to keep processing the rest of the batch - capture this exception and continue.
                        // Also, consider capturing details of the message that failed processing so it can be processed again later.
                        exceptions.Add(e);
                        log.LogError(e, "Error occurred sending single event to " + EventHubName);
                    }
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.
            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
