namespace ifmIoTCore.NetAdapter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Messages;
    using Resources;

    public class ClientNetAdapterManager : IClientNetAdapterManager
    {
        private readonly List<IClientNetAdapterFactory> _clientNetAdapterFactories = new List<IClientNetAdapterFactory>();

        public IEnumerable<IClientNetAdapterFactory> ClientNetAdapterFactories
        {
            get
            {
                lock (_clientNetAdapterFactories)
                {
                    return new List<IClientNetAdapterFactory>(_clientNetAdapterFactories);
                }
            }
        }

        public void RegisterClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory)
        {
            if (clientNetAdapterFactory == null) throw new ArgumentNullException(nameof(clientNetAdapterFactory));

            lock (_clientNetAdapterFactories)
            {
                if (_clientNetAdapterFactories.FirstOrDefault(x => x.Scheme == clientNetAdapterFactory.Scheme) == null)
                {
                    _clientNetAdapterFactories.Add(clientNetAdapterFactory);
                }
            }
        }

        public void RemoveClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory)
        {
            if (clientNetAdapterFactory == null) throw new ArgumentNullException(nameof(clientNetAdapterFactory));

            lock (_clientNetAdapterFactories)
            {
                _clientNetAdapterFactories.Remove(clientNetAdapterFactory);
                clientNetAdapterFactory.Dispose();
            }
        }

        public IClientNetAdapter CreateClientNetAdapter(Uri targetUri)
        {
            if (targetUri == null) throw new ArgumentNullException(nameof(targetUri));

            lock (_clientNetAdapterFactories)
            {
                var clientFactory = _clientNetAdapterFactories.FirstOrDefault(x => x.Scheme == targetUri.Scheme);
                if (clientFactory == null)
                {
                    throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ClientFactoryNotFound, targetUri.Scheme));
                }
                return clientFactory.CreateClient(targetUri);
            }
        }

        public void Dispose()
        {
            lock (_clientNetAdapterFactories)
            {
                foreach (var item in _clientNetAdapterFactories)
                {
                    item.Dispose();
                }
                _clientNetAdapterFactories.Clear();
            }
        }
    }
}
