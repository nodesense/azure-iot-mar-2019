// Program.cs
using Microsoft.Azure.Devices.Client;
using System;

namespace AzureIoTDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #1 - Simulated device. Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            TempDevice.s_deviceClient = DeviceClient.CreateFromConnectionString(TempDevice.s_connectionString, TransportType.Mqtt);
            TempDevice.SendDeviceToCloudMessagesAsync();
            // TempDevice.UploadFile();

            // ClientDevice/IoT Device register the method with Cloud/IoT Hub

            TempDevice.s_deviceClient.SetMethodHandlerAsync("HelloWorld", TempDevice.HelloWorld, null).Wait();

            TempDevice.s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", TempDevice.SetTelemetryInterval, null).Wait();

            // Receive messages from cloud
            TempDevice.ReceiveC2dAsync();
            Console.ReadLine();
        }
    }
}
