namespace ifmIoTCore
{
    using System.Threading;
    using ifmIoTCore.Elements.EventArguments;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Messages;
    using ifmIoTCore.NetAdapter;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Elements;
    using Elements.ServiceData.Responses;
    using Persistence;
    using Resources;
    using Utilities;

    internal class IoTCore : IIoTCore
    {
        public string Identifier { get; }
        public string Version => typeof(IoTCore).Assembly.GetName().Version.ToString(3);
        public IDeviceElement Root { get; private set; }
        public IPersistenceManager PersistenceManager { get; set; }
        public IDataStore DataStore { get; }
        public IElementManager ElementManager { get; }
        public IServerNetAdapterManager ServerNetAdapterManager { get; }
        public ClientNetAdapterManager ClientNetAdapterManager { get; }
        public IMessageHandler MessageHandler { get; }
        public IMessageSender MessageSender { get; }
        public ILogger Logger { get; }

        public IoTCore(string identifier,
            ElementManager elementManager,
            ServerNetAdapterManager serverNetAdapterManager,
            ClientNetAdapterManager clientNetAdapterManager,
            IMessageHandler messageHandler,
            MessageSender messageSender,
            ILogger logger)
        {

            Identifier = identifier;
            ElementManager = elementManager;
            ServerNetAdapterManager = serverNetAdapterManager;
            this.ClientNetAdapterManager = clientNetAdapterManager;
            MessageHandler = messageHandler;
            this.MessageSender = messageSender;
            this.Logger = logger;

            Logger?.Info(string.Format(Resource1.IoTCoreStarted, Version));

            Root = elementManager.CreateRootDeviceElement(identifier, () =>
                {
                    return serverNetAdapterManager.ServerNetAdapters.Select(x => new GetIdentityResponseServiceData.ServerInfo(x.Uri.Scheme, x.Uri.ToString(), new List<string> { x.ConverterType })).ToList();
                },
                () => typeof(IoTCore).Assembly.GetName().Version.ToString(3));

            DataStore = new JsonDataStore(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ifm", "IoTCore", "DataStore.json"));
        }

        public void Dispose()
        {
            Root.Dispose();
            Root = null;
        }

        public ReaderWriterLockSlim Lock => ElementManager.Lock;

        public IDeviceElement CreateDeviceElement(IBaseElement parentElement, string identifier, Func<IDeviceElement, GetIdentityResponseServiceData> getIdentityFunc,
            Func<IDeviceElement, GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeFunc, Func<IDeviceElement, QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeFunc, Func<IDeviceElement, GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiFunc, Action<IDeviceElement, SetDataMultiRequestServiceData> setDataMultiFunc, Func<IDeviceElement, GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListFunc,
            Format format = null, List<string> profiles = null, string uid = null, bool isHidden = false, object context = null,
            bool raiseTreeChanged = false)
        {
            return ElementManager.CreateDeviceElement(parentElement, identifier, getIdentityFunc, getTreeFunc, queryTreeFunc, getDataMultiFunc, setDataMultiFunc, getSubscriberListFunc, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public IStructureElement CreateStructureElement(IBaseElement parentElement, string identifier, Format format = null,
            List<string> profiles = null, string uid = null, bool isHidden = false, object context = null,
            bool raiseTreeChanged = false)
        {
            return ElementManager.CreateStructureElement(parentElement, identifier, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public IActionServiceElement CreateActionServiceElement(IBaseElement parentElement, string identifier, Action<IActionServiceElement, int?> func,
            Format format = null, List<string> profiles = null, string uid = null, bool isHidden = false, object context = null,
            bool raiseTreeChanged = false)
        {
            return ElementManager.CreateActionServiceElement(parentElement, identifier, func, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public IGetterServiceElement<TOut> CreateGetterServiceElement<TOut>(IBaseElement parentElement, string identifier, Func<IGetterServiceElement<TOut>, int?, TOut> func,
            Format format = null, List<string> profiles = null, string uid = null, bool isHidden = false, object context = null,
            bool raiseTreeChanged = false)
        {
            return ElementManager.CreateGetterServiceElement(parentElement, identifier, func, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public ISetterServiceElement<TIn> CreateSetterServiceElement<TIn>(IBaseElement parentElement, string identifier, Action<ISetterServiceElement<TIn>, TIn, int?> func,
            Format format = null, List<string> profiles = null, string uid = null, bool isHidden = false, object context = null,
            bool raiseTreeChanged = false)
        {
            return ElementManager.CreateSetterServiceElement(parentElement, identifier, func, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public IServiceElement<TIn, TOut> CreateServiceElement<TIn, TOut>(IBaseElement parentElement, string identifier, Func<IServiceElement<TIn, TOut>, TIn, int?, TOut> func,
            Format format = null, List<string> profiles = null, string uid = null, bool isHidden = false, object context = null,
            bool raiseTreeChanged = false)
        {
            return ElementManager.CreateServiceElement(parentElement, identifier, func, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public IEventElement CreateEventElement(IBaseElement parentElement, string identifier, Action<IEventElement, SubscribeRequestServiceData, int?> preSubscribeFunc = null,
            Action<IEventElement, UnsubscribeRequestServiceData, int?> postUnsubscribeFunc = null, Format format = null, List<string> profiles = null, string uid = null,
            bool isHidden = false, object context = null, bool raiseTreeChanged = false)
        {
            return ElementManager.CreateEventElement(parentElement, identifier, preSubscribeFunc, postUnsubscribeFunc, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public IDataElement<T> CreateDataElement<T>(IBaseElement parentElement, string identifier, Func<IDataElement<T>, T> getDataFunc = null,
            Action<IDataElement<T>, T> setDataFunc = null, bool createGetDataServiceElement = true, bool createSetDataServiceElement = true,
            T value = default, TimeSpan? cacheTimeout = null, Format format = null, List<string> profiles = null, string uid = null,
            bool isHidden = false, object context = null, bool raiseTreeChanged = false)
        {
            return ElementManager.CreateDataElement(parentElement, identifier, getDataFunc, setDataFunc, createGetDataServiceElement, createSetDataServiceElement, value, cacheTimeout, format, profiles, uid, isHidden, context, raiseTreeChanged);
        }

        public void RemoveElement(IBaseElement parentElement, IBaseElement element, bool raiseTreeChanged = false)
        {
            ElementManager.RemoveElement(parentElement, element, raiseTreeChanged);
        }

        public void AddLink(IBaseElement sourceElement, IBaseElement targetElement, string identifier = null,
            bool raiseTreeChanged = true)
        {
            ElementManager.AddLink(sourceElement, targetElement, identifier, raiseTreeChanged);
        }

        public void RemoveLink(IBaseElement sourceElement, IBaseElement targetElement, bool raiseTreeChanged = true)
        {
            ElementManager.RemoveLink(sourceElement, targetElement, raiseTreeChanged);
        }

        public IBaseElement GetElementByAddress(string address)
        {
            return ElementManager.GetElementByAddress(address);
        }

        public void RaiseTreeChanged(IBaseElement parentElement = null, IBaseElement childElement = null,
            TreeChangedAction action = TreeChangedAction.TreeChanged)
        {
            ElementManager.RaiseTreeChanged(parentElement, childElement, action);
        }

        public event EventHandler<TreeChangedEventArgs> TreeChanged
        {
            add => ElementManager.TreeChanged += value;
            remove => ElementManager.TreeChanged -= value;
        }
        
        public IBaseElement GetRootElement()
        {
            return ElementManager.GetRootElement();
        }

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

        public ResponseMessage HandleRequest(int cid, string address, JToken data = null, string reply = null)
        {
            return MessageHandler.HandleRequest(cid, address, data, reply);
        }

        public ResponseMessage HandleRequest(RequestMessage message)
        {
            return MessageHandler.HandleRequest(message);
        }

        public void HandleEvent(EventMessage message)
        {
            MessageHandler.HandleEvent(message);
        }

        public ResponseMessage SendRequest(Uri remoteUri, RequestMessage message, TimeSpan? timeout = null)
        {
            return MessageSender.SendRequest(remoteUri, message, timeout);
        }

        public void SendEvent(Uri remoteUri, EventMessage message)
        {
            MessageSender.SendEvent(remoteUri, message);
        }

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

        public IServerNetAdapter FindReverseServerNetAdapter(Uri remoteUri)
        {
            return ServerNetAdapterManager.FindReverseServerNetAdapter(remoteUri);
        }

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
    }
}