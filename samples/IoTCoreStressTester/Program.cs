namespace IoTCoreStressTester
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using CommandLine;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.NetAdapter.Http;
    using ifmIoTCore.Profiles.DeviceManagement;
    using ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests;

    internal class Program
    {
        private static Process _process;
        private static PerformanceCounter _ramCounter;
        private static PerformanceCounter _cpuCounter;

        private static void Main(string[] args)
        {
            _process = Process.GetCurrentProcess();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _ramCounter = new PerformanceCounter("Process", "Working Set", _process.ProcessName);
                _cpuCounter = new PerformanceCounter("Process", "% Processor Time", _process.ProcessName);
            }

            Parser.Default.ParseArguments<CommandLineParameters>(args).WithParsed(o =>
            {
                Run(o.Id, o.HttpUri, o.CacheTimeout, o.PollInterval, o.Mirror1Uri, o.Mirror2Uri, o.Mirror3Uri, o.Mirror4Uri);
            });
        }

        private static void Run(string id, string httpUri, int cacheTimeout, int pollInterval, string mirror1Uri, string mirror2Uri, string mirror3Uri, string mirror4Uri)
        {
            // Do not use logger and only minimal console output, as focus is only on IoTCore performance, CPU and memory usage

            // Create IoTCore
            var ioTCore = IoTCoreFactory.Create(id);

            // Register http client factory and server (required for mirroring IOLM over http)
            ioTCore.RegisterClientNetAdapterFactory(new HttpClientNetAdapterFactory(new ifmIoTCore.Converter.Json.JsonConverter(), true));

            var httpServer = new HttpServerNetAdapter(ioTCore, new Uri(httpUri), new ifmIoTCore.Converter.Json.JsonConverter());
            ioTCore.RegisterServerNetAdapter(httpServer);

            // Optionally start http server
            httpServer.Start();

            // Create device manager
            var deviceManager = new DeviceManagementProfileBuilder(ioTCore);

            // Create a stop watch
            var stopWatch = new Stopwatch();

            // Mirror some IOLMs
            stopWatch.Start();
            if (!string.IsNullOrEmpty(mirror1Uri))
            {
                deviceManager.Mirror(new MirrorRequestServiceData(mirror1Uri, "mirror1", httpUri, cacheTimeout));
            }
            if (!string.IsNullOrEmpty(mirror2Uri))
            {
                deviceManager.Mirror(new MirrorRequestServiceData(mirror2Uri, "mirror2", httpUri, cacheTimeout));
            }
            if (!string.IsNullOrEmpty(mirror3Uri))
            {
                deviceManager.Mirror(new MirrorRequestServiceData(mirror3Uri, "mirror3", httpUri, cacheTimeout));
            }
            if (!string.IsNullOrEmpty(mirror4Uri))
            {
                deviceManager.Mirror(new MirrorRequestServiceData(mirror4Uri, "mirror4", httpUri, cacheTimeout));
            }

            Console.WriteLine($"Mirroring takes {stopWatch.ElapsedMilliseconds} ms");

            // Create list of data addresses to read
            var addresses = ioTCore.Root.GetElementsByProfile("pdin_interpreted").OfType<IDataElement>().Select(x => x.Address).ToList();

            // Poll process data from IOLMs
            Console.WriteLine("Press ESC to cancel");
            stopWatch.Restart();
            while (true)
            {
                // Check keyboard input
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo? keyInfo = Console.ReadKey();
                    if (keyInfo.Value.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }

                // Get data
                var t1 = stopWatch.ElapsedMilliseconds;
                var data = ioTCore.Root.GetDataMulti(new GetDataMultiRequestServiceData(addresses));
                var t2 = stopWatch.ElapsedMilliseconds;
                Console.WriteLine($"Reading {data.Count} data points takes {t2 - t1} ms");

                // Print success and failed reads
                //var successfulRead = 0;
                //foreach (var (key, value) in data)
                //{
                //    if (value.Code == ResponseCodes.Success)
                //    {
                //        successfulRead++;
                //    }
                //    else
                //    {
                //        Console.WriteLine($"Reading {key} failed with code {value.Code}");
                //    }
                //}
                //Console.WriteLine($"Reading {successfulRead} data points succeeded");

                // Print values
                //foreach (var (key, value) in data)
                //{
                //    Console.WriteLine($"{key}={value.Data} ({value.Code})");
                //}

                // Measure CPU and RAM usage with external tool (Performance Monitor, htop) to not pollute measurement with console output

                // Do some console output
                //Console.WriteLine($"Physical memory usage: {_process.WorkingSet64 / 1024 / 1024} MB");
                //Console.WriteLine($"Virtual memory usage: {_process.VirtualMemorySize64 / 1024 / 1024} MB");
                //Console.WriteLine($"Total processor time: {_process.TotalProcessorTime}");
                //if (_ramCounter != null)
                //{
                //    var ram = _ramCounter.NextValue();
                //    Console.WriteLine($"RAM counter: {ram / 1024 / 1024} MB");
                //}
                //if (_cpuCounter != null)
                //{
                //    var cpu = _cpuCounter.NextValue();
                //    Console.WriteLine($"CPU counter: {cpu} %");
                //}

                Thread.Sleep(pollInterval);
            }
        }
    }

    internal class CommandLineParameters
    {
        [Option('i', "id", Required = true, HelpText = "The id of the iotcore", Default = "id0")]
        public string Id { get; set; }

        [Option('h', "http-uri", Required = true, HelpText = "Uri to start the http server", Default = "http://127.0.0.1:8090")]
        public string HttpUri { get; set; }

        [Option("cache-timeout", Required = false, HelpText = "Data cache timeout in ms for mirrored IOLMs", Default = 500)]
        public int CacheTimeout { get; set; }

        [Option("poll-interval", Required = false, HelpText = "The data poll interval", Default = 100)]
        public int PollInterval { get; set; }

        [Option("mirror1-uri", Required = true, HelpText = "The mirror1 uri")]
        public string Mirror1Uri { get; set; }

        [Option("mirror2-uri", Required = false, HelpText = "The mirror2 uri")]
        public string Mirror2Uri { get; set; }

        [Option("mirror3-uri", Required = false, HelpText = "The mirror3 uri")]
        public string Mirror3Uri { get; set; }

        [Option("mirror4-uri", Required = false, HelpText = "The mirror4 uri")]
        public string Mirror4Uri { get; set; }
    }
}
