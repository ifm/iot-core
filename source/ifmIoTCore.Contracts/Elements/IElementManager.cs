namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Provides functionality to interact with the element manager
    /// </summary>
    public interface IElementManager
    {
        /// <summary>
        /// Locks access
        /// </summary>
        ReaderWriterLockSlim Lock { get; }

        /// <summary>
        /// The element tree root element
        /// </summary>
        IDeviceElement Root { get; set; }

        /// <summary>
        /// Gets the element with the specified address
        /// </summary>
        /// <param name="address">The address of the element</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByAddress(string address);

        /// <summary>
        /// Gets all elements with the specified profile
        /// </summary>
        /// <param name="profile">The profile of the elements</param>
        /// <returns>The elements that match the profile</returns>
        IEnumerable<IBaseElement> GetElementsByProfile(string profile);
    }
}