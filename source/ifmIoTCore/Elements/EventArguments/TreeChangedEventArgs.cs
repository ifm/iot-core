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
        public TreeChangedAction Action;

        /// <summary>
        /// The parent element that was affected by the change
        /// </summary>
        public IBaseElement ParentElement;

        /// <summary>
        /// The child element involved in the change
        /// </summary>
        public IBaseElement ChildElement;
    }

}
