namespace ifmIoTCore.Elements
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides functionality to interact with a service element through JSON  
    /// </summary>
    public interface IServiceElement : IBaseElement
    {
        /// <summary>
        /// Invokes the service function
        /// </summary>
        /// <param name="data">The service parameter</param>
        /// <param name="cid">The context id</param>
        /// <returns>The service result</returns>
        JToken Invoke(JToken data, int? cid);
    }

    /// <summary>
    /// Provides functionality to interact with an action service element
    /// </summary>
    public interface IActionServiceElement : IServiceElement
    {
        /// <summary>
        /// Invokes the service function
        /// </summary>
        /// <param name="cid">The context id</param>
        void Invoke(int? cid);
    }

    /// <summary>
    /// Provides functionality to interact with a read only service element
    /// </summary>
    /// <typeparam name="TOut">The service result type</typeparam>
    public interface IGetterServiceElement<out TOut> : IServiceElement
    {
        /// <summary>
        /// Invokes the service function
        /// </summary>
        /// <param name="cid">The context id</param>
        /// <returns>The service result</returns>
        TOut Invoke(int? cid);
    }

    /// <summary>
    /// Provides functionality to interact with a write only service element
    /// </summary>
    /// <typeparam name="TIn">The service parameter type</typeparam>
    public interface ISetterServiceElement<in TIn> : IServiceElement
    {
        /// <summary>
        /// Invokes the service function
        /// </summary>
        /// <param name="data">The service parameter</param>
        /// <param name="cid">The context id</param>
        void Invoke(TIn data, int? cid);
    }

    /// <summary>
    /// Provides functionality to interact with a read and write service element
    /// </summary>
    /// <typeparam name="TIn">The service parameter type</typeparam>
    /// <typeparam name="TOut">The service result type</typeparam>
    public interface IServiceElement<in TIn, out TOut> : IServiceElement
    {
        /// <summary>
        /// Invokes the service function
        /// </summary>
        /// <param name="data">The service parameter</param>
        /// <param name="cid">The context id</param>
        /// <returns>The service result</returns>
        TOut Invoke(TIn data, int? cid);
    }
}
