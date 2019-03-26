using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace AzureIoTDemo
{
    class TempDevice
    {
        // client driver, send message
        // Method Invocation
        // Upload File to Cloud
        public static DeviceClient s_deviceClient;

        public readonly static string s_connectionString = "HostName=IoTHubKeertee.azure-devices.net;DeviceId=device-2;SharedAccessKey=k+4dj7i3/+U5A7sWBbPDCb8RNIxWIcjn4VwMJc77HjM=";

        public static int interval = 5000;

        // Async method to send simulated telemetry
        public static async void SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                // payload, the actual message
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                Console.WriteLine("Sending " + messageString);

                // put the payload into message (mqtt), transmitted as bytes
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // custom headers, custom properties, not part of the payload
                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                // Send the telemetry message
                // MQTT, HTTP, AMQP
                await s_deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(interval);
            }
        }

    }
}
