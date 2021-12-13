namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    public enum ReferenceType
    {
        Child,
        Link
    }

    public enum ReferenceDirection
    {
        Forward,
        Inverse
    }

    public class ElementReference
    {
        public readonly IBaseElement SourceElement;
        public readonly IBaseElement TargetElement;
        public readonly string Identifier;
        public readonly ReferenceType Type;
        public readonly ReferenceDirection Direction;

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

    public class ElementReferenceTable
    {
        public IEnumerable<ElementReference> ForwardReferences => _forwardReferences;
        private List<ElementReference> _forwardReferences;

        public IEnumerable<ElementReference> InverseReferences => _inverseReferences;
        private List<ElementReference> _inverseReferences;

        public void AddForwardReference(IBaseElement sourceElement,
            IBaseElement targetElement,
            string identifier,
            ReferenceType type)
        {
            var reference = new ElementReference(sourceElement, targetElement, identifier, type, ReferenceDirection.Forward);
            _forwardReferences ??= new List<ElementReference>();
            _forwardReferences.Add(reference);
        }

        public void AddInverseReference(IBaseElement sourceElement,
            IBaseElement targetElement,
            string identifier,
            ReferenceType type)
        {
            var reference = new ElementReference(sourceElement, targetElement, identifier, type, ReferenceDirection.Inverse);
            _inverseReferences ??= new List<ElementReference>();
            _inverseReferences.Add(reference);
        }

        public bool RemoveForwardReference(IBaseElement sourceElement,
            IBaseElement targetElement)
        {
            var reference = _forwardReferences?.FirstOrDefault(x => x.SourceElement == sourceElement && x.TargetElement == targetElement);
            if (reference == null) return false;
            if (!_forwardReferences.Remove(reference)) return false;
            if (_forwardReferences.Count == 0)
            {
                _forwardReferences = null;
            }
            return true;
        }

        public bool RemoveForwardReference(ElementReference reference)
        {
            if (_forwardReferences == null) return false;
            if (reference == null) return false;
            if (!_forwardReferences.Remove(reference)) return false;
            if (_forwardReferences.Count == 0)
            {
                _forwardReferences = null;
            }
            return true;
        }

        public bool RemoveInverseReference(IBaseElement sourceElement,
            IBaseElement targetElement)
        {
            var reference = _inverseReferences?.FirstOrDefault(x => x.SourceElement == sourceElement && x.TargetElement == targetElement);
            if (reference == null) return false;
            _inverseReferences.Remove(reference);
            if (_inverseReferences.Count == 0)
            {
                _inverseReferences = null;
            }
            return true;
        }

        public bool RemoveInverseReference(ElementReference reference)
        {
            if (_inverseReferences == null) return false;
            if (reference == null) return false;
            if (!_inverseReferences.Remove(reference)) return false;
            if (_inverseReferences.Count == 0)
            {
                _inverseReferences = null;
            }
            return true;
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
