namespace Sample27
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Common.Variant;
    using ifmIoTCore.Elements;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.NetAdapter.Http;


    public class MyRequestServiceData
    {
        [VariantPropertyAttribute("in", Required = true)]
        public readonly string Input;

        public MyRequestServiceData(string input)
        {
            Input = input;
        }
    }

    public class MyResponseServiceData
    {
        [VariantPropertyAttribute("out", Required = true)]
        public readonly string Output;

        [VariantPropertyAttribute("length", Required = true)]
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
            var structureElement = new StructureElement("my_profile", 
                profiles: new List<string> {"my_profile"} );

            _ioTCore.Root.AddChild(structureElement);

            var serviceElement = new ServiceElement(
                "do_it",
                DoItFunc);

            structureElement.AddChild(serviceElement);
        }

        private Variant DoItFunc(IBaseElement element, Variant data, int? cid = null)
        {
            var myRequestServiceData = Variant.ToObject<MyRequestServiceData>(data);
            var result = DoIt(myRequestServiceData);

            return Variant.FromObject(result);
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
            Console.WriteLine(Variant.FromObject(response));

            // Call service via element
            var element = (IServiceElement)ioTCore.Root.GetElementByIdentifier("do_it");
            var json = element.Invoke(Variant.FromObject(new MyRequestServiceData("The little red rooster")), 
                null);
            Console.WriteLine(json);

            element = (IServiceElement)ioTCore.Root.GetElementByAddress("/my_profile/do_it");
            json = element.Invoke(Variant.FromObject(new MyRequestServiceData("The little red rooster")), 
                null);
            Console.WriteLine(json);

            // Make service accessible from remote under /my_profile/do_it
            var httpServer = new HttpServerNetAdapter(ioTCore, 
                new Uri("http://127.0.0.1:8101"), 
                new MessageConverter());

            httpServer.Start();

            Console.ReadLine();
        }
    }
}