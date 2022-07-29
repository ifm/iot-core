using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.Profiles.IoTCoreManagement
{
    using System;
    using System.Collections.Generic;
    using Base;
    using Elements;
    using Exceptions;
    using Messages;
    using Resources;
    using ServiceData.Requests;
    using ServiceData.Responses;

    public class IoTCoreManagementProfileBuilder : BaseProfileBuilder
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

        

        public string Name => ProfileName;

        public IoTCoreManagementProfileBuilder(ProfileBuilderConfiguration config) :base(config)
        {
        }

        public override void Build()
        {
            var structureElement = IoTCore.Root.AddChild(new StructureElement(ProfileName, profiles: new List<string> {ProfileName}));

            structureElement.AddChild(new SetterServiceElement(AddElementName, AddElementFunc));
            structureElement.AddChild(new SetterServiceElement(RemoveElementName, RemoveElementFunc));
            structureElement.AddChild(new ServiceElement(AddProfileName, AddProfileFunc));
            structureElement.AddChild(new ServiceElement(RemoveProfileName, RemoveProfileFunc));
            structureElement.AddChild(new SetterServiceElement(AddLinkName, AddLinkFunc));
            structureElement.AddChild(new SetterServiceElement(RemoveLinkName, RemoveLinkFunc));
        }

        private void AddElementFunc(IBaseElement element, Variant data, int? cid = null)
        {
            var addElementRequestServiceData = Variant.ToObject<AddElementRequestServiceData>(data);
            AddElement(addElementRequestServiceData);
        }

        public void AddElement(AddElementRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, AddElementName));
            }
            
            var parentElement = IoTCore.GetElementByAddress(data.Address);
            
            if (parentElement == null)
            {
                throw new Exception(string.Format(Resource1.ElementNotFound, data.Address));
            }

            switch (data.Type)
            {
                case Identifiers.Data:
                    parentElement.AddChild(new DataElement<Variant>(data.Identifier,
                        null,
                        null,
                        data.AddDataChanged,
                        null,
                        TimeSpan.FromMilliseconds(100),
                        data.Format,
                        data.Profiles,
                        data.UId), true);
                    break;

                case Identifiers.Structure:
                    parentElement.AddChild(new StructureElement(data.Identifier, data.Format, data.Profiles, data.UId), true);
                    break;

                case Identifiers.Event:
                    var eventElement = (IEventElement)parentElement.AddChild(new EventElement(data.Identifier, data.Format, data.Profiles, data.UId));
                    eventElement.AddChild(new ActionServiceElement(Identifiers.TriggerEvent , (e, cid) =>
                    {
                        eventElement.RaiseEvent();
                    }), true);
                    break;
                default:
                    throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.InvalidArgument, data.Type));
            }
        }

        private void RemoveElementFunc(IBaseElement element, Variant data, int? cid = null)
        {
            var removeElementRequestServiceData = Variant.ToObject<RemoveElementRequestServiceData>(data);

            if (removeElementRequestServiceData?.Address == Identifiers.Root)
            {
                throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.InvalidArgument, removeElementRequestServiceData.Address));
            }

            RemoveElement(removeElementRequestServiceData);
        }

        public void RemoveElement(RemoveElementRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, RemoveElementName));
            }

            var element = IoTCore.Root.GetElementByAddress(data.Address);
            if (element == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.Address));
            }
            element.Parent.RemoveChild(element, true);
        }


        private Variant AddProfileFunc(IBaseElement element, Variant data, int? cid = null)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, AddProfileName));
            }

            var addProfileRequestServiceData = Variant.ToObject<AddProfileRequestServiceData>(data);

            if (addProfileRequestServiceData.Addresses == null || addProfileRequestServiceData.Addresses.Count == 0)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, Resource1.AddressListIsEmpty);
            }

            return Variant.FromObject(AddProfile(addProfileRequestServiceData));
        }

        public AddProfileResponseServiceData AddProfile(AddProfileRequestServiceData data)
        {
            AddProfileResponseServiceData result = new AddProfileResponseServiceData();

            foreach (var address in data.Addresses)
            {
                var element = IoTCore.Root.GetElementByAddress(address);

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

        private Variant RemoveProfileFunc(IBaseElement element, Variant data, int? cid = null)
        {
            RemoveProfileRequestServiceData removeProfileRequestServiceData = Variant.ToObject<RemoveProfileRequestServiceData>(data);
            var result = RemoveProfile(removeProfileRequestServiceData);

            return Variant.FromObject(result);
        }

        public RemoveProfileResponseServiceData RemoveProfile(RemoveProfileRequestServiceData data)
        {
            RemoveProfileResponseServiceData result = new RemoveProfileResponseServiceData();

            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, RemoveProfileName));
            }

            foreach (var address in data.Addresses)
            {
                if (!result.TryGetValue(address, out var list))
                {
                    list = new List<ProfileRemoveResult>();
                    result.Add(address, list);
                }

                var element = IoTCore.Root.GetElementByAddress(address);

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

        private void AddLinkFunc(IBaseElement element, Variant data, int? cid = null)
        {
            AddLink(Variant.ToObject<AddLinkRequestServiceData>(data));
        }

        public void AddLink(AddLinkRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, AddLinkName));
            }

            var sourceElement = IoTCore.GetElementByAddress(data.SourceAddress);
            if (sourceElement == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.SourceAddress));
            }

            var targetElement = IoTCore.GetElementByAddress(data.TargetAddress);
            if (targetElement == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.TargetAddress));
            }

            sourceElement.AddLink(targetElement, data.Identifier, true);
        }

        private void RemoveLinkFunc(IBaseElement element, Variant data, int? cid = null)
        {
            RemoveLink(Variant.ToObject<RemoveLinkRequestServiceData>(data));
        }

        public void RemoveLink(RemoveLinkRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, RemoveLinkName));
            }

            var sourceElement = IoTCore.GetElementByAddress(data.SourceAddress);
            if (sourceElement == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.SourceAddress));
            }

            var targetElement = IoTCore.GetElementByAddress(data.TargetAddress);
            if (targetElement == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.TargetAddress));
            }

            sourceElement.RemoveLink(targetElement, true);
        }
    }
}
