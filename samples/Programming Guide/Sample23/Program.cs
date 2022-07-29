namespace Sample23
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.Logger;
    using ifmIoTCore.Logging.Log4Net;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var logger = new Logger(LogLevel.Info);
                var ioTCore = IoTCoreFactory.Create("MyIoTCore", null, logger);

                logger.Info("Informational log");

                ioTCore.Logger.Info("Call logger via IoTCore");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}