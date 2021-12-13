namespace Sample14
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Utilities;

    internal class Program
    {
        internal class UserData
        {
            public string String1;
            public int Int1;
            public float Float1;

            public UserData()
            {
                String1 = "Hallo";
                Int1 = 10;
                Float1 = 1.2345f;
            }
        }

        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, 
                    "struct1", 
                    null,
                    new List<string> { "profile1" });

                var service1 = ioTCore.CreateActionServiceElement(struct1, 
                    "service1", 
                    HandleService1);
                service1.UserData = new UserData();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        private static void HandleService1(IBaseElement element, int? cid)
        {
            var userData = (UserData)element.UserData;

            // Do something with user data
        }
    }
}