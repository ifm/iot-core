namespace ifm.IoTCore.ElementManager.Contracts.Elements;

using Common;
using ifm.Common.Variant;

/// <summary>
/// Defines additional service information.
/// </summary>
/// <param name="cid">The request context id.</param>
/// <param name="sessionToken">The session token that called the service.</param>
public class ServiceInfo(int cid, 
    string sessionToken)
{
    /// <summary>
    /// The service context id.
    /// </summary>
    public int Cid { get; } = cid;

    /// <summary>
    /// The session token that called the service.
    /// </summary>
    public string SessionToken { get; } = sessionToken;

    /// <summary>
    /// The service response code.
    /// </summary>
    public int? ResponseCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public ServiceInfo() : this(0, null)
    {
    }
}

/// <summary>
/// Controls access to a service.
/// </summary>
public enum AccessRestrictions
{
    /// <summary>No access restriction, user can call service without restrictions.</summary>
    None = 0,
    /// <summary>User with initial password can call service.</summary>
    InitialPassword = 1,
    /// <summary>User with changed password can call service.</summary>
    ChangedPassword = 2
}

/// <summary>
/// Controls access to a service.
/// </summary>
public class AccessControl
{
    /// <summary>A bitfield which specifies the user roles which have access to the service.</summary>
    public UserRole UserRoles { get; set; } = UserRole.Administrator | UserRole.Maintainer | UserRole.Observer;
}

/// <summary>
/// Provides functionality to interact with a service element.
/// </summary>
public interface IServiceElement : IBaseElement
{
    /// <summary>
    /// Invokes the service function.
    /// </summary>
    /// <param name="data">The service input data.</param>
    /// <param name="info">Additional service information.</param>
    /// <returns>The service output data.</returns>
    Variant Invoke(Variant data, ServiceInfo info = null);

    /// <summary>
    /// Controls access to the service.
    /// </summary>
    //[Obsolete("AccessRestrictions is deprecated. Use AccessControl instead.")]
    AccessRestrictions AccessRestrictions { get; set; }

    /// <summary>
    /// Controls access to the service.
    /// </summary>
    AccessControl AccessControl { get; }
}

/// <summary>
/// Provides functionality to interact with a service element.
/// </summary>
/// <typeparam name="TIn">The input data type.</typeparam>
/// <typeparam name="TOut">The output data type.</typeparam>
public interface IServiceElement<in TIn, out TOut> : IServiceElement
{
    /// <summary>
    /// Invokes the service function.
    /// </summary>
    /// <param name="data">The service input data.</param>
    /// <param name="info">Additional service information.</param>
    /// <returns>The service output data.</returns>
    TOut Invoke(TIn data, ServiceInfo info = null);
}

/// <summary>
/// Provides functionality to interact with an action service element.
/// </summary>
public interface IActionServiceElement : IServiceElement
{
    /// <summary>
    /// Invokes the service function.
    /// </summary>
    /// <param name="info">Additional service information.</param>
    void Invoke(ServiceInfo info = null);
}

/// <summary>
/// Provides functionality to interact with a getter service element.
/// </summary>
/// <typeparam name="TOut">The data type.</typeparam>
public interface IGetterServiceElement<out TOut> : IServiceElement
{
    /// <summary>
    /// Invokes the service function.
    /// </summary>
    /// <param name="info">Additional service information.</param>
    /// <returns>The service output data.</returns>
    TOut Invoke(ServiceInfo info = null);
}

/// <summary>
/// Provides functionality to interact with a setter service element.
/// </summary>
/// <typeparam name="TIn">The data type.</typeparam>
public interface ISetterServiceElement<in TIn> : IServiceElement
{
    /// <summary>
    /// Invokes the service function.
    /// </summary>
    /// <param name="data">The service input data.</param>
    /// <param name="info">Additional service information.</param>
    void Invoke(TIn data, ServiceInfo info = null);
}
