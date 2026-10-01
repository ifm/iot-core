namespace ifm.IoTCore.ElementManager.Contracts.Elements;

using System;
using System.Collections.Generic;
using Formats;
using ifm.Common.Tree;
using ifm.Common.Variant;

/// <summary>
/// Provides functionality to interact with a base element.
/// </summary>
public interface IBaseElement
{
    #region IoTCore properties

    /// <summary>
    /// Gets the type of the element.
    /// </summary>
    [VariantProperty("type", Required = true)]
    string Type { get; }

    /// <summary>
    /// Gets the identifier of the element.
    /// </summary>
    [VariantProperty("identifier", Required = true)]
    string Identifier { get; }

    /// <summary>
    /// Gets the address of the element. The address uniquely identifies the element.
    /// </summary>
    [VariantProperty("adr", IgnoredIfNull = true)]
    string Address { get; }

    /// <summary>
    /// Gets the format of the element.
    /// </summary>
    [VariantProperty("format", IgnoredIfNull = true)]
    Format Format { get; }

    /// <summary>
    /// Gets the profiles of the element.
    /// </summary>
    [VariantProperty("profiles", IgnoredIfNull = true)]
    IReadOnlyList<string> Profiles { get; }

    /// <summary>
    /// Gets the tags of the element.
    /// </summary>
    [VariantProperty("tags", IgnoredIfNull = true)]
    IReadOnlyList<string> Tags { get; }

    /// <summary>
    /// Gets the application or profile specific information about the element.
    /// </summary>
    [VariantProperty("infos", IgnoredIfNull = true)]
    public IReadOnlyDictionary<string, object> Infos { get; }

    /// <summary>
    /// Gets the unique identifier of the element.
    /// </summary>
    [VariantProperty("uid", IgnoredIfNull = true)]
    string UId { get; }

    /// <summary>
    /// Gets a snapshot of all referenced elements.
    /// </summary>
    [VariantProperty("subs", IgnoredIfNull = true)]
    IReadOnlyList<IBaseElement> Subs { get; }

    /// <summary>
    /// Gets or sets the hidden status of the element.
    /// </summary>
    bool IsHidden { get; set; }

    /// <summary>
    /// Adds a profile.
    /// </summary>
    /// <param name="profile">The name of the profile to add.</param>
    void AddProfile(string profile);

    /// <summary>
    /// Removes a profile.
    /// </summary>
    /// <param name="profile">The name of the profile to remove.</param>
    void RemoveProfile(string profile);

    /// <summary>
    /// Checks if this element has the profile.
    /// </summary>
    /// <param name="profile">The name of the profile.</param>
    /// <returns>true, if this element has the profile, otherwise false.</returns>
    bool HasProfile(string profile);

    /// <summary>
    /// Adds a tag.
    /// </summary>
    /// <param name="tag">The name of the tag to add.</param>
    void AddTag(string tag);

    /// <summary>
    /// Removes a tag.
    /// </summary>
    /// <param name="tag">The name of the tag to remove.</param>
    void RemoveTag(string tag);

    /// <summary>
    /// Checks if this element has the tag.
    /// </summary>
    /// <param name="tag">The name of the tag.</param>
    /// <returns>true, if this element has the tag, otherwise false.</returns>
    bool HasTag(string tag);

    /// <summary>
    /// Adds the element info with the associated key.
    /// </summary>
    /// <typeparam name="T">The type of the element info.</typeparam>
    /// <param name="key">The key of the element info.</param>
    /// <param name="value">The element info.</param>
    void AddInfo<T>(string key, T value);

    /// <summary>
    /// Removes the element info with the associated key.
    /// </summary>
    /// <param name="key">The key of the element info.</param>
    void RemoveInfo(string key);

    /// <summary>
    /// Gets the element info which is associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the element info.</typeparam>
    /// <param name="key">The key of the element info to get.</param>
    /// <returns>The user data.</returns>
    T GetInfo<T>(string key);

    #endregion

    #region user data

    /// <summary>
    /// Adds the user data with the associated key.
    /// </summary>
    /// <typeparam name="T">The type of the user data.</typeparam>
    /// <param name="key">The key of the user data.</param>
    /// <param name="value">The user data.</param>
    void AddUserData<T>(string key, T value);

    /// <summary>
    /// Removes the user data with the associated key.
    /// </summary>
    /// <param name="key">The key of the user data.</param>
    void RemoveUserData(string key);

    /// <summary>
    /// Gets the user data which is associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the user data.</typeparam>
    /// <param name="key">The key of the user data to get.</param>
    /// <returns>The user data.</returns>
    T GetUserData<T>(string key);

    #endregion

    #region Tree management

    /// <summary>
    /// Enter read lock.
    /// </summary>
    void EnterReadLock();

    /// <summary>
    /// Exit read lock.
    /// </summary>
    void ExitReadLock();

    /// <summary>
    /// Enter upgradable read lock.
    /// </summary>
    void EnterUpgradeableReadLock();

    /// <summary>
    /// Exit upgradable read lock.
    /// </summary>
    void ExitUpgradeableReadLock();

    /// <summary>
    /// Enter write lock.
    /// </summary>
    void EnterWriteLock();

    /// <summary>
    /// Exit write lock.
    /// </summary>
    void ExitWriteLock();

    /// <summary>
    /// Gets or sets the parent element.
    /// </summary>
    IBaseElement Parent { get; set; }

    /// <summary>
    /// The event that is raised when the parent is modified.
    /// </summary>
    event EventHandler ParentChanged;

    /// <summary>
    /// Gets the reference table.
    /// </summary>
    public ReferenceTable<IBaseElement> References { get; }

    /// <summary>
    /// Gets a snapshot of the forward references.
    /// </summary>
    IReadOnlyList<Reference<IBaseElement>> ForwardReferences { get; }

    /// <summary>
    /// Gets a snapshot of the inverse references.
    /// </summary>
    IReadOnlyList<Reference<IBaseElement>> InverseReferences { get; }

    /// <summary>
    /// The event that is raised when the underlying tree is modified.
    /// </summary>
    event EventHandler<TreeChangedEventArgs<IBaseElement>> TreeChanged;

    /// <summary>
    /// Adds a child reference to the provided element.
    /// </summary>
    /// <param name="element">The element to reference.</param>
    IBaseElement AddChild(IBaseElement element);

    /// <summary>
    /// Removes a child reference to the provided element.
    /// </summary>
    /// <param name="element">The element to reference.</param>
    void RemoveChild(IBaseElement element);

    /// <summary>
    /// Adds a link reference to the provided element.
    /// </summary>
    /// <param name="element">The element to reference.</param>
    /// <param name="identifier">The identifier for the reference.</param>
    void AddLink(IBaseElement element, string identifier = null);

    /// <summary>
    /// Removes a link reference to the provided element.
    /// </summary>
    /// <param name="element">The element to reference.</param>
    void RemoveLink(IBaseElement element);

    /// <summary>
    /// Gets the element with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the element.</param>
    /// <returns>The requested element if it exists; otherwise null.</returns>
    IBaseElement GetElementByIdentifier(string identifier);

    /// <summary>
    /// Gets the first element with the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match.</param>
    /// <param name="recurse">If true the tree is search is recursively searched; otherwise not</param>
    /// <param name="includeSelf">If true the instance is included in the search; otherwise not.</param>
    /// <returns>The requested element if it exists; otherwise null.</returns>
    IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate, bool recurse = true, bool includeSelf = true);

    /// <summary>
    /// Gets the first element with the specified predicate in inverse direction.
    /// </summary>
    /// <param name="predicate">The predicate to match.</param>
    /// <param name="recurse">If true the tree is search is recursively searched; otherwise not</param>
    /// <param name="includeSelf">If true the instance is included in the search; otherwise not.</param>
    /// <returns>The requested element if it exists; otherwise null.</returns>
    IBaseElement GetElementByPredicateInverse(Predicate<IBaseElement> predicate, bool recurse = true, bool includeSelf = true);

    /// <summary>
    /// Gets all elements with the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match.</param>
    /// <param name="recurse">If true the tree is search is recursively searched; otherwise not</param>
    /// <param name="includeSelf">If true the instance is included in the search; otherwise not.</param>
    /// <returns>The elements that match the predicate.</returns>
    IReadOnlyList<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate, bool recurse = true, bool includeSelf = true);

    #endregion
}