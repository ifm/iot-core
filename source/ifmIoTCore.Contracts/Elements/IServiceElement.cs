namespace ifmIoTCore.Elements
{
    using Common.Variant;

    /// <summary>
    /// Provides functionality to interact with a service element
    /// </summary>
    public interface IServiceElement : IBaseElement
    {
        /// <summary>
        /// Invokes the service function
        /// </summary>
        /// <param name="data">The service parameter</param>
        /// <param name="cid">The context id</param>
        /// <returns>The service result</returns>
        Variant Invoke(Variant data = null, int? cid = null);
    }
}
