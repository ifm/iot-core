using MQTTnet.Diagnostics.Logger;

namespace ifmIoTCore.NetAdapter.Mqtt
{
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Elements;
    using Elements.Formats;
    using Elements.Valuations;
    using Messages;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using MQTTnet.Client.Publishing;
    using MQTTnet.Client.Receiving;
    using MQTTnet.Client.Subscribing;
    using MQTTnet.Client.Unsubscribing;
    using MQTTnet.Diagnostics;
    using MQTTnet.Implementations;

    public class MqttHelper
    {
        public static MqttClient BuildMqttClient(CancellationToken cancellationToken = default)
        {
            var client = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            return client;
        }

        public static IMqttClientOptions BuildOptions(IPAddress serverIp, int port)
        {
            return new MqttClientOptionsBuilder()
                .WithTcpServer(serverIp.ToString(), port).Build();
        }

        public static IMqttClientOptions BuildOptions(IPAddress serverIp, int port, string clientId)
        {
            return new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(serverIp.ToString(), port).Build();
        }

        /// <summary>
        /// Creates a wrapper around a combined subscribe and publish action.
        /// </summary>
        /// <param name="client">The client to use for this communication.</param>
        /// <param name="publishTopic">The topic, under which the client will publish its request/event.</param>
        /// <param name="replyTopic">The topic on which the client can receive responses to a request.</param>
        /// <param name="converter">The converter to use for serializing the messages.</param>
        /// <param name="timeout">The communication timeout.</param>
        /// <param name="token">A cancellationtoken to cancel the operation.</param>
        /// <returns>The received response.</returns>
        /// <remarks>The client will find the topic under which to publish the response in the reply field of the request message. (For events there is no need for a reply). The syntax of the reply field is: adr:topic </remarks>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        internal static Func<Message, Task<Message>> CreateCommunicationFunction(
            MqttClient client,
            string publishTopic,
            string replyTopic,
            IConverter converter, 
            TimeSpan timeout, 
            CancellationToken token)
        {
            var result = new Func<Message, Task<Message>>(req =>
            {
                return Task.Run(async () =>
                {
                    Message responseMessage = null;
                    var serializedRequest = converter.Serialize(req);

                    if (req.Code == RequestCodes.Event)
                    {
                        var publishresult = await client.PublishAsync(
                            new MqttApplicationMessage
                            {
                                ContentType = converter.Type,
                                Payload = Encoding.UTF8.GetBytes(serializedRequest),
                                Topic = publishTopic
                            }, token);

                        responseMessage = 
                            publishresult.ReasonCode == MqttClientPublishReasonCode.Success
                                ? new Message(ResponseCodes.Success, 0, req.Address, null) 
                                : new ResponseMessage(ResponseCodes.InternalError, 0, req.Address, publishresult.ReasonCode.ToString());
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(replyTopic))
                        {
                            throw new ArgumentNullException("replyTopic may not be null.", nameof(replyTopic));
                        }

                        using (var manualResetEvent = new ManualResetEventSlim())
                        {
                            var subscriptionresult = await client.SubscribeAsync(
                                new MqttClientSubscribeOptions
                                {
                                    TopicFilters = new List<MqttTopicFilter>
                                    {
                                        new MqttTopicFilter { Topic = replyTopic }
                                    }
                                }, token);

                            var subscriptionResult = subscriptionresult.Items.Single();

                            if (subscriptionResult.ResultCode != MqttClientSubscribeResultCode.GrantedQoS0 &&
                                subscriptionResult.ResultCode != MqttClientSubscribeResultCode.GrantedQoS1 &&
                                subscriptionResult.ResultCode != MqttClientSubscribeResultCode.GrantedQoS2)
                            {
                                throw new Exception($"Subscription failed: {subscriptionResult.ResultCode}");
                            }
                            
                            client.ApplicationMessageReceivedHandler =
                                new MqttApplicationMessageReceivedHandlerDelegate(m =>
                                {
                                    var responseString = Encoding.UTF8.GetString(m.ApplicationMessage.Payload);
                                    var tempResponseMessage = converter.Deserialize<Message>(responseString);
                                    if (tempResponseMessage.Code != 10)
                                    {
                                        manualResetEvent.Set();
                                        responseMessage = tempResponseMessage;
                                    }
                                });

                            var publishresult = await client.PublishAsync(
                                new MqttApplicationMessage
                                {
                                    Payload = Encoding.UTF8.GetBytes(serializedRequest), 
                                    Topic = publishTopic
                                }, token);

                            manualResetEvent.Wait(timeout);

                            if (publishresult.ReasonCode != MqttClientPublishReasonCode.Success)
                            {
                                throw new Exception($"Publication failed: {publishresult.ReasonCode} {publishresult.ReasonString}");
                            }
                            
                            client.ApplicationMessageReceivedHandler = null;

                            var unsubscribeResult = await client.UnsubscribeAsync(
                                    new MqttClientUnsubscribeOptions
                                    {
                                        TopicFilters = new List<string> { replyTopic }
                                    }, token);

                            var failedItems = (from failed in unsubscribeResult.Items
                                where failed != null &&
                                      failed.ReasonCode != MqttClientUnsubscribeResultCode.Success
                                select failed).ToArray();

                            if (failedItems.Any())
                            {
                                var exceptions = from item in failedItems
                                    let e = new Exception(
                                        $"Failed unsubscription from topic: '{item.TopicFilter}' with reason '{item.ReasonCode}'.")
                                    select e;
                                throw new AggregateException(exceptions);
                            }
                        }
                    }

                    return responseMessage;
                }, token);
            });
            return result;
        }

        internal static void EnsureDefaultMqttSetup(IElementManager elementManager, IDeviceElement deviceElement)
        {
            var connectionsElement = deviceElement.GetElementByIdentifier("connections");

            if (connectionsElement == null)
            {
                connectionsElement = elementManager.CreateStructureElement(deviceElement, "connections", profiles:new List<string>{"connections"});
            }

            var mqttConnectionElement = connectionsElement.GetElementByIdentifier("mqttconnection");
            if (mqttConnectionElement == null)
            {
                mqttConnectionElement = elementManager.CreateStructureElement(connectionsElement, "mqttconnection", profiles: new List<string>());
            }

            var mqttSetupElement = mqttConnectionElement.GetElementByIdentifier("mqttsetup");

            if (mqttSetupElement == null)
            {
                mqttSetupElement = elementManager.CreateStructureElement(mqttConnectionElement, "mqttsetup", profiles:new List<string>{ "mqttsetup"});
            }

            var qos = mqttSetupElement.GetElementByIdentifier("qos");

            if (qos == null)
            {
                int qosValue = 0;
                qos = elementManager.CreateDataElement<int>(mqttSetupElement, "qos", element => qosValue, (element, i) =>
                {
                    if (qosValue == i) return;
                    qosValue = i;
                } ,true, true, profiles:new List<string>{"parameter"}, format:new IntegerEnumFormat(new IntegerEnumValuation(new Dictionary<string, string>{{"0", "Qos 0"},{"1", "Qos 1"}, {"2", "Qos 2"}})));
                // ToDo: Add a datachanged event?
            }

        }

        /// <summary>
        /// Validates a certificate against a certificate chain. Untrusted roots (thus self signed certificates) are allowed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="certificate">The certificate to check.</param>
        /// <param name="chain">The certificate chain to check against.</param>
        /// <param name="sslPolicyErrors">The <seealso cref="System.Net.Security.SslPolicyErrors"/></param>
        /// <returns>true, if the <paramref name="certificate"/> is valid, otherwise false.</returns>
        /// <remarks>From: <c>https://docs.microsoft.com/en-us/previous-versions/office/developer/exchange-server-2010/bb408523(v=exchg.140)</c></remarks>
        public static bool CertificateValidationCallBack(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain?.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                            (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid.
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }
                // When processing reaches this line, the only errors in the certificate chain are
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange Server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
    }
}
