namespace Sample19
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.NetAdapter.Http;

    internal class Program
    {
        static void Main()
        {
            HttpServerNetAdapter httpServer = null;
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                httpServer = new HttpServerNetAdapter(ioTCore, 
                    new Uri("http://127.0.0.1:8001"),
                    new ifmIoTCore.Converter.Json.JsonConverter());

                ioTCore.RegisterServerNetAdapter(httpServer);
                
                httpServer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            httpServer?.Stop();
        }
    }
}