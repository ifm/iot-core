namespace ifmIoTCore.Elements.EventArguments
{
    using System;

    /// <summary>
    /// Describes the action that caused a tree changed event 
    /// </summary>
    public enum TreeChangedAction
    {
        /// <summary>
        ///  An element was added
        /// </summary>
        ElementAdded,

        /// <summary>
        /// An element was removed
        /// </summary>
        ElementRemoved,

        /// <summary>
        ///  An link to an element was added
        /// </summary>
        LinkAdded,

        /// <summary>
        /// An link to an element was removed
        /// </summary>
        LinkRemoved,

        /// <summary>
        /// Some elements were added, some were removed
        /// </summary>
        TreeChanged
    }

    /// <summary>
    /// Provides data for the data tree changed event
    /// </summary>
    public class TreeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The action that caused the event
        /// </summary>
        public TreeChangedAction Action { get; }

        /// <summary>
        /// The parent element that was affected by the change
        /// </summary>
        public IBaseElement ParentElement { get; }

        /// <summary>
        /// The child element involved in the change
        /// </summary>
        public IBaseElement ChildElement { get; }

        /// <summary>
        /// The identifier for a link, if it is a link event
        /// </summary>
        public string Identifier { get; }

        public TreeChangedEventArgs(TreeChangedAction action, IBaseElement parentElement, IBaseElement childElement, string identifier = null)
        {
            Action = action;
            ParentElement = parentElement;
            ChildElement = childElement;
            Identifier = identifier;
        }
    }

}
