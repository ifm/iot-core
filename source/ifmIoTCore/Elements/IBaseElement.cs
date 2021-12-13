namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Formats;

    /// <summary>
    /// Provides functionality to interact with a base element
    /// </summary>
    public interface IBaseElement : IDisposable
    {
        /// <summary>
        /// Gets the type of the element
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the identifier of the element
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets the format of the element
        /// </summary>
        Format Format { get; }

        /// <summary>
        /// Gets the profiles of the element
        /// </summary>
        IEnumerable<string> Profiles { get; }

        /// <summary>
        /// Gets the unique identifier of the element
        /// </summary>
        string UId { get; }

        /// <summary>
        /// Gets the address of the element. The address uniquely identifies the element
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Gets the parent element of the element
        /// </summary>
        IBaseElement Parent { get; }

        /// <summary>
        /// Gets the child elements of the element
        /// </summary>
        IEnumerable<IBaseElement> Subs { get; }

        /// <summary>
        /// Gets the element references that this element references
        /// </summary>
        IEnumerable<ElementReference> ForwardReferences { get; }

        /// <summary>
        /// Gets the element references that reference this element
        /// </summary>
        IEnumerable<ElementReference> InverseReferences { get; }

        /// <summary>
        /// Gets the element references table. Note that the access to the table is not thread-safe
        /// </summary>
        ElementReferenceTable References { get; }

        /// <summary>
        /// Gets or sets the user data of the element
        /// </summary>
        object UserData { get; set; }

        /// <summary>
        /// Gets or sets the hidden status of the element; true if the element is hidden; otherwise false
        /// </summary>
        bool IsHidden { get; set; }

        /// <summary>
        /// Gets the context of the element
        /// </summary>
        object Context { get; }

        /// <summary>
        /// Gets the access lock of the element
        /// </summary>
        ReaderWriterLockSlim Lock { get; }

        /// <summary>
        /// Adds a profile
        /// </summary>
        /// <param name="profile">The profile to add</param>
        void AddProfile(string profile);

        /// <summary>
        /// Removes a profile
        /// </summary>
        /// <param name="profile">The profile to remove</param>
        void RemoveProfile(string profile);

        /// <summary>
        /// Checks if this element has the profile given in <paramref name="profile"/>.
        /// </summary>
        /// <param name="profile">The name of the profile to check.</param>
        /// <returns>true, if this element supports the profile <paramref name="profile"/>, otherwise false.</returns>
        bool HasProfile(string profile);

        /// <summary>
        /// Gets the first element with the specified path including the element itself into the search
        /// </summary>
        /// <param name="path">The path to the element</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByPath(string path);

        /// <summary>
        /// Gets the element with the specified address including the element itself into the search
        /// </summary>
        /// <param name="address">The address of the element</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByAddress(string address);

        /// <summary>
        /// Gets the first element with the specified identifier including the element itself into the search
        /// </summary>
        /// <param name="identifier">The identifier of the element</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByIdentifier(string identifier);

        /// <summary>
        /// Gets the first element with the specified profile including the element itself into the search
        /// </summary>
        /// <param name="profile">The profile of the element</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByProfile(string profile);

        /// <summary>
        /// Gets the first element with the specified predicate including the element itself into the search
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate);

        /// <summary>
        /// Gets all elements with the specified type including the element itself into the search
        /// </summary>
        /// <param name="type">The type of the elements</param>
        /// <returns>The elements that match the type</returns>
        IEnumerable<IBaseElement> GetElementsByType(string type);

        /// <summary>
        /// Gets all elements with the specified profile including the element itself into the search
        /// </summary>
        /// <param name="profile">The profile of the elements</param>
        /// <returns>The elements that match the profile</returns>
        IEnumerable<IBaseElement> GetElementsByProfile(string profile);

        /// <summary>
        /// Gets all elements with the specified predicate including the element itself into the search
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The elements that match the predicate</returns>
        IEnumerable<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate);
    }
}
