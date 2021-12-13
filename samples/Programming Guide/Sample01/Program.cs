namespace Sample01
{
    using System;
    using ifmIoTCore;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");
                Console.WriteLine(ioTCore.Version);
                Console.WriteLine(ioTCore.Root.Identifier);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}