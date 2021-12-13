namespace ifmIoTCore.NetAdapter
{
    using System;
    using System.Threading.Tasks;

    public abstract class ServerNetAdapterBase : IServerNetAdapter
    {
        public abstract string ConverterType { get; }
        public abstract Uri Uri { get; }

        public virtual void Dispose()
        {
        }

        public abstract void Start();
        public abstract Task StartAsync();
        public abstract void Stop();
        public abstract Task StopAsync();
    }
}