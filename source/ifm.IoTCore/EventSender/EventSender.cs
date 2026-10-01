namespace ifm.IoTCore.EventSender;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ifm.Common;
using ifm.Common.Variant;
using Logger.Contracts;

using Common;
using Contracts;
using Message;
using NetAdapter.Contracts.Server;
using NetAdapterManager.Contracts;

public class EventSender : DisposableBase, IEventSender
{
    private readonly IClientNetAdapterManager _clientNetAdapterManager;
    private readonly IServerNetAdapterManager _serverNetAdapterManager;
    private readonly ILogger _logger;

    private readonly BlockingRingBuffer<Tuple<string, EventMessageDataBase>> _messageDataQueue = new(1000);

    private readonly CancellationTokenSource _cts = new();
    private int _isStarted;

    private Task _senderTask;
    private int _eventNumber;

    public EventSender(IClientNetAdapterManager clientNetAdapterManager, IServerNetAdapterManager serverNetAdapterManager, ILogger logger)
    {
        _clientNetAdapterManager = clientNetAdapterManager ?? throw new ArgumentNullException(nameof(clientNetAdapterManager));
        _serverNetAdapterManager = serverNetAdapterManager ?? throw new ArgumentNullException(nameof(serverNetAdapterManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        if (Interlocked.CompareExchange(ref _isStarted, 0, 1) != 1) return;
        _cts.Cancel();
        Stop();
    }

    public void SendEvent(string targetUrl, EventMessageDataBase data)
    {
        _messageDataQueue.Add(new Tuple<string, EventMessageDataBase>(targetUrl, data));
    }

    private void Start()
    {
        try
        {
            _senderTask = Task.Run(SendMessages, _cts.Token);
            Interlocked.Increment(ref _isStarted);
        }
        catch (TaskCanceledException)
        {
            // Normal on cancellation. Ignore
        }
    }

    private void Stop()
    {
        try
        {
            _senderTask.Wait(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (AggregateException)
        {
            // Ignore
        }
        catch (ObjectDisposedException)
        {
            // Ignore
        }
    }


    private void SendMessages()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var messageData = _messageDataQueue.Take(_cts.Token);
                messageData.Item2.Number = ++_eventNumber;
                try
                {
                    _logger.Debug($"Sending event message to {messageData.Item1}");

                    if (TrySendFromServerToConnectedClient(messageData.Item1, messageData.Item2)) continue;
                    if (!TrySendFromClientToServer(messageData.Item1, messageData.Item2))
                    {
                        _logger.Error($"Send event message to {messageData.Item1} failed: No suitable method to send available");
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"Send event to {messageData.Item1} failed: {e.Message}");
                }
            }
            catch (OperationCanceledException)
            {
                // Normal on stop, ignore
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }

    private bool TrySendFromServerToConnectedClient(string targetUrl, EventMessageDataBase messageData)
    {
        // If authority is provided event is not for a connected client
        if (targetUrl.IndexOf("://", StringComparison.Ordinal) != -1) return false;
        var scheme = targetUrl.Left(':');
        if (scheme == null) return false;
        var target = targetUrl.Right(':');
        if (target == null) return false;
        var path = target.Left('?');
        var query = target.Right('?');
        if (query == null) return false;

        var clientId = HttpUtility.ParseQueryString(query).Get("clientid");
        if (clientId == null) return false;

        var servers = _serverNetAdapterManager.FindServerNetAdapters(scheme);
        foreach (var server in servers)
        {
            if (server is not IConnectedServerNetAdapter connectedServer) continue;
            if (!connectedServer.IsClientConnected(clientId)) continue;
            var eventMessage = new Message(RequestCodes.Event, messageData.SubscribeId, path, Variant.FromObject(messageData));
            connectedServer.SendEvent(clientId, eventMessage);
            return true;
        }
        return false;
    }

    private bool TrySendFromClientToServer(string targetUrl, EventMessageDataBase messageData)
    {
        if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out var uri)) return false;
        var eventMessage = new Message(RequestCodes.Event, messageData.SubscribeId, uri.LocalPath, Variant.FromObject(messageData));
        var client = _clientNetAdapterManager.CreateClientNetAdapter(new Uri(targetUrl));
        client.SendEvent(eventMessage);
        return true;
    }
}