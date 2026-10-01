namespace ifm.IoTCore.UserManager.Contracts;

using System;
using Common;

/// <summary>
/// Provides functionality to interact with a session.
/// </summary>
public interface ISession
{
    /// <summary>
    /// The user for the session.
    /// </summary>
    public User User { get; }

    /// <summary>
    /// Time when session was last active.
    /// </summary>
    public DateTime LastActive { get; }
}

/// <summary>
/// Provides functionality to interact with the session manager.
/// </summary>
public interface ISessionManager
{
    /// <summary>
    /// Creates a user session.
    /// </summary>
    /// <param name="user">The user for the session.</param>
    /// <returns>The session token.</returns>
    string CreateSession(User user);

    /// <summary>
    /// Deletes the user session.
    /// </summary>
    /// <param name="sessionToken">The session token.</param>
    void DeleteSession(string sessionToken);

    /// <summary>
    /// Gets the user session.
    /// </summary>
    /// <param name="sessionToken">The session token.</param>
    /// <returns>The user session.</returns>
    ISession GetSession(string sessionToken);
}