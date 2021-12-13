namespace ifmIoTCore.Elements
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using System.Diagnostics;
    using Exceptions;
    using Formats;
    using Messages;
    using Resources;
    using Utilities;

    internal class BaseElement : IBaseElement
    {
        public string Type { get; }

        public string Identifier { get; }

        public Format Format
        {
            get;
            protected set;
        }

        public IEnumerable<string> Profiles
        {
            get
            {
                // Take a snapshot
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
                // Non thread-safe version
                //return _profiles;
            }
        }
        private List<string> _profiles;

        public string UId { get; }

        public string Address { get; }

        public IBaseElement Parent { get; private set; }

        public IEnumerable<IBaseElement> Subs
        {
            get
            {
                // Take a snapshot
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
                // Non thread-safe version
                //if (_references.ForwardReferences == null)
                //{
                //    yield break;
                //}
                //foreach (var item in _references.ForwardReferences)
                //{
                //    yield return item.TargetElement;
                //}
            }
        }

        public IEnumerable<ElementReference> ForwardReferences
        {
            get
            {
                // Take a snapshot
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
                // Non thread-safe version
                //return References.ForwardReferences;
            }
        }

        public IEnumerable<ElementReference> InverseReferences
        {
            get
            {
                // Take a snapshot
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
                // Non thread-safe version
                //return References.InverseReferences;
            }
        }

        public ElementReferenceTable References { get; } = new ElementReferenceTable();

        public object UserData { get; set; }

        public bool IsHidden { get; set; }

        public object Context { get; }

        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

#if DEBUG
        private static int _elementCount;
#endif

        public BaseElement(IBaseElement parent,
            string type,
            string identifier,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null)
        {
            if (string.IsNullOrEmpty(type)) throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(type)));
            if (string.IsNullOrEmpty(identifier)) throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(identifier)));
            if (identifier.Contains("/") || identifier.Any(char.IsWhiteSpace)) throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.InvalidIdentifier, nameof(identifier)));

            Parent = parent;
            Type = type;
            Identifier = identifier;
            Format = format;
            _profiles = profiles;
            UId = uid;
            IsHidden = isHidden;
            Context = context;
            Address = Helpers.CreateAddress(Parent?.Address, Identifier);

            References.AddInverseReference(parent, this, Identifier, ReferenceType.Child);

#if DEBUG
            Debug.WriteLine($"Created element '{Identifier}' (address: {Address})");
            Debug.WriteLine($"Current element count: {++_elementCount}");
#endif
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

        public IBaseElement GetElementByPath(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            throw new NotImplementedException();
        }

        //private IBaseElement GetElementByPath(IBaseElement element, string path)
        //{
        //    if (!element.Lock.TryEnterReadLock(Configuration.Settings.Timeout))
        //    {
        //        throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, element.Identifier));
        //    }
        //    try
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        element.Lock.ExitReadLock();
        //    }
        //}

        public IBaseElement GetElementByAddress(string address)
        {
            return GetElementByPredicate(x => string.Equals(x.Address, Helpers.RemoveDeviceName(address), StringComparison.OrdinalIgnoreCase));
        }

        public IBaseElement GetElementByIdentifier(string identifier)
        {
            return GetElementByPredicate(this, x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
        }

        public IBaseElement GetElementByProfile(string profile)
        {
            return GetElementByPredicate(this, x => x.HasProfile(profile));
        }

        public IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return GetElementByPredicate(this, predicate);
        }

        public IEnumerable<IBaseElement> GetElementsByType(string type)
        {
            var result = new List<IBaseElement>();
            GetElementsByPredicate(this, x => x.Type == type, result);
            return result;
        }

        public IEnumerable<IBaseElement> GetElementsByProfile(string profile)
        {
            var result = new List<IBaseElement>();
            GetElementsByPredicate(this, x => x.HasProfile(profile), result);
            return result;
        }

        public IEnumerable<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<IBaseElement>();
            GetElementsByPredicate(this, predicate, result);
            return result;
        }

        private static IBaseElement GetElementByPredicate(IBaseElement element, Predicate<IBaseElement> predicate)
        {
            if (!element.Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, element.Identifier));
            }
            try
            {
                if (predicate(element))
                {
                    return element;
                }

                if (element.References.ForwardReferences == null) return null;
                foreach (var item in element.References.ForwardReferences)
                {
                    var result = GetElementByPredicate(item.TargetElement, predicate);
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

        private static void GetElementsByPredicate(IBaseElement element, Predicate<IBaseElement> predicate, ICollection<IBaseElement> result)
        {
            if (!element.Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, element.Identifier));
            }
            try
            {
                if (predicate(element))
                {
                    result.Add(element);
                }

                if (element.References.ForwardReferences == null) return;
                foreach (var item in element.References.ForwardReferences)
                {
                    GetElementsByPredicate(item.TargetElement, predicate, result);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.WriteLine($"Disposing element '{Identifier}' (address: {Address})");

                // Dispose all child elements, because child elements are owned by the element and cannot exist without parent
                if (References.ForwardReferences != null)
                {
                    foreach (var reference in References.ForwardReferences)
                    {
                        if (reference.Type == ReferenceType.Child)
                        {
                            reference.TargetElement.Dispose();
                        }
                    }
                }
                References.Clear();
                Lock.Dispose();
                Parent = null;

#if DEBUG
                Debug.WriteLine($"Current element count: {--_elementCount}");
#endif
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}