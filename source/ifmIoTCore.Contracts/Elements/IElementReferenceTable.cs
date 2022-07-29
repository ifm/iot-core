namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;

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

    public interface IElementReference
    {
        public IBaseElement SourceElement { get; }
        public IBaseElement TargetElement { get; }
        public string Identifier { get; }
        public ReferenceType Type { get; }
        public ReferenceDirection Direction { get; }

        public bool IsChild { get; }

        public bool IsLink { get; }

        public bool IsForward { get; }

        public bool IsInverse { get; }
    }

    public interface IElementReferenceTable
    {
        public IEnumerable<IElementReference> ForwardReferences { get; }

        public IEnumerable<IElementReference> InverseReferences { get; }

        public IElementReference AddForwardReference(IBaseElement sourceElement,
            IBaseElement targetElement,
            string identifier,
            ReferenceType type);

        public IElementReference AddInverseReference(IBaseElement sourceElement,
            IBaseElement targetElement,
            string identifier,
            ReferenceType type);

        public IElementReference RemoveForwardReference(IBaseElement sourceElement,
            IBaseElement targetElement);

        public IElementReference RemoveInverseReference(IBaseElement sourceElement,
            IBaseElement targetElement);

        public void Clear();
    }
}
