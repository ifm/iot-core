## Create Network Adapters

### Create a server network adapter

To make your IoTCore application accessible from another program over a network you must create a network adapter server and register it with the IoTCore, and then start the server. Network adapter servers can support various protocols over various communication interfaces. The standard, what all IoTCores support is JSON over HTTP. When you register the network adapter server with the IoTCore, the IoTCore will take ownership of the object. That means, that the IoTCore is responsible for stopping the server and disposing the object. You can still stop and restart the server as needed, while the IoTCore object is not disposed and implicitly disposed the network adapter server object.

### Create a client network adapter factory

To allow the IoTCore to connect to other applications over a network you must create a network adapter client factory and register it with the IoTCore. This is necessary, for instance, to enable the IoTCore to send events to another application or to mirror another IoTCore into its own element tree. When the IoTCore needs to connect to another application it uses the network adapter client factory to create a network adapter client for the required communication channel and protocol. With this client it connects to the remote application. When you register the network adapter client factory with the IoTCore, the IoTCore will take ownership of the object. That means, that the IoTCore is responsible for disposing the object.

### Create a Http Server Network Adapter

The IoTCore package provides an HTTP Server network adapter and JSON message converter. To add a server to your application create a new instance and register it with the IoTCore.

```
namespace Sample19;

using System;
using ifm.IoTCore.Factory;
using ifm.IoTCore.MessageConverter.Json;
using ifm.IoTCore.NetAdapter.Http;
using ifm.Logger;

internal class Program
{
    private static void Main()
    {
        var ioTCore = IoTCoreFactory.Create("MyIoTCore", new NullLogger());

        HttpServerNetAdapter httpServer = null;
        try
        {
            httpServer = new HttpServerNetAdapter(ioTCore.MessageHandler, 
                new Uri("http://127.0.0.1:8000"),
                new MessageConverter());

            ioTCore.ServerNetAdapterManager.RegisterServerNetAdapter(httpServer);

            httpServer.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        Console.ReadLine();
        httpServer?.Stop();
        ioTCore.Dispose();
    }
}
```

### Create a Http network adapter client factory

The IoTCore package provides an Http Client network adapter factory and a JSON message converter. To add this factory to your application create a new instance and register it with the IoTCore.

```
namespace Sample20;

using System;
using ifm.IoTCore.Factory;
using ifm.IoTCore.MessageConverter.Json;
using ifm.IoTCore.NetAdapter.Http;
using ifm.Logger;

internal class Program
{
    static void Main()
    {
        var ioTCore = IoTCoreFactory.Create("MyIoTCore", new NullLogger());
        try
        {
            ioTCore.ClientNetAdapterManager.RegisterClientNetAdapterFactory(
                new HttpClientNetAdapterFactory(new MessageConverter()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        Console.ReadLine();
        ioTCore.Dispose();
    }
}
```

### Create a Https server network adapter 

The IoTCore package provides an HTTPs Server network adapter and JSON message converter. To add a https server to your application create a new instance and register it with the IoTCore.

```
namespace Sample38;

using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ifm.IoTCore.Factory;
using ifm.Logger;
using ifm.Logger.Contracts;
using ifm.IoTCore.MessageConverter.Json;
using ifm.IoTCore.NetAdapter.Http.NetCoreApp;


class Program
{
    static void Main(string[] args)
    {
        var ioTCore = IoTCoreFactory.Create("test", new Log4NetLogger(LogLevel.Warning));

        var myServerCertificate = new X509Certificate2("mycert.pfx");

        var httpsServerNetAdapter = new HttpsServerNetAdapter(
            ioTCore.MessageHandler,
            new Uri("https://127.0.0.1:8090"),
            new MessageConverter(),
            ioTCore.Logger,myServerCertificate,
            (certificate, chain, sslPolicyErrors) =>
            {
                return sslPolicyErrors == SslPolicyErrors.None;
            });

        ioTCore.ServerNetAdapterManager.RegisterServerNetAdapter(httpsServerNetAdapter);

        httpsServerNetAdapter.Start();

        using (var manualResetEvent = new ManualResetEventSlim()) {

            ioTCore.ElementManager.CreateActionServiceElement(ioTCore.Root, 
                "stop",
                (element, i) => { manualResetEvent.Set(); }
            );
            manualResetEvent.Wait();
        }

        httpsServerNetAdapter.Stop();
    }
}
```

Note: On windows it might be necessary to allow opening ports that use http. The command for that is:

```
netsh http add urlacl url=http://127.0.0.1:8090/ user=myuser listen=yes
```

### Create a Https client network adapter factory

You can create a factory for https clients.

```
namespace Sample39;

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ifm.IoTCore.Factory;
using ifm.Logger.Contracts;
using ifm.IoTCore.MessageConverter.Json;
using ifm.IoTCore.NetAdapter.Http;
using ifm.Logger;

class Program
{
    static void Main(string[] args)
    {
        var ioTCore = IoTCoreFactory.Create("test", new Log4NetLogger(LogLevel.Warning));

        var myServerCertificate = new X509Certificate2("mycert.pfx");

        var httpsClientNetAdapterFactory = new HttpsClientNetAdapterFactory(new MessageConverter(),
            (httpRequestMessage, certificate, chain, sslPolicyErrors) =>
            {
                return sslPolicyErrors == SslPolicyErrors.None;
            },
            () =>
            {
                return new[] { new X509Certificate2("myClientCert.pfx") };
            });

        using (var manualResetEvent = new ManualResetEventSlim()) {

            ioTCore.ElementManager.CreateActionServiceElement(ioTCore.Root, 
                "stop",
                (element, i) => { manualResetEvent.Set(); }
            );
            manualResetEvent.Wait();
        }
    }
}
```