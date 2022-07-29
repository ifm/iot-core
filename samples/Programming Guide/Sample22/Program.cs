namespace Sample22
{
    using System;
    using ifm.CiA301.Can.Adapter;
    using ifmIoTCore;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.Messages;
    using ifmIoTCore.NetAdapter.CanOpen;
    using ifmIoTCore.NetAdapter.CanOpen.UriBuilders;

    internal class Program
    {
        private static void Main()
        {
            try
            {
                // Create a base iotcore instance.
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var uriAdapter1 = new EcomatControllerCanOpenUriBuilder(Vendor.Sontheim.ToString(), 
                    "canfox(1) ch01", 
                    125, 
                    127, 
                    1200, 
                    "Optimal");

                // Create a CanOpen server NetAdapter a client can access to.
                var serverNetAdapter = new CanOpenServerNetAdapter(ioTCore, 
                    uriAdapter1, 
                    new MessageConverter());

                // Register the NetAdapterServer on the current IoTCore and run the server.
                ioTCore.RegisterServerNetAdapter(serverNetAdapter);
                var _ = serverNetAdapter.StartAsync();

                // Create and register a CanOpenNetAdapterClientFactory. Not needed for this sample.
                // This is needed if you do mirroring. Than the IoTCore can create clients dynamically.
                //ioTCore.RegisterClientNetAdapterFactory(new CanOpenNetAdapterClientFactory(new JsonConverter()));

                // We need a specific uri. This uri contains all parameter needed to setup a connection.
                var uriAdapter2 = new EcomatControllerCanOpenUriBuilder(Vendor.Sontheim.ToString(), 
                    "canfox(2) ch01", 
                    125, 
                    127, 
                    1200, 
                    "Optimal");

                // Create the NetAdapterClient to have direct access to a NetAdapterServer.
                var clientNetAdapter = new CanOpenClientNetAdapter(uriAdapter2, 
                    new MessageConverter());

                // Create a /gettree request and send the request.
                var request = new Message(RequestCodes.Request, 0, "/gettree", null);
                var response = clientNetAdapter.SendRequest(request, 
                    new TimeSpan(0,0,5));

                // Print the response.
                Console.WriteLine(response.Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}