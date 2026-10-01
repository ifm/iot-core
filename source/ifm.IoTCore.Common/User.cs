using System;

namespace ifm.IoTCore.Common;

/// <summary>
/// Specifies the user roles.
/// </summary>
[Flags]
public enum UserRole
{
    /// <summary>The administrator user role.</summary>
    Administrator = 1,
    /// <summary>The maintainer user role.</summary>
    Maintainer = 2,
    /// <summary>The observer user role.</summary>
    Observer = 3,
    /// <summary>The anonymous user role.</summary>
    Anonymous = 4
}

/// <summary>
/// Represents a user.
/// </summary>
/// <param name="name">The name.</param>
/// <param name="role">The role.</param>
/// <param name="password">The password.</param>
/// <param name="changePassword">If true the password must be changed; otherwise not.</param>
public class User(string name,
    UserRole role,
    string password,
    bool changePassword)
{
    /// <summary>The name.</summary>
    public string Name { get; } = name;

    /// <summary>The role.</summary>
    public UserRole Role { get; } = role;

    /// <summary>The password.</summary>
    public string Password { get; set; } = password;

    /// <summary>If true the password must be changed; otherwise not.</summary>
    public bool ChangePassword { get; set; } = changePassword;
}

