namespace Sample23
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.Logging.Log4Net;
    using ifmIoTCore.Utilities;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var logger = new Logger(LogLevel.Info);
                var ioTCore = IoTCoreFactory.Create("MyIoTCore", 
                    logger);

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