namespace ifmIoTCore.Profiles.DeviceManagement
{
    using System;
    using System.Linq;
    using Common;
    using Common.Variant;
    using Elements;
    using Exceptions;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using Messages;
    using NetAdapter;
    using Utilities;

    internal class ProxyDevice
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;

        private readonly Uri _remoteUri;
        private readonly string _callback;
        private readonly TimeSpan? _cacheTimeout;
        private readonly AuthenticationInfo _authenticationInfo;

        public IProxyElement RootElement;

        public string RemoteUri => _remoteUri.OriginalString;
        public string Alias { get; }

        public ProxyDevice(IClientNetAdapterManager clientNetAdapterManager,
            string remoteUri,
            string alias,
            string callback,
            int? cacheTimeout,
            AuthenticationInfo authenticationInfo)
        {
            _clientNetAdapterManager = clientNetAdapterManager;

            _remoteUri = new Uri(remoteUri);
            Alias = alias;
            _callback = callback;
            if (cacheTimeout != null)
            {
                _cacheTimeout = TimeSpan.FromMilliseconds(cacheTimeout.Value);
            }
            _authenticationInfo = authenticationInfo;
        }

        public void CreateElements(IBaseElement parentElement)
        {
            var remoteTree = GetRemoteTree();

            // Create a tree response element for the root of the tree 
            var remoteElement = new GetTreeResponseServiceData.Element
            {
                Type = remoteTree.Type,
                Identifier = Alias ?? remoteTree.Identifier,
                Profiles = remoteTree.Profiles,
                Subs = remoteTree.Subs
            };

            // Create the root device element
            var remoteContext = new RemoteContext(_remoteUri, Helpers.RootAddress, _authenticationInfo, _callback);
            RootElement = CreateProxyDeviceElement(parentElement, remoteElement, remoteContext);

            // Create all child elements
            if (remoteElement.Subs != null)
            {
                foreach (var item in remoteElement.Subs)
                {
                    CreateElement(RootElement, item);
                }
            }


            // ToDo: Create treechanged handler service and subscribe to remote tree changed event

            parentElement.RaiseTreeChanged();
        }

        private GetTreeResponseServiceData GetRemoteTree()
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(_remoteUri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(null, Identifiers.GetTree), 
                null, 
                null, 
                _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data.ToString());
            }

            return Variant.ToObject<GetTreeResponseServiceData>(response.Data);
        }


        private void CreateElement(IProxyElement parentElement,
            GetTreeResponseServiceData.Element remoteElement)
        {
            var remoteContext = new RemoteContext(_remoteUri, Helpers.CreateAddress(parentElement.RemoteContext.Address, remoteElement.Identifier), _authenticationInfo, _callback);

            IProxyElement element = null;
            switch (remoteElement.Type)
            {
                case Identifiers.Device:
                    element = CreateProxyDeviceElement(parentElement,  remoteElement, remoteContext);
                    break;
                case Identifiers.Structure:
                    element = CreateProxyStructureElement(parentElement, remoteElement, remoteContext);
                    break;
                case Identifiers.Service:
                    element = CreateProxyServiceServiceElement(parentElement, remoteElement, remoteContext);
                    break;
                case Identifiers.Event:
                    element = CreateProxyEventElement(parentElement, remoteElement, remoteContext);
                    break;
                case Identifiers.Data:
                    element = CreateProxyDataElement(parentElement, remoteElement, remoteContext);
                    break;
                case Identifiers.SubDevice:
                    element = CreateProxySubDeviceElement(parentElement, remoteElement, remoteContext);
                    break;
            }

            if (element != null && remoteElement.Subs != null)
            {
                foreach (var item in remoteElement.Subs)
                {
                    CreateElement(element, item);
                }
            }
        }

        private IProxyElement CreateProxyDeviceElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement, 
            RemoteContext remoteContext)
        {
            // Remove all standard child elements, because they are created along with the element
            remoteElement.Subs.RemoveAll(x => x.Identifier.Equals(Identifiers.GetIdentity, StringComparison.OrdinalIgnoreCase) ||
                                               x.Identifier.Equals(Identifiers.GetTree, StringComparison.OrdinalIgnoreCase) ||
                                               x.Identifier.Equals(Identifiers.QueryTree, StringComparison.OrdinalIgnoreCase) ||
                                               x.Identifier.Equals(Identifiers.GetDataMulti, StringComparison.OrdinalIgnoreCase) ||
                                               x.Identifier.Equals(Identifiers.SetDataMulti, StringComparison.OrdinalIgnoreCase) ||
                                               x.Identifier.Equals(Identifiers.GetSubscriberList, StringComparison.OrdinalIgnoreCase));

            var element = (ProxyDeviceElement)parentElement.AddChild(new ProxyDeviceElement(_clientNetAdapterManager,
                remoteContext,
                remoteElement.Identifier,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId), false);

            // Create treechanged proxy event element
            var remoteTreeChangedEventElement = remoteElement.Subs.FirstOrDefault(x => x.Identifier == Identifiers.TreeChanged);
            if (remoteTreeChangedEventElement != null)
            {
                remoteElement.Subs.Remove(remoteTreeChangedEventElement);

                var remoteContext2 = new RemoteContext(remoteContext.Uri,
                    Helpers.CreateAddress(remoteContext.Address, Identifiers.TreeChanged),
                    remoteContext.AuthenticationInfo,
                    remoteContext.Callback);

                var treeChangedEventElement = new ProxyEventElement(_clientNetAdapterManager,
                    remoteContext2,
                    remoteTreeChangedEventElement.Identifier,
                    remoteTreeChangedEventElement.Format,
                    remoteTreeChangedEventElement.Profiles,
                    remoteTreeChangedEventElement.UId);

                element.AddChild(treeChangedEventElement, false);
                element.TreeChangedEventElement = treeChangedEventElement;
            }

            return element;
        }

        private IProxyElement CreateProxyStructureElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement, 
            RemoteContext remoteContext)
        {
            return (ProxyStructureElement)parentElement.AddChild(new ProxyStructureElement(remoteContext, 
                remoteElement.Identifier,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId), false);
        }

        private IProxyElement CreateProxyServiceServiceElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement, 
            RemoteContext remoteContext)
        {
            return (ProxyServiceElement)parentElement.AddChild(new ProxyServiceElement(_clientNetAdapterManager,
                remoteContext,
                remoteElement.Identifier,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId), false);
        }

        private IProxyElement CreateProxyEventElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement, 
            RemoteContext remoteContext)
        {
            // Remove all standard child elements, because they are created along with the element
            remoteElement.Subs.RemoveAll(x => x.Identifier.Equals(Identifiers.Subscribe, StringComparison.OrdinalIgnoreCase) ||
                                               x.Identifier.Equals(Identifiers.Unsubscribe, StringComparison.OrdinalIgnoreCase));

            return (ProxyEventElement)parentElement.AddChild(new ProxyEventElement(_clientNetAdapterManager,
                remoteContext,
                remoteElement.Identifier,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId), false);
        }

        private IProxyElement CreateProxyDataElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement, 
            RemoteContext remoteContext)
        {
            // Remove all standard child elements, because they are created along with the element
            remoteElement.Subs.RemoveAll(x => x.Identifier.Equals(Identifiers.GetData, StringComparison.OrdinalIgnoreCase) ||
                                               x.Identifier.Equals(Identifiers.SetData, StringComparison.OrdinalIgnoreCase));

            var element = (ProxyDataElement)parentElement.AddChild(new ProxyDataElement(_clientNetAdapterManager,
                remoteContext,
                _cacheTimeout,
                remoteElement.Identifier,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId), false);

            // Create datachanged proxy event element
            var remoteDataChangedEventElement = remoteElement.Subs.FirstOrDefault(x => x.Identifier == Identifiers.DataChanged);
            if (remoteDataChangedEventElement != null)
            {
                remoteElement.Subs.Remove(remoteDataChangedEventElement);

                var remoteContext2 = new RemoteContext(remoteContext.Uri,
                    Helpers.CreateAddress(remoteContext.Address, Identifiers.DataChanged),
                    remoteContext.AuthenticationInfo,
                    remoteContext.Callback);

                var dataChangedEventElement = new ProxyEventElement(_clientNetAdapterManager,
                    remoteContext2,
                    remoteDataChangedEventElement.Identifier,
                    remoteDataChangedEventElement.Format,
                    remoteDataChangedEventElement.Profiles,
                    remoteDataChangedEventElement.UId);

                element.AddChild(dataChangedEventElement, false);
                element.DataChangedEventElement = dataChangedEventElement;
            }

            return element;
        }

        private IProxyElement CreateProxySubDeviceElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement, 
            RemoteContext remoteContext)
        {
            // Remove all standard child elements, because they are created along with the element
            remoteElement.Subs.Remove(x => x.Identifier.Equals(Identifiers.GetIdentity, StringComparison.OrdinalIgnoreCase));

            return (ProxySubDeviceElement)parentElement.AddChild(new ProxySubDeviceElement(_clientNetAdapterManager, 
                remoteContext,
                remoteElement.Identifier,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId), false);
        }

        public void RemoveElements(IBaseElement parentElement)
        {
            parentElement.RemoveChild(RootElement, true);
        }
    }
}
