namespace Sample19
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.NetAdapter.Http;

    internal class Program
    {
        private static void Main()
        {
            IIoTCore ioTCore = null;
            HttpServerNetAdapter httpServer = null;
            try
            {
                ioTCore = IoTCoreFactory.Create("MyIoTCore");

                httpServer = new HttpServerNetAdapter(ioTCore,
                    new Uri("http://127.0.0.1:8001"),
                    new MessageConverter());

                ioTCore.RegisterServerNetAdapter(httpServer);

                httpServer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            httpServer?.Stop();
            ioTCore?.Dispose();
        }
    }
}