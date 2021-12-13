using System;
using ifmIoTCore.Exceptions;
using ifmIoTCore.NetAdapter;
using ifmIoTCore.Resources;
using ifmIoTCore.Utilities;

namespace ifmIoTCore.Messages
{
    internal class MessageSender : IMessageSender
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;
        public ILogger Logger { get; }

        public MessageSender(IClientNetAdapterManager clientNetAdapterManager, ILogger logger)
        {
            _clientNetAdapterManager = clientNetAdapterManager;
            Logger = logger;
        }

        public ResponseMessage SendRequest(Uri remoteUri, RequestMessage message, TimeSpan? timeout)
        {
            if (remoteUri == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(remoteUri)));
            }

            if (message == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(message)));
            }

            var client = _clientNetAdapterManager.CreateClientNetAdapter(remoteUri);
            try
            {
                Logger?.Debug($"Send request to '{remoteUri}': code={message.Code}, address={message.Address}, data={message.Data}");
                var response = client.SendRequest(message, timeout);
                Logger?.Debug($"Receive response from '{remoteUri}': code={response.Code}, address={response.Address}, data={response.Data}");
                return response;
            }
            catch (Exception e)
            {
                var errorMessage = string.Format(Resource1.SendRequestFailed, $"{remoteUri}/{message.Address}", e.Message);
                Logger?.Error(errorMessage);
                throw;
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void SendEvent(Uri remoteUri, EventMessage message)
        {
            if (remoteUri == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(remoteUri)));
            }

            if (message == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(message)));
            }

            var client = _clientNetAdapterManager.CreateClientNetAdapter(remoteUri);
            try
            {
                Logger?.Debug($"Send event to '{remoteUri}': code={message.Code}, address={message.Address}, data={message.Data}");
                client.SendEvent(message);
                Logger?.Debug($"Send event to '{remoteUri}' done");
            }
            catch (Exception e)
            {
                var errorMessage = string.Format(Resource1.SendEventFailed, $"{remoteUri}/{message.Address}", e.Message);
                Logger?.Error(errorMessage);
                throw;
            }
            finally
            {
                client.Disconnect();
            }
        }

        
    }
}
