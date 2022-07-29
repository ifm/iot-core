namespace ifmIoTCore.NetAdapter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ServerNetAdapterManager : IServerNetAdapterManager
    {
        private readonly List<IServerNetAdapter> _serverNetAdapters = new List<IServerNetAdapter>();

        public IEnumerable<IServerNetAdapter> ServerNetAdapters
        {
            get
            {
                lock (_serverNetAdapters)
                {
                    return new List<IServerNetAdapter>(_serverNetAdapters);
                }
            }
        }

        public void RegisterServerNetAdapter(IServerNetAdapter serverNetAdapter)
        {
            if (serverNetAdapter == null) throw new ArgumentNullException(nameof(serverNetAdapter));

            lock (_serverNetAdapters)
            {
                _serverNetAdapters.Add(serverNetAdapter);
            }
        }

        public void RemoveServerNetAdapter(IServerNetAdapter serverNetAdapter)
        {
            if (serverNetAdapter == null) throw new ArgumentNullException(nameof(serverNetAdapter));

            lock (_serverNetAdapters)
            {
                _serverNetAdapters.Remove(serverNetAdapter);
                serverNetAdapter.Stop();
                serverNetAdapter.Dispose();
            }
        }

        public IServerNetAdapter FindServerNetAdapter(Uri uri)
        {
            lock (_serverNetAdapters)
            {
                return _serverNetAdapters.FirstOrDefault(x => x.Uri == uri);
            }
        }

        public IEnumerable<IServerNetAdapter> FindServerNetAdapters(string scheme)
        {
            lock (_serverNetAdapters)
            {
                return _serverNetAdapters.Where(x => x.Scheme == scheme);
            }
        }

        public void Dispose()
        {
            lock (_serverNetAdapters)
            {
                foreach (var item in _serverNetAdapters)
                {
                    item.Stop();
                    item.Dispose();
                }
                _serverNetAdapters.Clear();
            }
        }
    }
}