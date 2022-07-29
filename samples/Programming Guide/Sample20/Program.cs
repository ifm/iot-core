namespace Sample20
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.NetAdapter.Http;

    internal class Program
    {
        static void Main()
        {
            IIoTCore ioTCore = null;
            try
            {
                ioTCore = IoTCoreFactory.Create("MyIoTCore");

                ioTCore.RegisterClientNetAdapterFactory(
                    new HttpClientNetAdapterFactory(
                        new MessageConverter()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            ioTCore?.Dispose();
        }
    }
}