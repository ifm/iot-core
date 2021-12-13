namespace Sample20
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.NetAdapter.Http;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                ioTCore.RegisterClientNetAdapterFactory(
                    new HttpClientNetAdapterFactory(
                        new ifmIoTCore.Converter.Json.JsonConverter()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}