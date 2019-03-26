// Project: ReadDeviceToCloudMessages
// ReadDeviceToCloudMessages.cs


using System;
using System.Collections.Generic;
using System.Text;

using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;


namespace ReadDeviceToCloudMessages
{
    // consume message from event hub
    // read the messages published by TempDevice
    // console log all the messages
    // Process data
    class ReadTempService
    {
 

        // Event Hub-compatible endpoint
        // az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}
        private readonly static string s_eventHubsCompatibleEndpoint = "{{connection-string}}";

        // Event Hub-compatible name
        // az iot hub show --query properties.eventHubEndpoints.events.path --name krishiothub
        private readonly static string s_eventHubsCompatiblePath = "{{path}}";

        // az iot hub policy show --name iothubowner --query primaryKey --hub-name krishiothub
        private readonly static string s_iotHubSasKey = "{{key}}";
        private readonly static string s_iotHubSasKeyName = "iothubowner";
        private static EventHubClient s_eventHubClient;

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            // Consumer/Subscriber
            // Create the receiver using the default consumer group.
            // For the purposes of this sample, read only messages sent since 
            // the time the receiver is created. Typically, you don't want to skip any messages.
            DateTime fromTime = DateTime.Now.AddHours(-3);
            var eventHubReceiver = s_eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(fromTime));
            Console.WriteLine("Create receiver on partition: " + partition);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                Console.WriteLine("Listening for messages on: " + partition);
                // consumer Pull data from event hub
                // Check for EventData - this methods times out if there is nothing to retrieve.
                var events = await eventHubReceiver.ReceiveAsync(100);

                // If there is data in the batch, process it.
                if (events == null) continue;

                foreach (EventData eventData in events)
                {
                    string data = Encoding.UTF8.GetString(eventData.Body.Array);
                    Console.WriteLine("Message received on partition {0}:", partition);
                    Console.WriteLine("  {0}:", data);
                    Console.WriteLine("Application properties (set by device):");
                    foreach (var prop in eventData.Properties)
                    {
                        Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }
                    Console.WriteLine("System properties (set by IoT Hub):");
                    foreach (var prop in eventData.SystemProperties)
                    {
                        Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }
                }
            }
        }

        public static async void Start()
        {

            Console.WriteLine("IoT Hub Quickstarts - Read device to cloud messages. Ctrl-C to exit.\n");

            // Create an EventHubClient instance to connect to the
            // IoT Hub Event Hubs-compatible endpoint.
            var connectionString = new EventHubsConnectionStringBuilder(new Uri(s_eventHubsCompatibleEndpoint), s_eventHubsCompatiblePath, s_iotHubSasKeyName, s_iotHubSasKey);
            s_eventHubClient = EventHubClient.CreateFromConnectionString(connectionString.ToString());

            // Create a PartitionReciever for each partition on the hub.
            var runtimeInfo = await s_eventHubClient.GetRuntimeInformationAsync();
            Console.WriteLine("**Partition Count " + runtimeInfo.PartitionCount);

            var d2cPartitions = runtimeInfo.PartitionIds;
            Console.WriteLine("Partition " + d2cPartitions);
            foreach (String p in d2cPartitions)
            {
                Console.WriteLine("P#" + p);
            }

            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };


            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
                // tasks.Add(ReceiveMessagesFromDeviceAsync("1", cts.Token));

            }

                // Wait for all the PartitionReceivers to finsih.
                Task.WaitAll(tasks.ToArray());


            
        }
    }
}

