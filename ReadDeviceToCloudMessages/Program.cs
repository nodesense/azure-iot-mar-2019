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

            ReadTempService.Start();
             
            Console.ReadLine();
        }
    }
}
