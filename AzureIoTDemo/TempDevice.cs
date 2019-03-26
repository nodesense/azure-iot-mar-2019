// TempDevice.cs
using System;

using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.Devices.Shared;

namespace AzureIoTDemo
{
    class TempDevice
    {
        // client driver, send message
        // Method Invocation
        // Upload File to Cloud
        public static DeviceClient s_deviceClient;

 
        public readonly static string s_connectionString = "{{{{connection-string}}";
        public static int interval = 10000;

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
                await s_deviceClient.SendEventAsync(message); //fails if event hub out of service
                // Produer/Sender/Device, on failure, should store  the messages in local storage
                // and Retry
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(interval);
            }
        }

        // Device method, to be invoked by cloud

            // {"msg": "greeting"}
        public static Task<MethodResponse> HelloWorld(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);

            Console.WriteLine("Received method called " + data);

            data = data.Replace("\"", "\\\""); // " replace with \"

            string result = "{\"result\":\"Got your message: " + data + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
         }


       // invoke SetTelemetryInterval
       // payload could be 1000 or 2000 values in ms
        // mobile app, send message to cloud to invoke this method
           public static Task<MethodResponse> SetTelemetryInterval(MethodRequest methodRequest, object userContext)
            {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            Console.WriteLine("Received method called " + data);
            // Check the payload is a single integer value
            if (Int32.TryParse(data, out interval))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Telemetry interval set to {0} seconds", data);
                Console.ResetColor();

                // Acknowlege the direct method call with a 200 success message
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                // Acknowlege the direct method call with a 400 error message
                string result = "{\"result\":\"Invalid parameter\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
        }


        // Receive Message From cloud
        public static async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                // Pull message
                Message receivedMessage = await s_deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}",
                Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await s_deviceClient.CompleteAsync(receivedMessage);
            }
        }


        public static async Task UploadFile()
        {
            var fileStreamSource = new FileStream("C:\\Users\\krish\\AzureIoTDemo\\logs.csv", FileMode.Open);
            var fileName = Path.GetFileName(fileStreamSource.Name);

            Console.WriteLine("Uploading File: {0}", fileName);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            await s_deviceClient.UploadToBlobAsync(fileName, fileStreamSource);
            watch.Stop();

            Console.WriteLine("Time to upload file: {0}ms\n", watch.ElapsedMilliseconds);
        }


        // Update the device twin
        // Edge device to cloud
        // Reported properties
        public static async void ReportStatus(String parentKey, String key, String value)
        {
            try
            {
                TwinCollection reportedProperties;
                reportedProperties = new TwinCollection();
                TwinCollection property = new TwinCollection();
                Console.WriteLine("Updating " + parentKey + ":" + key + ":" + value);
                property[key] = value;
                reportedProperties[parentKey] = property;
                // call to cloud
                await s_deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
            }catch(Exception e)
            {
                Console.WriteLine("Error in twin {0}", e.Message);
            }
            }

    }
}
