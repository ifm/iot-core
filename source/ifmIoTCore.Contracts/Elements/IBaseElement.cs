namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using EventArguments;

    /// <summary>
    /// Provides functionality to interact with a base element
    /// </summary>
    public interface IBaseElement
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
        string Address { get; set; }

        /// <summary>
        /// Gets the parent element of the element
        /// </summary>
        IBaseElement Parent { get; set; }

        /// <summary>
        /// Gets the child elements of the element
        /// </summary>
        IEnumerable<IBaseElement> Subs { get; }

        /// <summary>
        /// Gets the element references that this element references
        /// </summary>
        IEnumerable<IElementReference> ForwardReferences { get; }

        /// <summary>
        /// Gets the element references that reference this element
        /// </summary>
        IEnumerable<IElementReference> InverseReferences { get; }

        /// <summary>
        /// Gets the element references table. Note that the access to the table is not thread-safe
        /// </summary>
        IElementReferenceTable References { get; }

        /// <summary>
        /// The element added event
        /// </summary>
        event EventHandler<TreeChangedEventArgs> ElementAdded;

        /// <summary>
        /// The element removed event
        /// </summary>
        event EventHandler<TreeChangedEventArgs> ElementRemoved;

        /// <summary>
        /// The link added event
        /// </summary>
        event EventHandler<TreeChangedEventArgs> LinkAdded;

        /// <summary>
        /// The link removed event
        /// </summary>
        event EventHandler<TreeChangedEventArgs> LinkRemoved;

        /// <summary>
        /// The treechanged event; the event will only be raised if the raise flag is set
        /// in the corresponding Add/Remove methods
        /// </summary>
        event EventHandler<TreeChangedEventArgs> TreeChanged;

        /// <summary>
        /// Gets or sets the user data of the element
        /// </summary>
        object UserData { get; set; }

        /// <summary>
        /// Gets or sets the hidden status of the element; true if the element is hidden; otherwise false
        /// </summary>
        bool IsHidden { get; set; }
        
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
        /// Adds a child element
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <param name="raiseTreeChanged">If true a treechanged event is raised; otherwise not</param>
        IBaseElement AddChild(IBaseElement element, bool raiseTreeChanged = false);

        /// <summary>
        /// Removes a child element
        /// </summary>
        /// <param name="element">The element to remove</param>
        /// <param name="raiseTreeChanged">If true a treechanged event is raised; otherwise not</param>
        void RemoveChild(IBaseElement element, bool raiseTreeChanged = false);

        /// <summary>
        /// Adds a link to an element
        /// </summary>
        /// <param name="element">The element to link</param>
        /// <param name="identifier">The identifier for the link; if null the identifier of the linked element is used</param>
        /// <param name="raiseTreeChanged">If true a treechanged event is raised; otherwise not</param>
        void AddLink(IBaseElement element, string identifier = null, bool raiseTreeChanged = false);

        /// <summary>
        /// Removes a link to an element
        /// </summary>
        /// <param name="element">The element to unlink</param>
        /// <param name="raiseTreeChanged">If true a treechanged event is raised; otherwise not</param>
        void RemoveLink(IBaseElement element, bool raiseTreeChanged = false);

        /// <summary>
        /// Raises a treechanged event
        /// </summary>
        /// <param name="childElement">The child element affected by the change; null, if multiple changes done (action == treechanged)</param>
        /// <param name="action">The action that caused the event</param>
        void RaiseTreeChanged(IBaseElement childElement = null, TreeChangedAction action = TreeChangedAction.TreeChanged);

        /// <summary>
        /// Gets the element with the specified address.
        /// </summary>
        /// <param name="address">The address of the element</param>
        /// <param name="includeSelf">If true the element is included in the search; otherwise not</param>
        /// <param name="recurse">If true a recursive search is done; otherwise not</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByAddress(string address, bool includeSelf = true, bool recurse = true);

        /// <summary>
        /// Gets the first element with the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="includeSelf">If true the element is included in the search; otherwise not</param>
        /// <param name="recurse">If true a recursive search is done; otherwise not</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByIdentifier(string identifier, bool includeSelf = true, bool recurse = true);

        /// <summary>
        /// Gets the first element with the specified profile.
        /// </summary>
        /// <param name="profile">The profile of the element</param>
        /// <param name="includeSelf">If true the element is included in the search; otherwise not</param>
        /// <param name="recurse">If true a recursive search is done; otherwise not</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByProfile(string profile, bool includeSelf = true, bool recurse = true);

        /// <summary>
        /// Gets the first element with the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <param name="includeSelf">If true the element is included in the search; otherwise not</param>
        /// <param name="recurse">If true a recursive search is done; otherwise not</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate, bool includeSelf = true, bool recurse = true);

        /// <summary>
        /// Gets all elements with the specified type.
        /// </summary>
        /// <param name="type">The type of the elements</param>
        /// <param name="includeSelf">If true the element is included in the search; otherwise not</param>
        /// <param name="recurse">If true a recursive search is done; otherwise not</param>
        /// <returns>The elements that match the type</returns>
        IEnumerable<IBaseElement> GetElementsByType(string type, bool includeSelf = true, bool recurse = true);

        /// <summary>
        /// Gets all elements with the specified profile.
        /// </summary>
        /// <param name="profile">The profile of the elements</param>
        /// <param name="includeSelf">If true the element is included in the search; otherwise not</param>
        /// <param name="recurse">If true a recursive search is done; otherwise not</param>
        /// <returns>The elements that match the profile</returns>
        IEnumerable<IBaseElement> GetElementsByProfile(string profile, bool includeSelf = true, bool recurse = true);

        /// <summary>
        /// Gets all elements with the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <param name="includeSelf">If true the element is included in the search; otherwise not</param>
        /// <param name="recurse">If true a recursive search is done; otherwise not</param>
        /// <returns>The elements that match the predicate</returns>
        IEnumerable<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate, bool includeSelf = true, bool recurse = true);
    }
}
