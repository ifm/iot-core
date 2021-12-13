using System.Linq;
using MQTTnet.Client.Connecting;

namespace ifmIoTCore.NetAdapter.Mqtt
{
    using System;
    using System.Net;
    using System.Threading;
    using Messages;
    using MQTTnet.Client;

    /// <summary>
    /// This class implements the client role of the mqtt net adapter.
    /// </summary>
    public class MqttClientNetAdapter : IClientNetAdapter
    {
        private readonly IConverter _converter;
        private readonly IPEndPoint _ipEndPoint;
        private readonly string _topic;
        private readonly TimeSpan _defaultTimeout;
        private MqttClient _client;

        /// <summary>
        /// Initializes a new instance of <see cref="MqttClientNetAdapter"/>.
        /// </summary>
        /// <param name="topic">The topic on which to publish the outgoing messages.</param>
        /// <param name="ipEndPoint">The <see cref="IPEndPoint"/> of the broker, to which to communicate.</param>
        /// <param name="converter">The converter to use for serialization of the messages.</param>
        /// <param name="defaultTimeout">The communication timeout.</param>
        public MqttClientNetAdapter(string topic, IPEndPoint ipEndPoint, IConverter converter, TimeSpan defaultTimeout)
        {
            this._topic = topic;
            this._ipEndPoint = ipEndPoint;
            this._converter = converter;
            this._defaultTimeout = defaultTimeout;
        }

        /// <summary>
        /// Provides a timestamp, when this netadapter was recently used.
        /// </summary>
        public DateTime LastUsed { get; private set; }

        protected MqttClient Client
        {
            get
            {
                if (this._client != null)
                {
                    return this._client;
                }

                
                this._client = MqttHelper.BuildMqttClient();
                return this._client;
            }
        }

        /// <summary>
        /// Disconnects the client from the broker.
        /// </summary>
        public void Disconnect()
        {
            this._client?.DisconnectAsync().GetAwaiter().GetResult();
            this._client?.Dispose();
            this._client = null;
        }

        /// <summary>
        /// Frees all used resources.
        /// </summary>
        public void Dispose()
        {
            this._client?.Dispose();
        }

        /// <summary>
        /// Sends a request to the broker.
        /// </summary>
        /// <param name="requestMessage">The request message</param>
        /// <param name="timeout">The timeout to wait for a response</param>
        /// <returns>The response message</returns>
        public ResponseMessage SendRequest(RequestMessage requestMessage, TimeSpan? timeout)
        {
            var result = this.SendMessage(requestMessage, timeout ?? _defaultTimeout);
            return new ResponseMessage(result.Code, result.Cid, result.Address, result.Data);
        }

        /// <summary>
        /// Sends an event to the broker.
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        public void SendEvent(EventMessage eventMessage)
        {
            this.SendMessage(eventMessage, _defaultTimeout);
        }

        /// <summary>
        /// Sends a message to the broker.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The received response.</returns>
        private Message SendMessage(Message message, TimeSpan timeout)
        {
            LastUsed = DateTime.Now;
            if (!this.Client.IsConnected)
            {
                var options = MqttHelper.BuildOptions(this._ipEndPoint.Address, this._ipEndPoint.Port);
                var connectionResult = this.Client.ConnectAsync(options).GetAwaiter().GetResult();

                if (connectionResult.ResultCode != MqttClientConnectResultCode.Success)
                {
                    throw new Exception($"{connectionResult.ReasonString} {connectionResult.ResultCode}");
                }
            }

            string replyTopic = null;

            if (message is RequestMessage requestMessage)
            {
                replyTopic = ExtractReplyTopic(requestMessage.Reply);
                
                if (string.IsNullOrWhiteSpace(replyTopic))
                {
                    throw new ArgumentException(
                        $"ReplyTopic could not be parsed from 'reply' field of message. The given value was: {requestMessage.Reply ?? "null"}");
                }
            }

            var func = MqttHelper.CreateCommunicationFunction(this.Client, this._topic, replyTopic, this._converter, timeout, CancellationToken.None);
            return func(message).GetAwaiter().GetResult();
        }

        public Uri GetLocalUri()
        {
            return new Uri($"http://{this._ipEndPoint.Address}/{this._ipEndPoint.Port}");
        }

        public Uri GetRemoteUri()
        {
            return new Uri($"http://{this._ipEndPoint.Address}/{this._ipEndPoint.Port}");
        }

        private static string ExtractReplyTopic(string replyField)
        {
            if (string.IsNullOrEmpty(replyField))
            {
                return null;
            }

            if (!replyField.Contains(":"))
            {
                throw new ArgumentException("The syntax of the reply field is: adr:mqttreplyTopic. If adr is empty, please provide reply like: ':mqttreplytopic'.", "reply");
            }

            var splitField = replyField.Split(':');
            if (splitField.Length < 2)
            {
                throw new Exception($"The argument passed as reply doesn´t contain a replytopic. Value of replyfield was: '{replyField}'.");
            }

            return splitField.Last();
        }
    }
}