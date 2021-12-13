namespace Sample27
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.NetAdapter.Http;
    using ifmIoTCore.Utilities;
    using Newtonsoft.Json;

    public class MyRequestServiceData
    {
        [JsonProperty("in", Required = Required.Always)]
        public readonly string Input;

        public MyRequestServiceData(string input)
        {
            Input = input;
        }
    }

    public class MyResponseServiceData
    {
        [JsonProperty("out", Required = Required.Always)]
        public readonly string Output;

        [JsonProperty("length", Required = Required.Always)]
        public readonly int Length;

        public MyResponseServiceData(string output, int length)
        {
            Output = output;
            Length = length;
        }
    }

    public class MyProfile
    {
        private readonly IIoTCore _ioTCore;

        public string Name = "MyProfile";

        public MyProfile(IIoTCore ioTCore)
        {
            _ioTCore = ioTCore;
        }

        public void Build()
        {
            var structureElement = _ioTCore.CreateStructureElement(_ioTCore.Root, 
                "my_profile", 
                profiles: new List<string> {"my_profile"} );
            _ioTCore.CreateServiceElement<MyRequestServiceData, MyResponseServiceData>(structureElement, 
                "do_it", 
                DoItFunc);
        }

        private MyResponseServiceData DoItFunc(IBaseElement element, 
            MyRequestServiceData data, 
            int? cid = null)
        {
            return DoIt(data);
        }

        // Make service available on API
        public MyResponseServiceData DoIt(MyRequestServiceData data)
        {
            return new MyResponseServiceData(data.Input, data.Input.Length);
        }

        public void Dispose()
        {
        }
    }

    class Program
    {
        static void Main()
        {
            var ioTCore = IoTCoreFactory.Create("MyIoTCore");

            var myProfile = new MyProfile(ioTCore);
            myProfile.Build();

            // Call service via API
            var response = myProfile.DoIt(new MyRequestServiceData("The little red rooster"));
            Console.WriteLine(Helpers.ToJson(response));

            // Call service via element
            var element = (IServiceElement)ioTCore.Root.GetElementByIdentifier("do_it");
            var json = element.Invoke(Helpers.ToJson(new MyRequestServiceData("The little red rooster")), 
                null);
            Console.WriteLine(json);

            element = (IServiceElement)ioTCore.Root.GetElementByAddress("/my_profile/do_it");
            json = element.Invoke(Helpers.ToJson(new MyRequestServiceData("The little red rooster")), 
                null);
            Console.WriteLine(json);

            // Make service accessible from remote under /my_profile/do_it
            var httpServer = new HttpServerNetAdapter(ioTCore, 
                new Uri("http://127.0.0.1:8101"), 
                new ifmIoTCore.Converter.Json.JsonConverter());

            httpServer.Start();

            Console.ReadLine();
        }
    }
}