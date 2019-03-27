// Program.cs
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureIoTDemo
{
    class Program
    {
        static RegistryManager registryManager;
 
        static string connectionString = "HostName=krishiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=P/XEf5Az4g5YwiqfDFGw8SClKv2x4jfUcbzBISmcECY=";
 
        public static async Task AddTagsAndQuery()
        {
            var twin = await registryManager.GetTwinAsync("device-1");
            var patch =
                @"{
            tags: {
                location: {
                    region: 'Bangalore',
                    plant: 'MG-Road'
                }
            }
        }";
            // Update Device Twin. tag/metadata
            await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);

        }

        public static async Task DeviceQuery()
        {
            var query = registryManager.CreateQuery(
     "SELECT * FROM devices WHERE tags.location.region = 'Bangalore'", 100);
            Console.WriteLine("Execute Query");
            var twinsInRedmond43 = await query.GetNextAsTwinAsync();

            Console.WriteLine("Devices in Bangalore: {0}",
              string.Join(", ", twinsInRedmond43.Select(t => t.DeviceId)));
        }


            static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #1 - Simulated device. Ctrl-C to exit.\n");
             
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddTagsAndQuery().Wait();

            // DeviceQuery().Wait();

            // Connect to the IoT hub using the MQTT protocol
            TempDevice.s_deviceClient = DeviceClient.CreateFromConnectionString(TempDevice.s_connectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);

            TempDevice.ReportStatus("measurement", "gas", "11");
            TempDevice.ReportStatus("measurement", "temp", "40");
            TempDevice.ReportStatus("battery", "status", "Excellent");
       
            //TempDevice.ReportStatus("battery", "life", "365 days left");

            TempDevice.SendDeviceToCloudMessagesAsync();
            // TempDevice.UploadFile();

            // ClientDevice/IoT Device register the method with Cloud/IoT Hub

            // TempDevice.s_deviceClient.SetMethodHandlerAsync("HelloWorld", TempDevice.HelloWorld, null).Wait();

            // TempDevice.s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", TempDevice.SetTelemetryInterval, null).Wait();

            // Receive messages from cloud
            // TempDevice.ReceiveC2dAsync();
            // TempDevice.UploadFile();
            Console.ReadLine();
        }
    }
}
