namespace ifm.IoTCore.UserManager.Contracts;

using System.Collections.Generic;
using Common;

/// <summary>
/// Represents the outcome of an authentication attempt.
/// </summary>
public enum AuthenticationResult
{
    /// <summary>Authentication failed, name or password invalid.</summary>
    Failed,
    /// <summary>Authentication succeeded, name and password valid, but password must be changed.</summary>
    ChangePassword,
    /// <summary>Authentication succeeded, name and password valid.</summary>
    Success
}

/// <summary>
/// Provides functionality to interact with the user manager.
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Add a new user.
    /// </summary>
    /// <param name="user">The user information.</param>
    void AddUser(User user);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="userName">The name of the user.</param>
    void DeleteUser(string userName);

    /// <summary>
    /// Gets a user by name.
    /// </summary>
    /// <param name="userName">The name of the user.</param>
    /// <returns>The user.</returns>
    User GetUser(string userName);

    /// <summary>
    /// Gets all users.
    /// </summary>
    List<User> Users { get; }

    /// <summary>
    /// Change the password of the user.
    /// </summary>
    /// <param name="userName">The name of the user.</param>
    /// <param name="oldPassword">The old password of the user.</param>
    /// <param name="newPassword">The new password of the user.</param>
    void ChangePassword(string userName, string oldPassword, string newPassword);

    /// <summary>
    /// Gets the change password value of the build-in user.
    /// </summary>
    bool IsInitialPassword { get; }

    /// <summary>
    /// Changes the password of the build-in user.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    void ChangePassword(string password);

    /// <summary>
    /// Gets if authentication is required.
    /// </summary>
    bool IsAuthenticationRequired { get; set; }

    /// <summary>
    /// Authenticates the user authentication information.
    /// </summary>
    /// <param name="userName">The name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns>The authentication result.</returns>
    AuthenticationResult Authenticate(string userName, string password);
}