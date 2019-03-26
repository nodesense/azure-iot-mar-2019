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
            Console.ReadLine();
        }
    }
}
