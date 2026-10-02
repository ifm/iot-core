namespace ifm.IoTCore.ElementManager.Elements;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using Common.Exceptions;
using Contracts.Elements;
using Contracts.Elements.Formats;
using ifm.Common;
using ifm.Common.Tree;

internal class BaseElement : IBaseElement
{
    private static Regex IdentifierValidator => new("^[a-zA-Z0-9_\\-\\]\\[]*$");
    private static Regex ProfileValidator => new("^[a-zA-Z0-9_\\-/\\]\\[/]*$");

    private readonly ReaderWriterLockSlim _lock = new();
    private readonly int _timeout;

    private ConcurrentDictionary<string, object> _profiles;
    private ConcurrentDictionary<string, object> _tags;
    private ConcurrentDictionary<string, object> _infos;
    private ConcurrentDictionary<string, object> _userData;

    public BaseElement(string type,
        string identifier,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        int timeout = 5000)
    {
        if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));
        if (!IdentifierValidator.IsMatch(identifier)) throw new ArgumentException(identifier);

        Type = type;
        Identifier = identifier;
        Address = AddressParser.CreateAddress(null, Identifier);
        Format = format;
        if (profiles != null)
        {
            _profiles = new ConcurrentDictionary<string, object>();
            foreach (var profile in profiles)
            {
                _profiles.TryAdd(profile, null);
            }
        }
        UId = uid;
        IsHidden = isHidden;
        _timeout = timeout;
    }

    public string Type { get; }

    public string Identifier { get; }

    public string Address { get; private set; }

    public Format Format { get; protected set; }

    public IReadOnlyList<string> Profiles => _profiles?.Keys.ToList();

    public IReadOnlyList<string> Tags => _tags?.Keys.ToList();

    public IReadOnlyDictionary<string, object> Infos => _infos;

    public string UId { get; }

    public IReadOnlyList<IBaseElement> Subs
    {
        get
        {
            EnterReadLock();
            try
            {
                // Take a snapshot
                return References.ForwardReferences?.Select(x => x.TargetNode).ToList();
            }
            finally
            {
                ExitReadLock();
            }
        }
    }

    public bool IsHidden { get; set; }

    public void AddProfile(string profile)
    {
        if (!ProfileValidator.IsMatch(profile)) throw new ArgumentException(profile);

        _profiles ??= new ConcurrentDictionary<string, object>();
        _profiles.TryAdd(profile, null);
    }

    public void RemoveProfile(string profile)
    {
        if (_profiles == null) return;
        if (!_profiles.TryRemove(profile, out _)) return;
        if (_profiles.IsEmpty) _profiles = null;
    }

    public bool HasProfile(string profile)
    {
        return _profiles != null && _profiles.ContainsKey(profile);
    }

    public void AddTag(string tag)
    {
        _tags ??= new ConcurrentDictionary<string, object>();
        _tags.TryAdd(tag, null);
    }

    public void RemoveTag(string tag)
    {
        if (_tags == null) return;
        if (!_tags.TryRemove(tag, out _)) return;
        if (_tags.IsEmpty) _tags = null;
    }

    public bool HasTag(string tag)
    {
        return _tags != null && _tags.ContainsKey(tag);
    }

    public void AddInfo<T>(string key, T value)
    {
        _infos ??= new ConcurrentDictionary<string, object>();
        _infos.TryAdd(key, value);
    }

    public void RemoveInfo(string key)
    {
        if (_infos == null) return;
        _infos.TryRemove(key, out _);
        if (_infos.IsEmpty) _infos = null;
    }

    public T GetInfo<T>(string key)
    {
        if (_infos.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default;
    }

    public void AddUserData<T>(string key, T value)
    {
        _userData ??= new ConcurrentDictionary<string, object>();
        _userData.TryAdd(key, value);
    }

    public void RemoveUserData(string key)
    {
        if (_userData == null) return;
        _userData.TryRemove(key, out _);
        if (_userData.IsEmpty) _userData = null;
    }

    public T GetUserData<T>(string key)
    {
        if (_userData.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default;
    }

    public void EnterReadLock()
    {
        if (!_lock.TryEnterReadLock(_timeout))
        {
            throw new LockedException("The element is locked");
        }
    }

    public void ExitReadLock()
    {
        _lock.ExitReadLock();
    }

    public void EnterUpgradeableReadLock()
    {
        if (!_lock.TryEnterUpgradeableReadLock(_timeout))
        {
            throw new LockedException("The element is locked");
        }
    }

    public void ExitUpgradeableReadLock()
    {
        _lock.ExitUpgradeableReadLock();
    }

    public void EnterWriteLock()
    {
        if (!_lock.TryEnterWriteLock(_timeout))
        {
            throw new LockedException("The element is locked");
        }
    }

    public void ExitWriteLock()
    {
        _lock.ExitWriteLock();
    }

    public IBaseElement Parent
    {
        get => _parent;
        set
        {
            if (_parent != null)
            {
                _parent.ParentChanged -= OnParentParentChanged;
            }
            _parent = value;
            Address = AddressParser.CreateAddress(_parent?.Address, Identifier);
            if (_parent != null)
            {
                Parent.ParentChanged += OnParentParentChanged;
            }
            ParentChanged?.Raise(this);
            return;

            void OnParentParentChanged(object sender, EventArgs _)
            {
                Address = AddressParser.CreateAddress(((IBaseElement)sender).Address, Identifier);
                ParentChanged?.Raise(this);
            }
        }
    }
    private IBaseElement _parent;

    public event EventHandler ParentChanged;

    public ReferenceTable<IBaseElement> References { get; } = new();

    public IReadOnlyList<Reference<IBaseElement>> ForwardReferences
    {
        get
        {
            EnterReadLock();
            try
            {
                // Take a snapshot
                return References.ForwardReferences?.Select(item => new Reference<IBaseElement>(item.SourceNode, item.TargetNode, item.Type, item.Direction, item.Identifier)).ToList();
            }
            finally
            {
                ExitReadLock();
            }
        }
    }

    public IReadOnlyList<Reference<IBaseElement>> InverseReferences
    {
        get
        {
            EnterReadLock();
            try
            {
                // Take a snapshot
                return References.InverseReferences?.Select(item => new Reference<IBaseElement>(item.SourceNode, item.TargetNode, item.Type, item.Direction, item.Identifier)).ToList();
            }
            finally
            {
                ExitReadLock();
            }
        }
    }

    public event EventHandler<TreeChangedEventArgs<IBaseElement>> TreeChanged;

    public void RaiseTreeChanged()
    {
        RaiseTreeChanged(new TreeChangedEventArgs<IBaseElement>(TreeChangedActions.TreeChanged, this, null, null));
    }

    public IBaseElement AddChild(IBaseElement element, bool raiseTreeChanged)
    {
        ExceptionHelpers.ThrowIfNull(element, nameof(element));

        EnterUpgradeableReadLock();
        try
        {
            EnterWriteLock();
            try
            {
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetNode == element) != null)
                {
                    throw new AlreadyExistsException($"The element '{this}' already has a reference to element '{element}'");
                }
                if (References.ForwardReferences?.FirstOrDefault(x => string.Equals(x.Identifier, element.Identifier, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    throw new AlreadyExistsException($"The element '{this}' already has a child element with identifier '{element.Identifier}'");
                }
                if (IsCircularDependency(this, element))
                {
                    throw new BadRequestException($"Adding element '{element}' to element '{this}' creates a circular dependency.");
                }
                element.EnterWriteLock();
                try
                {
                    if (element.Parent != null)
                    {
                        throw new BadRequestException($"The node {element} already has a parent node {element.Parent}");
                    }
                    element.Parent = this;
                    element.References.AddInverseReference(this, element, ReferenceTypes.Child, element.Identifier);
                    element.TreeChanged += OnTreeChanged;
                }
                finally
                {
                    element.ExitWriteLock();
                }
                References.AddForwardReference(this, element, ReferenceTypes.Child, element.Identifier);
            }
            finally
            {
                ExitWriteLock();
            }

            if (raiseTreeChanged)
            {
                RaiseTreeChanged(new TreeChangedEventArgs<IBaseElement>(TreeChangedActions.ChildAdded, this, element, element.Identifier));
            }

            return element;
        }
        finally
        {
            ExitUpgradeableReadLock();
        }
    }

    public void RemoveChild(IBaseElement element, bool raiseTreeChanged)
    {
        ExceptionHelpers.ThrowIfNull(element, nameof(element));

        EnterUpgradeableReadLock();
        try
        {
            EnterWriteLock();
            try
            {
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetNode == element && x.IsChild) == null)
                {
                    throw new NotFoundException($"The element {element} is not a child element of {this}");
                }
                element.EnterWriteLock();
                try
                {
                    if (IsLinked(element))
                    {
                        throw new BadRequestException($"The element {element} is linked or contains linked nodes");
                    }
                    element.Parent = null;
                    element.References.RemoveInverseReference(this, element);
                    element.TreeChanged -= OnTreeChanged;
                }
                finally
                {
                    element.ExitWriteLock();
                }
                References.RemoveForwardReference(this, element);
            }
            finally
            {
                ExitWriteLock();
            }

            if (raiseTreeChanged)
            {
                RaiseTreeChanged(new TreeChangedEventArgs<IBaseElement>(TreeChangedActions.ChildRemoved, this, element, null));
            }
        }
        finally
        {
            ExitUpgradeableReadLock();
        }
    }

    public void AddLink(IBaseElement element, string identifier, bool raiseTreeChanged)
    {
        ExceptionHelpers.ThrowIfNull(element, nameof(element));

        identifier ??= element.Identifier;

        EnterUpgradeableReadLock();
        try
        {
            EnterWriteLock();
            try
            {
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetNode == element) != null)
                {
                    throw new AlreadyExistsException($"The element '{this}' already has a reference to element '{element}'");
                }
                if (References.ForwardReferences?.FirstOrDefault(x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    throw new AlreadyExistsException($"The element '{this}' already has a child element with identifier '{element.Identifier}'");
                }
                if (IsCircularDependency(this, element))
                {
                    throw new BadRequestException($"Adding element '{element}' to element '{this}' creates a circular dependency.");
                }
                element.EnterWriteLock();
                try
                {
                    element.References.AddInverseReference(this, element, ReferenceTypes.Link, identifier);
                }
                finally
                {
                    element.ExitWriteLock();
                }
                References.AddForwardReference(this, element, ReferenceTypes.Link, identifier);
            }
            finally
            {
                ExitWriteLock();
            }

            if (raiseTreeChanged)
            {
                RaiseTreeChanged(new TreeChangedEventArgs<IBaseElement>(TreeChangedActions.LinkAdded, this, element, identifier));
            }
        }
        finally
        {
            ExitUpgradeableReadLock();
        }
    }

    public void RemoveLink(IBaseElement element, bool raiseTreeChanged)
    {
        ExceptionHelpers.ThrowIfNull(element, nameof(element));

        EnterUpgradeableReadLock();
        try
        {
            EnterWriteLock();
            try
            {
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetNode == element && x.IsLink) == null)
                {
                    throw new NotFoundException($"The element {this} does not have a link to {element}");
                }
                element.EnterWriteLock();
                try
                {
                    element.References.RemoveInverseReference(this, element);
                }
                finally
                {
                    element.ExitWriteLock();
                }
                References.RemoveForwardReference(this, element);
            }
            finally
            {
                ExitWriteLock();
            }

            if (raiseTreeChanged)
            {
                RaiseTreeChanged(new TreeChangedEventArgs<IBaseElement>(TreeChangedActions.LinkRemoved, this, element, null));
            }
        }
        finally
        {
            ExitUpgradeableReadLock();
        }
    }

    public IBaseElement GetElementByIdentifier(string identifier)
    {
        ExceptionHelpers.ThrowIfNull(identifier, nameof(identifier));

        EnterReadLock();
        try
        {
            if (References.ForwardReferences == null) return null;
            foreach (var item in References.ForwardReferences)
            {
                if (item.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                {
                    return item.TargetNode;
                }
            }
            return null;
        }
        finally
        {
            ExitReadLock();
        }
    }

    public IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate, bool recurse, bool includeSelf)
    {
        ExceptionHelpers.ThrowIfNull(predicate, nameof(predicate));

        EnterReadLock();
        try
        {
            if (includeSelf && predicate(this)) return this;
            if (References.ForwardReferences == null) return null;
            foreach (var item in References.ForwardReferences)
            {
                if (!item.IsChild) continue;
                var result = GetElementByPredicate(item.TargetNode, predicate, recurse);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            ExitReadLock();
        }
    }

    private static IBaseElement GetElementByPredicate(IBaseElement element, Predicate<IBaseElement> predicate, bool recurse)
    {
        if (predicate(element)) return element;
        if (!recurse) return null;

        element.EnterReadLock();
        try
        {
            if (element.References.ForwardReferences == null) return null;
            foreach (var item in element.References.ForwardReferences)
            {
                if (!item.IsChild) continue;
                var result = GetElementByPredicate(item.TargetNode, predicate, true);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            element.ExitReadLock();
        }
    }

    public IBaseElement GetElementByPredicateInverse(Predicate<IBaseElement> predicate, bool recurse, bool includeSelf)
    {
        EnterReadLock();
        try
        {
            if (includeSelf && predicate(this)) return this;
            if (References.InverseReferences == null) return null;
            foreach (var item in References.InverseReferences)
            {
                if (!item.IsChild) continue;
                var result = GetElementByPredicateInverse(item.SourceNode, predicate, recurse);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            ExitReadLock();
        }
    }

    private static IBaseElement GetElementByPredicateInverse(IBaseElement element, Predicate<IBaseElement> predicate, bool recurse)
    {
        if (predicate(element)) return element;
        if (!recurse) return null;

        element.EnterReadLock();
        try
        {
            if (element.References.InverseReferences == null) return null;
            foreach (var item in element.References.InverseReferences)
            {
                if (!item.IsChild) continue;
                var result = GetElementByPredicateInverse(item.SourceNode, predicate, true);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            element.ExitReadLock();
        }
    }

    public IReadOnlyList<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate, bool recurse, bool includeSelf)
    {
        EnterReadLock();
        try
        {
            var result = new List<IBaseElement>();
            if (includeSelf && predicate(this)) result.Add(this);
            if (References.ForwardReferences == null) return null;
            foreach (var item in References.ForwardReferences)
            {
                if (item.IsChild) GetElementsByPredicate(item.TargetNode, predicate, recurse, result);
            }
            return result;
        }
        finally
        {
            ExitReadLock();
        }
    }

    private static void GetElementsByPredicate(IBaseElement element, Predicate<IBaseElement> predicate, bool recurse, ICollection<IBaseElement> result)
    {
        if (predicate(element)) result.Add(element);
        if (!recurse) return;

        element.EnterReadLock();
        try
        {
            if (element.References.ForwardReferences == null) return;
            foreach (var item in element.References.ForwardReferences)
            {
                if (item.IsChild) GetElementsByPredicate(item.TargetNode, predicate, true, result);
            }
        }
        finally
        {
            element.ExitReadLock();
        }
    }

    public override string ToString()
    {
        return Identifier;
    }

    private void OnTreeChanged(object sender, TreeChangedEventArgs<IBaseElement> args)
    {
        TreeChanged.Raise(sender, args);
    }

    private void RaiseTreeChanged(TreeChangedEventArgs<IBaseElement> args)
    {
        TreeChanged.Raise(this, args);
    }

    private static bool IsCircularDependency(IBaseElement sourceElement, IBaseElement targetElement)
    {
        if (sourceElement == targetElement) return true;
        return targetElement.References.ForwardReferences != null && targetElement.References.ForwardReferences.Any(x => IsCircularDependency(sourceElement, x.TargetNode));
    }

    private static bool IsLinked(IBaseElement targetElement)
    {
        if (targetElement.References.InverseReferences.FirstOrDefault(x => x.IsLink) != null) return true;
        return targetElement.References.ForwardReferences != null && targetElement.References.ForwardReferences.Any(x => IsLinked(x.TargetNode));
    }
}