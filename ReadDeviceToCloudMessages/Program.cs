// Proj: ReadDevicetoCloudMessages
// Program.cs 
using System;

namespace ReadDeviceToCloudMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // ReadTempService.Start();

            // Consumer data from another event hub
            ReadAlertTempService.Start();
             
            Console.ReadLine();
        }
    }
}
