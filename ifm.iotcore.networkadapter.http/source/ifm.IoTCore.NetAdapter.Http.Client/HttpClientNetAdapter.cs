namespace ifm.IoTCore.NetAdapter.Http.Client;

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using ifm.Common;
using Logger.Contracts;
using Common;
using Common.Exceptions;
using NetAdapter.Contracts.Client;
using Message;
using MessageConverter.Contracts;

public class HttpClientNetAdapter : IClientNetAdapter
{
    private readonly IMessageConverter _converter;
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    private readonly BlockingRingBuffer<Tuple<string, Message>> _messageDataQueue = new(1000);
    private readonly CancellationTokenSource _cts = new();
    private int _isStarted;

    private Task _senderTask;

    public HttpClientNetAdapter(Uri remoteUri,
        IMessageConverter converter,
        TimeSpan timeout,
        bool keepAlive,
        HttpClientHandler handler = null,
        ILogger logger = null)
    {
        RemoteUri = remoteUri;
        _converter = converter;
        _client = handler == null ? new HttpClient { Timeout = timeout } : new HttpClient(handler) { Timeout = timeout };
        _logger = logger;

        if (keepAlive)
        {
            _client.DefaultRequestHeaders.ConnectionClose = false;
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        LastUsed = DateTime.Now;
        Start();
    }

    public DateTime LastUsed { get; private set; }

    public Uri RemoteUri { get; }

    public Message SendRequest(Message requestMessage)
    {
        LastUsed = DateTime.Now;
        var requestString = _converter.Serialize(requestMessage);

        try
        {
            _logger?.Debug($"HttpClient: {RemoteUri}: Request: {requestString}");

            var httpResponse = _client.Send(new HttpRequestMessage(HttpMethod.Post, RemoteUri)
            {
                Content = CreateStringContent(requestString, _converter.ContentType)
            });

            var responseStream = httpResponse.Content.ReadAsStream();
            using var reader = new StreamReader(responseStream);
            var responseString = reader.ReadToEnd();

            _logger?.Debug($"HttpClient: {RemoteUri}: Response: {responseString}");

            return _converter.Deserialize(responseString);
        }
        catch (HttpRequestException e)
        {
            _logger?.Error($"HttpClient: {RemoteUri}: {e.Message}");
            throw new IoTCoreException(ResponseCodes.Timeout, e.Message, e.InnerException?.Message);
        }
        catch (Exception e)
        {
            if (e.InnerException is System.TimeoutException)
            {
                _logger?.Error($"HttpClient: {RemoteUri}: {e.InnerException?.Message}");
                throw new IoTCoreException(ResponseCodes.Timeout, e.InnerException?.Message, e.InnerException?.Message);
            }

            _logger?.Error($"HttpClient: {RemoteUri}: {e.Message}");
            throw new IoTCoreException(ResponseCodes.InternalError, e.Message, e.InnerException?.Message);
        }
    }

    public void SendEvent(Message eventMessage)
    {
        LastUsed = DateTime.Now;
        _messageDataQueue.Add(new Tuple<string, Message>(RemoteUri.ToString(), eventMessage));
    }

    public void Dispose()
    {
        _client.Dispose();
        if (Interlocked.CompareExchange(ref _isStarted, 0, 1) != 1) return;
        _cts.Cancel();
        Stop();
    }

    private static StringContent CreateStringContent(string content, string contentType)
    {
        var stringContent = new StringContent(content, Encoding.UTF8, contentType);
        if (contentType.ToLowerInvariant() == "application/json")
        {
            stringContent.Headers.Remove("Content-Type");
            stringContent.Headers.Add("Content-Type", contentType);
        }
        return stringContent;
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

                try
                {
                    _logger?.Debug($"Sending event message to {messageData.Item1}");

                    var eventString = _converter.Serialize(messageData.Item2);

                    try
                    {
                        _client.Send(new HttpRequestMessage(HttpMethod.Post, messageData.Item1)
                        {
                            Content = CreateStringContent(eventString, _converter.ContentType)
                        });
                    }
                    catch (HttpRequestException e)
                    {
                        throw new IoTCoreException(ResponseCodes.Timeout, e.Message, e.InnerException?.Message);
                    }
                    catch (Exception e)
                    {
                        throw new IoTCoreException(ResponseCodes.InternalError, e.Message, e.InnerException?.Message);
                    }

                }
                catch (Exception e)
                {
                    _logger?.Error($"Send event to {messageData.Item1} failed: {e.Message}");
                }
            }
            catch (OperationCanceledException)
            {
                // Normal on stop, ignore
            }
            catch (Exception e)
            {
                _logger?.Error(e.Message);
            }
        }
    }


}