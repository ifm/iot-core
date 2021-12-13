namespace ifmIoTCore.Profiles.IoTCoreManagement
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    using Elements;
    using Exceptions;
    using Messages;
    using Resources;
    using ServiceData.Requests;
    using ServiceData.Responses;

    public class IoTCoreManagementProfileBuilder
    {
        public const string ProfileName = "iotcore_management";
        public const string AddElementName = "addelement";
        public const string RemoveElementName = "removeelement";
        public const string AddProfileName = "addprofile";
        public const string RemoveProfileName = "removeprofile";
        public const string GetElementInfoName = "getelementinfo";
        public const string SetElementInfoName = "setelementinfo";
        public const string AddLinkName = "addlink";
        public const string RemoveLinkName = "removelink";

        private readonly IIoTCore _ioTCore;

        public string Name { get; } = ProfileName;

        public IoTCoreManagementProfileBuilder(IIoTCore iotCore)
        {
            _ioTCore = iotCore;
        }

        public void Build()
        {
            var structureElement = _ioTCore.CreateStructureElement(_ioTCore.Root, ProfileName, profiles: new List<string> {ProfileName});

            _ioTCore.CreateSetterServiceElement<AddElementRequestServiceData>(structureElement, AddElementName, AddElementFunc);
            _ioTCore.CreateSetterServiceElement<RemoveElementRequestServiceData>(structureElement, RemoveElementName, RemoveElementFunc);
            _ioTCore.CreateServiceElement<AddProfileRequestServiceData, AddProfileResponseServiceData>(structureElement, AddProfileName, AddProfileFunc);
            _ioTCore.CreateServiceElement<RemoveProfileRequestServiceData, RemoveProfileResponseServiceData>(structureElement, RemoveProfileName, RemoveProfileFunc);
            _ioTCore.CreateSetterServiceElement<AddLinkRequestServiceData>(structureElement, AddLinkName, AddLinkFunc);
            _ioTCore.CreateSetterServiceElement<RemoveLinkRequestServiceData>(structureElement, RemoveLinkName, RemoveLinkFunc);
        }

        private void AddElementFunc(IBaseElement element, AddElementRequestServiceData data, int? cid = null)
        {
            AddElement(data);
        }

        public void AddElement(AddElementRequestServiceData data)
        {
            if (data == null)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, AddElementName));
            }
            
            var parentElement = _ioTCore.GetElementByAddress(data.Address);
            
            if (parentElement == null)
            {
                throw new Exception(string.Format(Resource1.ElementNotFound, data.Address));
            }

            switch (data.Type)
            {
                case Identifiers.Data:
                    var dataElement = _ioTCore.CreateDataElement<JToken>(parentElement, 
                        data.Identifier, 
                        null, 
                        null, 
                        true, 
                        true,
                        null,
                        TimeSpan.FromMilliseconds(100),
                        data.Format,
                        data.Profiles,
                        data.UId);
                    if (data.AddDataChanged)
                    {
                        dataElement.DataChangedEventElement = _ioTCore.CreateEventElement(dataElement, Identifiers.DataChanged);
                    }

                    break;

                case Identifiers.Structure:
                    _ioTCore.CreateStructureElement(parentElement, data.Identifier, data.Format, data.Profiles, data.UId);
                    break;

                case Identifiers.Event:
                    var eventElement = _ioTCore.CreateEventElement(parentElement, data.Identifier, null, null, data.Format, data.Profiles, data.UId);
                    _ioTCore.CreateActionServiceElement(eventElement, Identifiers.TriggerEvent, (e, cid) =>
                    {
                        eventElement.RaiseEvent();
                    });
                    break;
                default:
                    throw new ServiceException(ResponseCodes.BadRequest, string.Format(Resource1.InvalidArgument, data.Type));
            }
        }

        private void RemoveElementFunc(IBaseElement element, RemoveElementRequestServiceData data, int? cid = null)
        {
            if (data?.Address == Identifiers.Root)
            {
                throw new ServiceException(ResponseCodes.BadRequest, string.Format(Resource1.InvalidArgument, data.Address));
            }
            RemoveElement(data);
        }

        public void RemoveElement(RemoveElementRequestServiceData data)
        {
            if (data == null)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, RemoveElementName));
            }

            var element = _ioTCore.Root.GetElementByAddress(data.Address);
            if (element == null)
            {
                throw new ServiceException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.Address));
            }
            _ioTCore.RemoveElement(element.Parent, element);
        }


        private AddProfileResponseServiceData AddProfileFunc(IBaseElement element, AddProfileRequestServiceData data, int? cid = null)
        {
            if (data == null)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, AddProfileName));
            }

            if (data.Addresses == null || data.Addresses.Count == 0)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, Resource1.AddressListIsEmpty);
            }

            return AddProfile(data);
        }

        public AddProfileResponseServiceData AddProfile(AddProfileRequestServiceData data)
        {
            AddProfileResponseServiceData result = new AddProfileResponseServiceData();

            foreach (var address in data.Addresses)
            {
                var element = _ioTCore.Root.GetElementByAddress(address);

                if (!result.TryGetValue(address, out var list))
                {
                    list = new List<ProfileAddResult>();
                    result.Add(address, list);
                }

                if (element == null)
                {
                    list.Add(new ProfileAddResult(ProfileAddCode.ElementNotFound, null, "Element with this address was not found."));
                }
                else
                {
                    foreach (var profile in data.Profiles)
                    {
                        if (!element.HasProfile(profile))
                        {
                            element.AddProfile(profile);
                            list.Add(new ProfileAddResult(ProfileAddCode.Ok, profile));
                        }
                        else
                        {
                            list.Add(new ProfileAddResult(ProfileAddCode.ProfileAlreadyExistsOnElement, profile, "Element already contains profile."));
                        }
                    }
                }
            }

            return result;
        }

        private RemoveProfileResponseServiceData RemoveProfileFunc(IBaseElement element, RemoveProfileRequestServiceData data, int? cid = null)
        {
            return RemoveProfile(data);
        }

        public RemoveProfileResponseServiceData RemoveProfile(RemoveProfileRequestServiceData data)
        {
            RemoveProfileResponseServiceData result = new RemoveProfileResponseServiceData();

            if (data == null)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, RemoveProfileName));
            }

            foreach (var address in data.Addresses)
            {
                if (!result.TryGetValue(address, out var list))
                {
                    list = new List<ProfileRemoveResult>();
                    result.Add(address, list);
                }

                var element = _ioTCore.Root.GetElementByAddress(address);

                if (element == null)
                {
                    list.Add(new ProfileRemoveResult(ProfileRemoveCode.ElementNotFound, null, "Element with this address was not found."));
                }
                else
                {
                    foreach (var profile in data.Profiles)
                    {
                        if (!element.HasProfile(profile))
                        {
                            list.Add(new ProfileRemoveResult(ProfileRemoveCode.ProfileNotFoundOnElement, profile, "Cannot remove profile, since the element does not have the profile."));
                        }
                        else
                        {
                            element.RemoveProfile(profile);
                            list.Add(new ProfileRemoveResult(ProfileRemoveCode.Ok, profile, null));
                        }
                    }
                }
            }

            return result;
        }

        private void AddLinkFunc(IBaseElement element, AddLinkRequestServiceData data, int? cid = null)
        {
            AddLink(data);
        }

        public void AddLink(AddLinkRequestServiceData data)
        {
            if (data == null)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, AddLinkName));
            }

            var sourceElement = _ioTCore.GetElementByAddress(data.SourceAddress);
            if (sourceElement == null)
            {
                throw new ServiceException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.SourceAddress));
            }

            var targetElement = _ioTCore.GetElementByAddress(data.TargetAddress);
            if (targetElement == null)
            {
                throw new ServiceException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.TargetAddress));
            }
            _ioTCore.AddLink(sourceElement, targetElement, data.Identifier);
        }

        private void RemoveLinkFunc(IBaseElement element, RemoveLinkRequestServiceData data, int? cid = null)
        {
            RemoveLink(data);
        }

        public void RemoveLink(RemoveLinkRequestServiceData data)
        {
            if (data == null)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, RemoveLinkName));
            }

            var sourceElement = _ioTCore.GetElementByAddress(data.SourceAddress);
            if (sourceElement == null)
            {
                throw new ServiceException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.SourceAddress));
            }

            var targetElement = _ioTCore.GetElementByAddress(data.TargetAddress);
            if (targetElement == null)
            {
                throw new ServiceException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.TargetAddress));
            }

            _ioTCore.RemoveLink(sourceElement, targetElement);
        }
    }
}
