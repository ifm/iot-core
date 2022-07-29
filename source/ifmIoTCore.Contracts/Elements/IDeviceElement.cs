namespace ifmIoTCore.Elements
{
    /// <summary>
    /// Provides functionality to interact with a device element
    /// </summary>
    public interface IDeviceElement : IBaseElement
    {
        /// <summary>
        /// Gets the getidentity service element
        /// </summary>
        IServiceElement GetIdentityServiceElement { get; }

        /// <summary>
        /// Gets the gettree service element
        /// </summary>
        IServiceElement GetTreeServiceElement { get; }

        /// <summary>
        /// Gets the querytree service element
        /// </summary>
        IServiceElement QueryTreeServiceElement { get; }

        /// <summary>
        /// Gets the getdatamulti service element
        /// </summary>
        IServiceElement GetDataMultiServiceElement { get; }

        /// <summary>
        /// Gets the setdatamulti service element
        /// </summary>
        IServiceElement SetDataMultiServiceElement { get; }

        /// <summary>
        /// Gets the getsubscriberlist service element
        /// </summary>
        IServiceElement GetSubscriberListServiceElement { get; }

        /// <summary>
        /// Gets the treechanged event element
        /// </summary>
        IEventElement TreeChangedEventElement { get; }

        /// <summary>
        /// Raises a tree changed event
        /// </summary>
        void RaiseTreeChanged();
    }
}
