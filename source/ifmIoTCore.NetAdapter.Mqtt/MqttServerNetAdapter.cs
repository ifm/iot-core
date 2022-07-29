using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.NetAdapter.Mqtt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using Common;
    using Elements;
    using Elements.ServiceData.Responses;
    using Exceptions;
    using Messages;
    using Logger;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Disconnecting;
    using MQTTnet.Client.Publishing;
    using MQTTnet.Client.Receiving;
    using MQTTnet.Client.Subscribing;
    using MQTTnet.Client.Unsubscribing;
    using MQTTnet.Exceptions;
    using ProfileBuilders.Mqtt;
    using Utilities;

    /// <summary>
    /// This class implements the server role of the mqtt net adapter.
    /// </summary>
    public sealed class MqttServerNetAdapter : ServerNetAdapterBase
    {
        public const string ProtocolName = "mqtt";

        private readonly ILogger _logger;
        private readonly IMessageConverter _converter;
        
        private readonly MqttConnectionProfileBuilder _connectionProfileBuilder;
        private MqttClient _mqttClient;
        private readonly Action<Exception> _disconnectionHandler;
        private IDeviceElement _deviceElement;
        private readonly IElementManager _elementManager;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of <see cref="MqttServerNetAdapter"/>.
        /// </summary>
        /// <param name="elementManager">The element manager</param>
        /// <param name="deviceElement">The root element if the iotcore which is represented by this netadapter.</param>
        /// <param name="converter">The converter to use for serialization of the messages.</param>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> of the broker to communicate with.</param>
        /// <param name="disconnectionHandler">A handler message in case of disruption of the connection.</param>
        /// <param name="commandTopic">The commandtopic to listen for incoming commands for.</param>
        /// <param name="logger">The logger instance to use for logging.</param>
        public MqttServerNetAdapter(IElementManager elementManager, IDeviceElement deviceElement, IMessageConverter converter, IPEndPoint endPoint, Action<Exception> disconnectionHandler = null, string commandTopic = "cmdTopic/#", ILogger logger = null)
        {
            this._elementManager = elementManager;
            this._deviceElement = deviceElement;

            MqttHelper.EnsureDefaultMqttSetup(_elementManager, this._deviceElement);

            this._converter = converter;

            this._disconnectionHandler = disconnectionHandler;
            this._logger = logger ?? new NullLogger();

            this._connectionProfileBuilder = new MqttConnectionProfileBuilder(_elementManager, deviceElement, this, endPoint, commandTopic, logger ?? new NullLogger());
            this._connectionProfileBuilder.RunControlEvent += this.OnRunControlEvent;
            this._connectionProfileBuilder.PropertyChanging += this.ConnectionProfileBuilderOnPropertyChanging;
            this._connectionProfileBuilder.PropertyChanged += this.ConnectionProfileBuilderOnPropertyChanged;
            this._connectionProfileBuilder.Build();

            this._mqttClient = MqttHelper.BuildMqttClient();
            this._mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(this.MessageReceivedEventHandler);
            this._mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(this.DisconnectedHandler);
        }

        /// <summary>
        /// Gets the IConverter type for the data format, which the network adapter server implements .
        /// </summary>
        public override string Scheme => ProtocolName;

        public override string Format => _converter.Type;

        /// <summary>
        /// Gets the uri of the network adapter server.
        /// </summary>
        public override Uri Uri => this._connectionProfileBuilder.Uri;

        public override bool IsListening => throw new NotImplementedException();

        public string CurrentClientId => this._mqttClient?.Options?.ClientId;
        public string CommandTopic => this._connectionProfileBuilder.CommandTopic;
        public string ReplyTopic => this._connectionProfileBuilder.ReplyTopic;
        public event EventHandler<MqttServerNetAdapterState> StateChanged; 

        /// <summary>
        /// Disposes of all used resources.
        /// </summary>
        public override void Dispose()
        {
            if (this._isDisposed) return;

            this._connectionProfileBuilder.RunControlEvent -= this.OnRunControlEvent;
            this._connectionProfileBuilder.PropertyChanged -= this.ConnectionProfileBuilderOnPropertyChanged;
            this._connectionProfileBuilder.PropertyChanging -= this.ConnectionProfileBuilderOnPropertyChanging;
            this._connectionProfileBuilder.Dispose();
            
            this._mqttClient?.Dispose();
            this._mqttClient = null;

            this._isDisposed = true;
        }

        /// <summary>
        /// Starts the network adapter server.
        /// </summary>
        public override void Start()
        {
            var task = this.StartAsync();
            task.Wait();
        }

        /// <summary>
        /// Starts the network adapter server asynchronous.
        /// </summary>
        public override Task StartAsync()
        {
            if (this._isDisposed) return Task.FromException(new ObjectDisposedException(nameof(MqttServerNetAdapter)));

            return Task.Run(async () =>
            {
                if (!this._mqttClient.IsConnected) await this.ConnectAsync();
                await this.SubscribeAsync();
                this.RaiseStateChanged(MqttServerNetAdapterState.Started);
            });
        }

        /// <summary>
        /// Stops the network adapter server.
        /// </summary>
        public override void Stop()
        {
            var task = this.StopAsync();
            task.Wait();
        }

        /// <summary>
        /// Stops the network adapter server asynchronously.
        /// </summary>
        public override Task StopAsync()
        {
            return Task.Run(async () =>
            {
                await this.UnsubscribeAsync();
                await this.DisconnectAsync();
                this.RaiseStateChanged(MqttServerNetAdapterState.Stopped);
            });
        }

        /// <summary>
        /// Subscribes to the command topic, to listen for incoming messages.
        /// </summary>
        /// <param name="throwOnFail">if <c>true</c> this method will throw an <see cref="AggregateException"/> when the subscription fails.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        private async Task SubscribeAsync(bool throwOnFail = false)
        {
            if (this._isDisposed) throw new ObjectDisposedException(nameof(MqttServerNetAdapter));

            try
            {
                var level = this._connectionProfileBuilder.GetQualityOfService();
                var subscribeResult = await this._mqttClient.SubscribeAsync(this.CommandTopic, level);

                var messages = new List<string>();

                foreach (var item in subscribeResult.Items)
                {
                    if (item.ResultCode != MqttClientSubscribeResultCode.GrantedQoS0 &&
                        item.ResultCode != MqttClientSubscribeResultCode.GrantedQoS1 &&
                        item.ResultCode != MqttClientSubscribeResultCode.GrantedQoS2)
                    {
                        var errorMessage = $"Could not subscribe to '{this.CommandTopic}'. The resultcode was: '{item.ResultCode}'.";
                        this._logger.Error(errorMessage);

                        messages.Add(errorMessage);
                    }
                }

                if (messages.Count > 0 && throwOnFail)
                {
                    var invalidOperationExceptions = messages.Select(x => new InvalidOperationException(x));
                    throw new AggregateException(invalidOperationExceptions);
                }
            }
            catch (MqttCommunicationException e)
            {
                this._logger.Error(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Unsubscribes to the command topic, to stop listening for incoming messages.
        /// </summary>
        /// <param name="throwOnFail">if <c>true</c> this method will throw an <see cref="AggregateException"/> when the unsubscription fails.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        private async Task UnsubscribeAsync(bool throwOnFail = false)
        {
            if (this._isDisposed) throw new ObjectDisposedException(nameof(MqttServerNetAdapter));
            if (this._mqttClient == null ) return;

            if (!this._mqttClient.IsConnected) return;

            try
            {
                var unsubscribeResult = await this._mqttClient.UnsubscribeAsync(this.CommandTopic);
                var messages = new List<string>();

                foreach (var item in unsubscribeResult.Items)
                {
                    if (item.ReasonCode == MqttClientUnsubscribeResultCode.Success)
                    {
                        continue;
                    }

                    var errorMessage = $"Could not unsubscribe from topic '{this.CommandTopic}'. The code was '{item.ReasonCode}'";
                    this._logger.Error(errorMessage);

                    if (throwOnFail)
                    {
                        messages.Add(errorMessage);
                    }
                }

                if (messages.Count > 0 && throwOnFail)
                {
                    var invalidOperationExceptions = messages.Select(x => new InvalidOperationException(x));
                    throw new AggregateException(invalidOperationExceptions);
                }
            }
            catch (MqttCommunicationException e)
            {
                this._logger.Error(e.Message);
                throw;
            }
        }

        private async Task MessageReceivedEventHandler(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            if (this._isDisposed) return;
            if (!this._mqttClient.IsConnected) return;

            try
            {
                if (eventArgs?.ApplicationMessage?.Payload == null ||
                    eventArgs.ApplicationMessage?.Payload.Length == 0)
                {
                    throw new IoTCoreException(ResponseCodes.DataInvalid, "The received message is empty or null.");
                }

                var messagePayload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

                var message = this._converter.Deserialize(messagePayload);
                switch (message.Code)
                {
                    case RequestCodes.Request:
                    {
                        var requestEventArgs = new RequestMessageEventArgs(message);
                        this.RaiseRequestReceived(requestEventArgs);

                        await this.PublishResponseMessageAsync(requestEventArgs.ResponseMessage);
                        break;
                    }
                    case RequestCodes.Event:
                    {
                        this.RaiseEventReceived(message);

                        break;
                    }
                }
            }
            catch (IoTCoreException serviceException)
            {
                var errorResponseMessage = new Message(ResponseCodes.InternalError, 0, string.Empty,
                    null); //Helpers.ToJson(new ErrorInfoResponseServiceData(serviceException.Message)));
                await this.PublishResponseMessageAsync(errorResponseMessage);
            }
            catch (Exception exception)
            {
                var errorResponseMessage = new Message(ResponseCodes.InternalError, 0, string.Empty, (VariantValue)exception.Message);
                await this.PublishResponseMessageAsync(errorResponseMessage);
            }
        }

        private async Task PublishResponseMessageAsync(Message message)
        {
            if (this._isDisposed) return;

            var serializedMessage = this._converter.Serialize(message);
            var payload = Encoding.UTF8.GetBytes(serializedMessage);
            await this.PublishResponseAsync(this._connectionProfileBuilder.ReplyTopic, payload);
        }

        private async Task ConnectAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(MqttServerNetAdapter));

            try
            {
                var connectionResult = await this._mqttClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Parse(this.Uri.Host), this.Uri.Port), CancellationToken.None);
                if (connectionResult.ResultCode != MqttClientConnectResultCode.Success)
                {
                    throw new InvalidOperationException($"Could not connect the broker. The reason was {connectionResult.ResultCode} {connectionResult.ReasonString}");
                }   
            }
            catch (MqttCommunicationException communicationException)
            {
                this._logger.Error(communicationException.Message);
                throw;
            }
        }

        private async Task DisconnectAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(MqttServerNetAdapter));
            
            try
            {
                await this._mqttClient.DisconnectAsync();
            }
            catch (MqttCommunicationException communicationException)
            {
                this._logger.Error(communicationException.Message);
                throw;
            }
        }

        private async Task PublishResponseAsync(string replyTopic, byte[] payload)
        {
            if (this._isDisposed) throw new ObjectDisposedException(nameof(MqttServerNetAdapter));

            if (!this._mqttClient.IsConnected)
            {
                return;
            }

            try
            {
                var publishResult = await this._mqttClient.PublishAsync(new MqttApplicationMessage
                {
                    ContentType = this._converter.Type,
                    Payload = payload,
                    Topic = replyTopic,
                    QualityOfServiceLevel = this._connectionProfileBuilder.GetQualityOfService()
                });

                if (publishResult.ReasonCode != MqttClientPublishReasonCode.Success)
                {
                    this._logger.Error($"Could not publish reply. The errorcode was '{publishResult.ReasonCode}'. The message was '{publishResult.ReasonString}'.");
                }
            }
            catch (Exception exception)
            {
                this._logger.Error($"Could not send publish message. The errormessage was '{exception.Message}'.");
            }
        }

        private void DisconnectedHandler(MqttClientDisconnectedEventArgs eventArgs)
        {
            this.RaiseStateChanged(MqttServerNetAdapterState.Stopped);
            this._logger.Warning("Mqtt client disconnected.");
            this._disconnectionHandler?.Invoke(eventArgs?.Exception);
        }

        private void OnRunControlEvent(object sender, RunControlEventType e)
        {
            switch (e)
            {
                case RunControlEventType.Started:
                    this.Start();
                    break;
                case RunControlEventType.Stopped:
                    this.Stop();
                    break;
                case RunControlEventType.Reset:
                    this.Stop();
                    this.Start();
                    break;
                case RunControlEventType.Suspend:
                    this.Stop();
                    break;
            }
        }

        private void ConnectionProfileBuilderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(MqttCommIfSetupProfileBuilder.Qos)) return;
            try
            {
                this.SubscribeAsync().GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                this._logger.Error(exception.Message);
            }
        }

        private void ConnectionProfileBuilderOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName != nameof(MqttCommIfSetupProfileBuilder.Qos)) return;
            try
            {
                this.UnsubscribeAsync().GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                this._logger.Error(exception.Message);
            }
        }

        private void RaiseStateChanged(MqttServerNetAdapterState state)
        {
            try
            {
                this.StateChanged.Raise(this, state, true);
            }
            catch (Exception e)
            {
                this._logger.Error(e.Message);

                if (e is AggregateException aggregateException)
                {
                    foreach (var exception in aggregateException.InnerExceptions)
                    {
                        this._logger.Error(exception.Message);
                    }
                }
            }
        }   
    }
}