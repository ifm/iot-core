namespace ifmIoTCore.Utilities
{
    using System;
    using Elements;
    using Exceptions;
    using Messages;
    using Newtonsoft.Json.Linq;

    public static class Helpers
    {
        public const char AddressSeparator = '/';
        public const string RootAddress = "/";

        /// <summary>
        /// Creates the address for an element
        /// </summary>
        /// <param name="parentAddress">The address of the parent element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <returns>The address for the element</returns>
        public static string CreateAddress(string parentAddress, string identifier)
        {
            // In this version the address starts with the device name
            //return string.IsNullOrEmpty(parentAddress) ? $"{identifier}" : $"{parentAddress}{AddressSeparator}{identifier}";

            // In this version the address starts with "/"
            return string.IsNullOrEmpty(parentAddress) ? 
                RootAddress : 
                parentAddress.EndsWith(AddressSeparator) ? 
                    $"{parentAddress}{identifier}" : 
                    $"{parentAddress}/{identifier}";
        }

        /// <summary>
        /// Creates the address for an element
        /// </summary>
        /// <param name="parentAddress">The address of the parent element</param>
        /// <param name="identifiers">The identifiers of the element</param>
        /// <returns>The address for the element</returns>
        public static string CreateAddress(string parentAddress, params string[] identifiers)
        {
            // In this version the address starts with the device name
            //return string.IsNullOrEmpty(parentAddress) ? $"{identifier}" : $"{parentAddress}{AddressSeparator}{identifier}";

            // In this version the address starts with "/"
            return string.IsNullOrEmpty(parentAddress) ?
                RootAddress :
                parentAddress.EndsWith(AddressSeparator) ?
                    $"{parentAddress}{string.Join(AddressSeparator.ToString(), identifiers)}" :
                    $"{parentAddress}/{string.Join(AddressSeparator.ToString(), identifiers)}";
        }

        public static string RemoveDeviceName(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                if (address.IndexOf(AddressSeparator) == -1)
                {
                    return AddressSeparator.ToString();
                }
                if (!string.IsNullOrEmpty(address) && address[0] != AddressSeparator)
                {
                    return address.RemoveFirstToken(AddressSeparator);
                }
            }
            return address;
        }

        public static string AddDeviceName(string address, string deviceName)
        {
            return address.IndexOf(AddressSeparator) != 0 ? address : $"{deviceName}{address}";
        }

        public static bool CheckDeviceName(string address, string deviceName)
        {
            var deviceNameFromAddress = address.Left(AddressSeparator);
            if (!string.IsNullOrEmpty(deviceNameFromAddress))
            {
                return string.Compare(deviceNameFromAddress, deviceName, StringComparison.OrdinalIgnoreCase) == 0;
            }
            return true;
        }

        public static T FromJson<T>(JToken json)
        {
            T data;
            try
            {
                data = json != null ? json.ToObject<T>() : default;
            }
            catch (Exception e)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, e.Message);
            }
            return data;
        }

        public static JToken ToJson<T>(T data)
        {
            return data != null ? JToken.FromObject(data) : null;
        }
    }
}
