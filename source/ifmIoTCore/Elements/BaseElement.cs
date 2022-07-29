namespace ifmIoTCore.Elements
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using Common;
    using EventArguments;
    using Exceptions;
    using Messages;
    using Resources;
    using Utilities;

    public class BaseElement : IBaseElement
    {
        public string Type { get; }

        public string Identifier { get; }

        public Format Format { get; protected set; }

        public IEnumerable<string> Profiles
        {
            get
            {
                if (!Lock.TryEnterReadLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
                }
                try
                {
                    return _profiles?.ToList();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }
        private List<string> _profiles;

        public string UId { get; }

        public string Address { get; set; }

        public IBaseElement Parent { get; set; }

        public IEnumerable<IBaseElement> Subs
        {
            get
            {
                if (!Lock.TryEnterReadLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
                }
                try
                {
                    return References.ForwardReferences?.Select(x => x.TargetElement).ToList();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        public IEnumerable<IElementReference> ForwardReferences
        {
            get
            {
                if (!Lock.TryEnterReadLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
                }
                try
                {
                    return References.ForwardReferences?.ToList();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        public IEnumerable<IElementReference> InverseReferences
        {
            get
            {
                if (!Lock.TryEnterReadLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
                }
                try
                {
                    return References.InverseReferences?.ToList();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        public IElementReferenceTable References { get; } = new ElementReferenceTable();

        public event EventHandler<TreeChangedEventArgs> ElementAdded;
        public event EventHandler<TreeChangedEventArgs> ElementRemoved;
        public event EventHandler<TreeChangedEventArgs> LinkAdded;
        public event EventHandler<TreeChangedEventArgs> LinkRemoved;
        public event EventHandler<TreeChangedEventArgs> TreeChanged;

        public object UserData { get; set; }

        public bool IsHidden { get; set; }
        
        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        
        public BaseElement(string type,
            string identifier,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false)
        {
            if (string.IsNullOrEmpty(type)) throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(type)));
            if (string.IsNullOrEmpty(identifier)) throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(identifier)));
            if (identifier.Contains("/") || identifier.Any(char.IsWhiteSpace)) throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.InvalidIdentifier, nameof(identifier)));

            Type = type;
            Identifier = identifier;
            Format = format;
            _profiles = profiles;
            UId = uid;
            Address = Helpers.RootAddress;
            IsHidden = isHidden;
        }

        public void AddProfile(string profile)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }
            try
            {
                _profiles ??= new List<string>();
                if (_profiles.Any(x => x.Equals(profile, StringComparison.OrdinalIgnoreCase))) return;
                _profiles.Add(profile);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public void RemoveProfile(string profile)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }
            try
            {
                if (_profiles == null) return;
                if (!_profiles.Remove(profile)) return;
                if (_profiles.Count == 0)
                {
                    _profiles = null;
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public bool HasProfile(string profile)
        {
            if (!Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }
            try
            {
                return _profiles?.Any(x => x.Equals(profile, StringComparison.OrdinalIgnoreCase)) ?? false;
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public IBaseElement AddChild(IBaseElement childElement, bool raiseTreeChanged = false)
        {
            if (childElement == null) throw new ArgumentNullException(nameof(childElement));

            // Lock this
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }

            try
            {
                // Check if reference to element or element with same identifier already exists
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetElement == childElement) != null ||
                    References.ForwardReferences?.FirstOrDefault(x => string.Equals(x.Identifier, childElement.Identifier, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    throw new IoTCoreException(ResponseCodes.AlreadyExists, string.Format(Resource1.ElementAlreadyExists, childElement.Identifier, Identifier));
                }

                // Check if adding element would create a circular dependency
                if (IsInAncestorTree(this, childElement))
                {
                    throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.AddAncestorElementNotAllowed, childElement.Identifier, Identifier));
                }

                // Lock element
                if (!childElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, childElement.Identifier));
                }

                try
                {
                    // Check if element already has a parent
                    if (childElement.Parent != null)
                    {
                        throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.ElementAlreadyHasParent, childElement.Identifier, Identifier));
                    }

                    // Set the element context
                    childElement.Parent = this;
                    childElement.References.AddInverseReference(this, childElement, childElement.Identifier, ReferenceType.Child);
                    UpdateChildAddress(this, childElement);

                    // Add event handlers to bubble up events from child element
                    childElement.ElementAdded += OnChildElementAdded;
                    childElement.ElementRemoved += OnChildElementRemoved;
                    childElement.LinkAdded += OnChildLinkAdded;
                    childElement.LinkRemoved += OnChildLinkRemoved;
                    childElement.TreeChanged += OnChildTreeChanged;
                }
                finally
                {
                    childElement.Lock.ExitWriteLock();
                }

                // Add the reference to the element
                References.AddForwardReference(this, childElement, childElement.Identifier, ReferenceType.Child);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            // Raise the events
            RaiseElementAdded(childElement);
            if (raiseTreeChanged)
            {
                RaiseTreeChanged(childElement, TreeChangedAction.ElementAdded);
            }

            return childElement;
        }

        private static bool IsInAncestorTree(IBaseElement ancestorElement, IBaseElement element)
        {
            if (ancestorElement == element) return true;
            return ancestorElement.Parent != null && IsInAncestorTree(ancestorElement.Parent, element);
        }

        private static void UpdateChildAddress(IBaseElement parentElement, IBaseElement childElement)
        {
            childElement.Address = Helpers.CreateAddress(parentElement.Address, childElement.Identifier);

            if (childElement.References.ForwardReferences == null) return;
            foreach (var item in childElement.References.ForwardReferences)
            {
                if (item.Type == ReferenceType.Child)
                {
                    UpdateChildAddress(childElement, item.TargetElement);
                }
            }
        }

        public void RemoveChild(IBaseElement childElement, bool raiseTreeChanged = false)
        {
            if (childElement == null) throw new ArgumentNullException(nameof(childElement));

            // Lock this
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }

            try
            {
                // Check if the element is a child element
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetElement == childElement && x.IsChild) == null)
                {
                    throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotChild, childElement.Identifier, Identifier));
                }

                // Lock element
                if (!childElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, childElement.Identifier));
                }

                try
                {
                    // Remove the element context
                    childElement.Parent = null;
                    childElement.References.RemoveInverseReference(this, childElement);

                    // Remove event handler from child element
                    childElement.ElementAdded -= OnChildElementAdded;
                    childElement.ElementRemoved -= OnChildElementRemoved;
                    childElement.LinkAdded -= OnChildLinkAdded;
                    childElement.LinkRemoved -= OnChildLinkRemoved;
                    childElement.TreeChanged -= OnChildTreeChanged;
                }
                finally
                {
                    childElement.Lock.ExitWriteLock();
                }

                // Remove the reference to the element
                References.RemoveForwardReference(this, childElement);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            // Raise the events
            RaiseElementRemoved(childElement);
            if (raiseTreeChanged)
            {
                RaiseTreeChanged(childElement, TreeChangedAction.ElementRemoved);
            }
        }

        public void AddLink(IBaseElement targetElement, string identifier = null, bool raiseTreeChanged = false)
        {
            if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

            // Lock this
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }

            try
            {
                identifier ??= targetElement.Identifier;

                // Check if reference to element or element with same identifier already exists
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetElement == targetElement) != null ||
                    References.ForwardReferences?.FirstOrDefault(x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    throw new IoTCoreException(ResponseCodes.AlreadyExists, string.Format(Resource1.ElementAlreadyExists, identifier, Identifier));
                }

                // Check if adding element would create a circular dependency
                if (IsCircularDependency(this, targetElement))
                {
                    throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.AddLinkNotAllowed, Identifier, targetElement.Identifier));
                }

                // Lock element
                if (!targetElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, targetElement.Identifier));
                }

                try
                {
                    // Set the element context
                    targetElement.References.AddInverseReference(this, targetElement, identifier, ReferenceType.Link);
                }
                finally
                {
                    targetElement.Lock.ExitWriteLock();
                }

                // Add the reference to the element
                References.AddForwardReference(this, targetElement, identifier, ReferenceType.Link);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            // Raise the events
            RaiseLinkAdded(targetElement, identifier);
            if (raiseTreeChanged)
            {
                RaiseTreeChanged(targetElement, TreeChangedAction.LinkAdded);
            }
        }

        private static bool IsCircularDependency(IBaseElement sourceElement, IBaseElement targetElement)
        {
            if (sourceElement == targetElement)
            {
                return true;
            }

            if (targetElement.ForwardReferences != null)
            {
                foreach (var reference in targetElement.ForwardReferences)
                {
                    if (IsCircularDependency(sourceElement, reference.TargetElement))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public void RemoveLink(IBaseElement targetElement, bool raiseTreeChanged = false)
        {
            if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

            // Lock this
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }

            string identifier;
            try
            {
                // Check if the element is a linked element
                if (References.ForwardReferences?.FirstOrDefault(x => x.TargetElement == targetElement && x.IsLink) == null)
                {
                    throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotLinked, targetElement.Identifier, Identifier));
                }

                // Lock element
                if (!targetElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, targetElement.Identifier));
                }

                try
                {
                    // Remove the element context
                    targetElement.References.RemoveInverseReference(this, targetElement);
                }
                finally
                {
                    targetElement.Lock.ExitWriteLock();
                }

                // Remove the reference to the element
                identifier = References.RemoveForwardReference(this, targetElement).Identifier;
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            // Raise the events
            RaiseLinkRemoved(targetElement, identifier);
            if (raiseTreeChanged)
            {
                RaiseTreeChanged(targetElement, TreeChangedAction.LinkRemoved);
            }
        }

        private void OnChildElementAdded(object sender, TreeChangedEventArgs e)
        {
            ElementAdded.Raise(this, e);
        }

        private void OnChildElementRemoved(object sender, TreeChangedEventArgs e)
        {
            ElementRemoved.Raise(this, e);
        }

        private void OnChildLinkAdded(object sender, TreeChangedEventArgs e)
        {
            LinkAdded.Raise(this, e);
        }

        private void OnChildLinkRemoved(object sender, TreeChangedEventArgs e)
        {
            LinkRemoved.Raise(this, e);
        }

        private void OnChildTreeChanged(object sender, TreeChangedEventArgs e)
        {
            TreeChanged.Raise(this, e);
        }

        private void RaiseElementAdded(IBaseElement childElement)
        {
            ElementAdded.Raise(this, new TreeChangedEventArgs(TreeChangedAction.ElementAdded, this, childElement));
        }

        private void RaiseElementRemoved(IBaseElement childElement)
        {
            ElementRemoved.Raise(this, new TreeChangedEventArgs(TreeChangedAction.ElementRemoved, this, childElement));
        }

        private void RaiseLinkAdded(IBaseElement targetElement, string identifier)
        {
            LinkAdded.Raise(this, new TreeChangedEventArgs(TreeChangedAction.LinkAdded, this, targetElement, identifier));
        }

        private void RaiseLinkRemoved(IBaseElement targetElement, string identifier)
        {
            LinkRemoved.Raise(this, new TreeChangedEventArgs(TreeChangedAction.LinkRemoved, this, targetElement, identifier));
        }

        public void RaiseTreeChanged(IBaseElement childElement, TreeChangedAction action)
        {
            var treeChangedEventArgs = new TreeChangedEventArgs(action, this, childElement);
            TreeChanged.Raise(this, treeChangedEventArgs);
        }

        public IBaseElement GetElementByAddress(string address, bool includeSelf = true, bool recurse = true)
        {
            return GetElementByPredicate(x => string.Equals(x.Address, Helpers.RemoveDeviceName(address), StringComparison.OrdinalIgnoreCase), includeSelf, recurse);
        }

        public IBaseElement GetElementByIdentifier(string identifier, bool includeSelf = true, bool recurse = true)
        {
            return GetElementByPredicate(this, x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase), includeSelf, recurse);
        }

        public IBaseElement GetElementByProfile(string profile, bool includeSelf = true, bool recurse = true)
        {
            return GetElementByPredicate(this, x => x.HasProfile(profile), includeSelf, recurse);
        }

        public IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate, bool includeSelf = true, bool recurse = true)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return GetElementByPredicate(this, predicate, includeSelf, recurse);
        }

        public IEnumerable<IBaseElement> GetElementsByType(string type, bool includeSelf = true, bool recurse = true)
        {
            var result = new List<IBaseElement>();
            GetElementsByPredicate(this, x => x.Type == type, result, includeSelf, recurse);
            return result;
        }

        public IEnumerable<IBaseElement> GetElementsByProfile(string profile, bool includeSelf = true, bool recurse = true)
        {
            var result = new List<IBaseElement>();
            GetElementsByPredicate(this, x => x.HasProfile(profile), result, includeSelf, recurse);
            return result;
        }

        public IEnumerable<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate, bool includeSelf = true, bool recurse = true)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<IBaseElement>();
            GetElementsByPredicate(this, predicate, result, includeSelf, recurse);
            return result;
        }

        private static IBaseElement GetElementByPredicate(IBaseElement element, Predicate<IBaseElement> predicate, bool includeSelf, bool recurse)
        {
            if (!element.Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, element.Identifier));
            }
            try
            {
                if (includeSelf)
                {
                    if (predicate(element))
                    {
                        return element;
                    }
                }

                if (!recurse || element.References.ForwardReferences == null) return null;
                foreach (var item in element.References.ForwardReferences)
                {
                    var result = GetElementByPredicate(item.TargetElement, predicate, true, true);
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
            finally
            {
                element.Lock.ExitReadLock();
            }
        }

        private static void GetElementsByPredicate(IBaseElement element, Predicate<IBaseElement> predicate, ICollection<IBaseElement> result, bool includeSelf, bool recurse)
        {
            if (!element.Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, element.Identifier));
            }
            try
            {
                if (includeSelf)
                {
                    if (predicate(element))
                    {
                        result.Add(element);
                    }
                }

                if (!recurse || element.References.ForwardReferences == null) return;
                foreach (var item in element.References.ForwardReferences)
                {
                    GetElementsByPredicate(item.TargetElement, predicate, result, true, true);
                }
            }
            finally
            {
                element.Lock.ExitReadLock();
            }
        }

        public override string ToString()
        {
            return $"{Address}";
        }
    }
}