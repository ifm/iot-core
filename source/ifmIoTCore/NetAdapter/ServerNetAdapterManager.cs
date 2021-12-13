using System;
using System.Collections.Generic;
using System.Linq;
using ifmIoTCore.Exceptions;
using ifmIoTCore.Messages;
using ifmIoTCore.Resources;

namespace ifmIoTCore.NetAdapter
{
    internal class ServerNetAdapterManager : IServerNetAdapterManager
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;

        public ServerNetAdapterManager(IClientNetAdapterManager clientNetAdapterManager)
        {
            _clientNetAdapterManager = clientNetAdapterManager;
        }

        private readonly List<IServerNetAdapter> _serverNetAdapters = new List<IServerNetAdapter>();
        public IEnumerable<IServerNetAdapter> ServerNetAdapters => _serverNetAdapters;

        public void RegisterServerNetAdapter(IServerNetAdapter netAdapterServer)
        {
            if (netAdapterServer == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(netAdapterServer)));
            }

            _serverNetAdapters.Add(netAdapterServer);
        }

        public void RemoveServerNetAdapter(IServerNetAdapter netAdapterServer)
        {
            _serverNetAdapters.Remove(netAdapterServer);
        }

        public IServerNetAdapter FindServerNetAdapter(Uri uri)
        {
            return _serverNetAdapters.FirstOrDefault(x => x.Uri == uri);
        }

        public IServerNetAdapter FindReverseServerNetAdapter(Uri remoteUri)
        {
            Uri localUri;
            var client = _clientNetAdapterManager.CreateClientNetAdapter(remoteUri);
            try
            {
                localUri = client.GetLocalUri();
            }
            finally
            {
                client.Disconnect();
            }
            return _serverNetAdapters.FirstOrDefault(x => x.Uri.Scheme == localUri.Scheme && x.Uri.Host == localUri.Host);
        }

        public void Dispose()
        {
            if (_serverNetAdapters != null)
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