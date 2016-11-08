using System;

namespace ForensicIOS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var iex = new DeviceExtractor();
            iex.Start();
            Console.ReadLine();
        }
    }
}