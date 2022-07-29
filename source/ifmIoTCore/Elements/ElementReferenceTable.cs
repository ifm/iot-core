namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    public class ElementReference : IElementReference
    {
        public IBaseElement SourceElement { get; }
        public IBaseElement TargetElement { get; }
        public string Identifier { get; }
        public ReferenceType Type { get; }
        public ReferenceDirection Direction { get; }

        public ElementReference(IBaseElement sourceElement,
            IBaseElement targetElement,
            string identifier,
            ReferenceType type,
            ReferenceDirection direction)
        {
            SourceElement = sourceElement;
            TargetElement = targetElement;
            Identifier = identifier;
            Type = type;
            Direction = direction;
        }

        public bool IsChild => Type == ReferenceType.Child;

        public bool IsLink => Type == ReferenceType.Link;

        public bool IsForward => Direction == ReferenceDirection.Forward;

        public bool IsInverse => Direction == ReferenceDirection.Inverse;

        public override string ToString()
        {
            return IsForward ? $"{SourceElement} --> {TargetElement}" : $"{SourceElement} <-- {TargetElement}";
        }
    }

    public class ElementReferenceTable : IElementReferenceTable
    {
        public IEnumerable<IElementReference> ForwardReferences => _forwardReferences;
        private List<IElementReference> _forwardReferences;

        public IEnumerable<IElementReference> InverseReferences => _inverseReferences;
        private List<IElementReference> _inverseReferences;

        public IElementReference AddForwardReference(IBaseElement sourceElement,
            IBaseElement targetElement,
            string identifier,
            ReferenceType type)
        {
            var reference = new ElementReference(sourceElement, targetElement, identifier, type, ReferenceDirection.Forward);
            _forwardReferences ??= new List<IElementReference>();
            _forwardReferences.Add(reference);
            return reference;
        }

        public IElementReference AddInverseReference(IBaseElement sourceElement,
            IBaseElement targetElement,
            string identifier,
            ReferenceType type)
        {
            var reference = new ElementReference(sourceElement, targetElement, identifier, type, ReferenceDirection.Inverse);
            _inverseReferences ??= new List<IElementReference>();
            _inverseReferences.Add(reference);
            return reference;
        }

        public IElementReference RemoveForwardReference(IBaseElement sourceElement,
            IBaseElement targetElement)
        {
            var reference = _forwardReferences?.FirstOrDefault(x => x.SourceElement == sourceElement && x.TargetElement == targetElement);
            if (reference == null) return null;
            if (!_forwardReferences.Remove(reference)) return null;
            if (_forwardReferences.Count == 0)
            {
                _forwardReferences = null;
            }
            return reference;
        }

        public IElementReference RemoveInverseReference(IBaseElement sourceElement,
            IBaseElement targetElement)
        {
            var reference = _inverseReferences?.FirstOrDefault(x => x.SourceElement == sourceElement && x.TargetElement == targetElement);
            if (reference == null) return null;
            if (!_inverseReferences.Remove(reference)) return null;
            if (_inverseReferences.Count == 0)
            {
                _inverseReferences = null;
            }
            return reference;
        }

        public void Clear()
        {
            if (_forwardReferences != null)
            {
                _forwardReferences.Clear();
                _forwardReferences = null;
            }

            if (_inverseReferences != null)
            {
                _inverseReferences.Clear();
                _inverseReferences = null;
            }
        }
    }
}
