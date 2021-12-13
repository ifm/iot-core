using System;
using System.Collections.Generic;
using System.Linq;
using ifmIoTCore.Exceptions;
using ifmIoTCore.Messages;
using ifmIoTCore.Resources;

namespace ifmIoTCore.NetAdapter
{
    public class ClientNetAdapterManager : IClientNetAdapterManager
    {
        private readonly List<IClientNetAdapterFactory> _clientNetAdapterFactories = new List<IClientNetAdapterFactory>();
        public IEnumerable<IClientNetAdapterFactory> ClientNetAdapterFactories => _clientNetAdapterFactories;

        /// <summary>
        /// Lock object to synchronize <seealso cref="CreateClientNetAdapter"/> calls.
        /// </summary>
        /// <remarks>lock(this) is not used, because this here is the IoTCore instance. When locking on the iotcore occures in other places, this could lead to a deadlock.</remarks>
        private readonly object _netAdapterManagerLock = new object(); 

        public void RegisterClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory)
        {
            if (clientNetAdapterFactory == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(clientNetAdapterFactory)));
            }

            if (_clientNetAdapterFactories.FirstOrDefault(x => x.Protocol == clientNetAdapterFactory.Protocol) == null)
            {
                _clientNetAdapterFactories.Add(clientNetAdapterFactory);
            }
        }

        public void RemoveClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory)
        {
            clientNetAdapterFactory?.Dispose();
            _clientNetAdapterFactories.Remove(clientNetAdapterFactory);
        }

        public IClientNetAdapter CreateClientNetAdapter(Uri remoteUri)
        {
            if (remoteUri == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(remoteUri)));
            }

            lock(_netAdapterManagerLock)
            {
                var clientFactory = _clientNetAdapterFactories.FirstOrDefault(x => x.Protocol == remoteUri.Scheme);
                if (clientFactory == null)
                {
                    throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ClientFactoryNotFound, remoteUri.Scheme));
                }
                return clientFactory.CreateClient(remoteUri);
            }
        }

        public void Dispose()
        {
            if (_clientNetAdapterFactories != null)
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
