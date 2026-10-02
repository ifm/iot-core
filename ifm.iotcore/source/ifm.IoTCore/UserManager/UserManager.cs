namespace ifm.IoTCore.UserManager;

using System;
using System.Collections.Generic;
using System.Linq;
using ifm.Common;
using DataStore.Contracts;

using Common;
using Common.Exceptions;
using Contracts;

public class UserManager : IUserManager
{
    private class Settings
    {
        public bool EnableAuthentication { get; set; }
        public User Administrator { get; set; }
        public List<User> Users { get; set; }
    }

    private readonly IDataStore _dataStore;
    private const string Section = "user-manager";
    private const string Key = "settings";
    private readonly Settings _settings;
    private readonly object _lock = new();

    public UserManager(IDataStore dataStore, bool enableAuthentication, string userName, string password, bool changePassword)
    {
        ExceptionHelpers.ThrowIfNull(dataStore, nameof(dataStore));
        ExceptionHelpers.ThrowIfNullOrEmpty(userName, nameof(userName));

        _dataStore = dataStore;
        _settings = _dataStore.Get<Settings>(Section, Key) ?? new Settings
        {
            EnableAuthentication = enableAuthentication
        };
        _settings.Administrator ??= new User(userName, UserRole.Administrator, password, changePassword);
        _settings.Users ??= [];
    }

    public void AddUser(User user)
    {
        ExceptionHelpers.ThrowIfNull(user, nameof(user));
        ExceptionHelpers.ThrowIfNullOrEmpty(user.Name, nameof(user.Name));

        lock (_lock)
        {
            if (_settings.Administrator.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase) || _settings.Users.FirstOrDefault(x => x.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)) != null) throw new DataInvalidException("User already exists");
            _settings.Users.Add(user);
            _dataStore.Set(Section, Key, _settings);
        }
    }

    public void DeleteUser(string userName)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(userName, nameof(userName));

        lock (_lock)
        {
            var user = _settings.Users.FirstOrDefault(x => x.Name.Equals(userName, StringComparison.OrdinalIgnoreCase)) ?? throw new DataInvalidException("User not found");
            _settings.Users.Remove(user);
            _dataStore.Set(Section, Key, _settings);
        }
    }

    public User GetUser(string userName)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(userName, nameof(userName));

        lock (_lock)
        {
            if (_settings.Administrator.Name.Equals(userName, StringComparison.OrdinalIgnoreCase)) return _settings.Administrator;
            return _settings.Users.FirstOrDefault(x => x.Name.Equals(userName, StringComparison.OrdinalIgnoreCase)) ?? throw new DataInvalidException("User not found");
        }
    }

    public List<User> Users
    {
        get
        {
            lock (_lock)
            {
                // Take a snapshot
                var items = new List<User>
                {
                    _settings.Administrator
                };
                items.AddRange(_settings.Users);
                return items;
            }
        }
    }

    public void ChangePassword(string userName, string oldPassword, string newPassword)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(userName, nameof(userName));

        lock (_lock)
        {
            if (_settings.Administrator.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                _settings.Administrator.Password = newPassword;
                _settings.Administrator.ChangePassword = false;
                _dataStore.Set(Section, Key, _settings);
                return;
            }
            var user = _settings.Users.FirstOrDefault(x => x.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (user == null || user.Password != oldPassword) throw new DataInvalidException("Invalid user name or password");
            user.Password = newPassword;
            user.ChangePassword = false;
            _dataStore.Set(Section, Key, _settings);
        }
    }

    public bool IsInitialPassword
    {
        get
        {
            lock (_lock)
            {
                return _settings.Administrator.ChangePassword;
            }
        }
    }

    public void ChangePassword(string password)
    {
        lock (_lock)
        {
            _settings.Administrator.Password = password;
            _settings.Administrator.ChangePassword = false;
            _dataStore.Set(Section, Key, _settings);
        }
    }

    public bool IsAuthenticationRequired
    {
        get
        {
            lock (_lock)
            {
                return _settings.EnableAuthentication;
            }
        }
        set
        {
            lock (_lock)
            {
                if (_settings.EnableAuthentication == value) return;
                _settings.EnableAuthentication = value;
                _dataStore.Set(Section, Key, _settings);
            }
        }
    }

    public AuthenticationResult Authenticate(string userName, string password)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(userName, nameof(userName));

        lock (_lock)
        {
            if (_settings.Administrator.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                if (_settings.Administrator.Password != password) return AuthenticationResult.Failed;
                return _settings.Administrator.ChangePassword ? AuthenticationResult.ChangePassword : AuthenticationResult.Success;
            }
            var user = _settings.Users.FirstOrDefault(x => x.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (user == null || user.Password != password) return AuthenticationResult.Failed;
            return user.ChangePassword ? AuthenticationResult.ChangePassword : AuthenticationResult.Success;
        }
    }
}
