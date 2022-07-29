namespace ifmIoTCore.IoTCore
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using DataStore;
    using Elements;
    using Logger;
    using Messages;
    using NetAdapter;
    using Resources;

    internal class IoTCore : IIoTCore
    {
        public string Identifier { get; }

        public string Version => typeof(IoTCore).Assembly.GetName().Version.ToString(3);

        public IElementManager ElementManager { get; }

        public IMessageHandler MessageHandler { get; }

        public IClientNetAdapterManager ClientNetAdapterManager { get; }

        public IServerNetAdapterManager ServerNetAdapterManager { get; }

        public IDataStore DataStore { get; }

        public ILogger Logger { get; }

        public IoTCore(string identifier,
            IElementManager elementManager,
            IMessageHandler messageHandler,
            IClientNetAdapterManager clientNetAdapterManager,
            IServerNetAdapterManager serverNetAdapterManager,
            IDataStore dataStore,
            ILogger logger)
        {
            Identifier = identifier;
            ElementManager = elementManager;
            MessageHandler = messageHandler;
            ClientNetAdapterManager = clientNetAdapterManager;
            ServerNetAdapterManager = serverNetAdapterManager;
            DataStore = dataStore;
            Logger = logger;

            Logger?.Info(string.Format(Resource1.IoTCoreStarted, Version));

            ElementManager.Root = new RootDeviceElement(identifier, Version, ElementManager, ServerNetAdapterManager);
        }

        public void Dispose()
        {
            ServerNetAdapterManager.Dispose();
            ClientNetAdapterManager.Dispose();
        }

        #region IElementManager

        ReaderWriterLockSlim IElementManager.Lock => ElementManager.Lock;

        IDeviceElement IElementManager.Root
        {
            get => ElementManager.Root;
            set => ElementManager.Root = value;
        }

        public IBaseElement GetElementByAddress(string address)
        {
            return ElementManager.GetElementByAddress(address);
        }

        public IEnumerable<IBaseElement> GetElementsByProfile(string profile)
        {
            return ElementManager.GetElementsByProfile(profile);
        }

        #endregion

        #region IMessageHandler

        public event EventHandler<RequestMessageEventArgs> RequestMessageReceived
        {
            add => MessageHandler.RequestMessageReceived += value;
            remove => MessageHandler.RequestMessageReceived -= value;
        }

        public event EventHandler<EventMessageEventArgs> EventMessageReceived
        {
            add => MessageHandler.EventMessageReceived += value;
            remove => MessageHandler.EventMessageReceived -= value;
        }

        public event EventHandler<RequestMessageEventArgs> RequestMessageResponded
        {
            add => MessageHandler.RequestMessageResponded += value;
            remove => MessageHandler.RequestMessageResponded -= value;
        }

        public Message HandleRequest(Message message)
        {
            return MessageHandler.HandleRequest(message);
        }

        public void HandleEvent(Message message)
        {
            MessageHandler.HandleEvent(message);
        }

        #endregion

        #region IClientNetAdapterManager

        public IEnumerable<IClientNetAdapterFactory> ClientNetAdapterFactories => ClientNetAdapterManager.ClientNetAdapterFactories;

        public void RegisterClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory)
        {
            ClientNetAdapterManager.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
        }

        public void RemoveClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory)
        {
            ClientNetAdapterManager.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
        }

        public IClientNetAdapter CreateClientNetAdapter(Uri remoteUri)
        {
            return ClientNetAdapterManager.CreateClientNetAdapter(remoteUri);
        }

        #endregion

        #region IServerNetAdapterManager

        public IEnumerable<IServerNetAdapter> ServerNetAdapters => ServerNetAdapterManager.ServerNetAdapters;

        public void RegisterServerNetAdapter(IServerNetAdapter serverNetAdapter)
        {
            ServerNetAdapterManager.RegisterServerNetAdapter(serverNetAdapter);
        }

        public void RemoveServerNetAdapter(IServerNetAdapter serverNetAdapter)
        {
            ServerNetAdapterManager.RemoveServerNetAdapter(serverNetAdapter);
        }

        public IServerNetAdapter FindServerNetAdapter(Uri uri)
        {
            return ServerNetAdapterManager.FindServerNetAdapter(uri);
        }

        public IEnumerable<IServerNetAdapter> FindServerNetAdapters(string scheme)
        {
            return ServerNetAdapterManager.FindServerNetAdapters(scheme);
        }

        #endregion
    }
}