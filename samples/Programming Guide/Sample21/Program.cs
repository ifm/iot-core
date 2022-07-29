namespace Sample21
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.NetAdapter.Opc;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var opcServer = OpcServerNetAdapter.CreateInstance(ioTCore.Root, 
                    null,
                    ioTCore.DataStore, 
                    ioTCore.Logger);

                opcServer.Uri = new Uri("http://127.0.0.1:62546");
                
                ioTCore.RegisterServerNetAdapter(opcServer);
                
                opcServer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}