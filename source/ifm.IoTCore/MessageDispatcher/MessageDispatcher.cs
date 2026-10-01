namespace ifm.IoTCore.MessageDispatcher;

using System;
using ifm.Common;
using ifm.Common.Variant;
using ifm.Logger.Contracts;

using Common;
using Common.Exceptions;
using ifm.IoTCore.ElementManager.Contracts;
using ifm.IoTCore.ElementManager.Contracts.Elements;
using ifm.IoTCore.UserManager.Contracts;
using Message;
using Contracts;

public class MessageDispatcher(IElementManager elementManager,
    IUserManager userManager,
    ISessionManager sessionManager,
    ILogger logger,
    bool authenticate = true) : IMessageDispatcher
{
    private readonly IElementManager _elementManager = elementManager ?? throw new ArgumentNullException(nameof(elementManager));
    private readonly IUserManager _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly ISessionManager _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public Message HandleRequest(Message message)
    {
        ExceptionHelpers.ThrowIfNull(message, nameof(message));

        _logger.Debug($"Request: address={message.Address}, data={message.Data}");

        Message response;
        try
        {
            var serviceElement = GetServiceElement(message.Address);
            if (!AuthorizeUser(message.Authentication, serviceElement))
            {
                throw new AccessDeniedException();
            }
            response = InvokeService(serviceElement, message);
        }
        catch (IoTCoreException e)
        {
            _logger.Error($"Service {message.Address} failed. {e.Message} {e.ResponseCode} {e.ErrorDetails}");

            response = new Message(e.ResponseCode,
                message.Cid,
                message.Reply ?? message.Address,
                Variant.FromObject(new ErrorInfoResponseServiceData(e.Message, e.ErrorCode, e.ErrorDetails, e.HexError)));
        }

        _logger.Debug($"Response: {response.Address} code={response.Code}, data={response.Data}");

        return response;
    }

    public void HandleEvent(Message message)
    {
        ExceptionHelpers.ThrowIfNull(message, nameof(message));

        _logger.Debug($"Event: address={message.Address}, data={message.Data}");

        try
        {
            var serviceElement = GetServiceElement(message.Address);
            InvokeService(serviceElement, message);
        }
        catch (IoTCoreException e)
        {
            _logger.Error($"Service {message.Address} failed. {e.Message} {e.ResponseCode} {e.ErrorDetails}");
        }
    }

    private bool AuthorizeUser(Message.AuthenticationInfo authenticationInfo, IServiceElement serviceElement)
    {
        if (!authenticate || !_userManager.IsAuthenticationRequired) 
        { 
            return true; 
        }

        if (authenticationInfo == null)
        {
            return serviceElement.AccessRestrictions == AccessRestrictions.None || serviceElement.AccessControl.UserRoles.HasFlag(UserRole.Anonymous);
        }

        if (authenticationInfo.Token != null)
        {
            var session = _sessionManager.GetSession(authenticationInfo.Token);
            return serviceElement.AccessControl.UserRoles.HasFlag(session.User.Role);
        }

        if (authenticationInfo.User != null)
        {
            var result = _userManager.Authenticate(authenticationInfo.User, authenticationInfo.Password);
            switch (serviceElement.AccessRestrictions)
            {
                case AccessRestrictions.None:
                    return true;
                case AccessRestrictions.InitialPassword:
                    if (result is AuthenticationResult.ChangePassword or AuthenticationResult.Success) return true;
                    break;
                case AccessRestrictions.ChangedPassword:
                    if (result is AuthenticationResult.Success) return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return false;
    }

    private IServiceElement GetServiceElement(string address)
    {
        var element = _elementManager.GetElementByAddress(address);
        if (element == null)
        {
            throw new NotFoundException($"Element '{address}' not found");
        }
        return element as IServiceElement ?? throw new BadRequestException($"Element '{address}' is not a service");
    }

    private static Message InvokeService(IServiceElement serviceElement, Message message)
    {
        try
        {
            var serviceInfo = new ServiceInfo(message.Cid, message.Authentication?.Token);
            var data = serviceElement.Invoke(message.Data, serviceInfo);
            return new Message(serviceInfo.ResponseCode ?? ResponseCodes.Success, message.Cid, message.Reply ?? message.Address, data);
        }
        catch (IoTCoreException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new IoTCoreException(ResponseCodes.InternalError, $"Service '{serviceElement.Address}' failed", e.Message);
        }
    }
}