namespace Sample29
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.NetAdapter.Mqtt;

    class Program
    {
        static void Main()
        {
            try
            {
                var iotCore = IoTCoreFactory.Create("test");
                iotCore.RegisterClientNetAdapterFactory(
                    new MqttNetAdapterClientFactory(
                        new MessageConverter()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}