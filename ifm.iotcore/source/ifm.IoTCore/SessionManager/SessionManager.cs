namespace ifm.IoTCore.SessionManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Exceptions;
using DataStore.Contracts;
using ifm.Common;
using UserManager.Contracts;

internal class Settings(uint timeout)
{
    public const uint TimeoutMin = 1;
    public const uint TimeoutMax = 43200; // 30 days
    public const uint TimeoutDefault = 15;

    public Settings() : this(TimeoutDefault)
    {
    }

    public uint Timeout
    {
        get => _timeout;
        set
        {
            if (_timeout == value) return;
            if (value is < TimeoutMin or > TimeoutMax)
            {
                throw new DataOutOfRangeException(detailsMessage: nameof(Timeout));
            }
            _timeout = value;
        }
    }
    private uint _timeout = timeout;
}

internal class Session(User user) : ISession
{
    public User User { get; } = user;
    public DateTime LastActive { get; set; }
}

public class SessionManager : ISessionManager, IDisposable
{
    private readonly Dictionary<string, Session> _sessions = new();
    private readonly object _lock = new();

    private readonly CancellationTokenSource _cts = new();
    private readonly Task _cleanupTask;

    private readonly Settings _settings;

    public SessionManager(IDataStore dataStore)
    {
        _settings = dataStore.Get<Settings>("iotcore.session-manager", "settings") ?? new Settings();
        _cleanupTask = Task.Factory.StartNew(CleanupInactiveSessions, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public string CreateSession(User user)
    {
        ExceptionHelpers.ThrowIfNull(user, nameof(user));

        lock (_lock)
        {
            var sessionToken = _sessions.FirstOrDefault(x => x.Value.User == user).Key;
            if (sessionToken == null)
            {
                sessionToken = CreateSessionToken(user.Name);
                _sessions.Add(sessionToken, new Session(user));
            }
            return sessionToken;
        }
    }

    public void DeleteSession(string sessionToken)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(sessionToken, nameof(sessionToken));

        lock (_lock)
        {
            if (!_sessions.Remove(sessionToken)) throw new DataInvalidException($"Token {sessionToken} does not exist");
        }
    }

    public ISession GetSession(string sessionToken)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(sessionToken, nameof(sessionToken));

        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionToken, out var session)) throw new DataInvalidException($"Token {sessionToken} does not exist");
            session.LastActive = DateTime.Now;
            return session;
        }
    }

    private static string CreateSessionToken(string userName)
    {
        // Add timestamp to prevent token reuse
        var payload = $"{userName}|{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        // Convert payload and secret key to bytes
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var keyBytes = "Hexen&fett"u8.ToArray();

        // Sign the payload
        using var hmac = new HMACSHA256(keyBytes);
        var signatureBytes = hmac.ComputeHash(payloadBytes);
        var signature = Convert.ToBase64String(signatureBytes);

        // Final token format: Base64(payload) + "." + Base64(signature)
        return $"{Convert.ToBase64String(payloadBytes)}.{signature}";
    }

    private void CleanupInactiveSessions()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (_cts.Token.WaitHandle.WaitOne(60000)) continue;
            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    if (DateTime.Now - session.Value.LastActive > TimeSpan.FromMinutes(_settings.Timeout))
                    {
                        _sessions.Remove(session.Key);
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cleanupTask.Wait();
    }
}
