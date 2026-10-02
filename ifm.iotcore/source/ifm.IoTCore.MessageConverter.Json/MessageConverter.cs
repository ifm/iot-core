namespace ifm.IoTCore.MessageConverter.Json;

using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ifm.Common.Variant;

using Common;
using Common.Exceptions;
using Message;
using Contracts;

/// <summary>
/// Specifies how a message deserialization operation handles null or missing message content.
/// </summary>
/// <remarks>Use this enumeration to control whether deserialization methods allow null message payloads or throw
/// an exception when encountering null or missing data. The selected mode affects how deserialization errors are
/// handled and may influence application error handling strategies.</remarks>
public enum MessageDeserializationMode
{
    /// <summary>Indicates that null values are allowed for the associated parameter, property, or field.</summary>
    AllowNull = 0,
    /// <summary>Specifies that an exception should be thrown if a null value is encountered.</summary>
    ThrowOnNull = 1
}

/// <summary>
/// Provides functionality to serialize and deserialize messages to and from JSON format for communication with IoT Core
/// systems.
/// </summary>
/// <remarks>The MessageConverter supports configurable deserialization behavior through the
/// MessageDeserializationMode parameter. It uses relaxed JSON escaping to ensure compatibility with special characters
/// in message content. This class is typically used to convert between strongly-typed Message objects and their JSON
/// representations for transmission or storage.</remarks>
public class MessageConverter : IMessageConverter
{
    // Note: The encoder option UnsafeRelaxedJsonEscaping is needed because of special character escaping.
    // see : https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/character-encoding
    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        PropertyNameCaseInsensitive = true
    };

    private readonly MessageDeserializationMode _converterMode;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public MessageConverter() : this(MessageDeserializationMode.ThrowOnNull)
    {
    }

    /// <summary>
    /// Initializes a new instance.
    /// <param name="messageConverterMode">The mode of the </param>
    /// </summary>
    public MessageConverter(MessageDeserializationMode messageConverterMode)
    {
        _converterMode = messageConverterMode;
    }

    /// <summary>
    /// Gets the content type represented by this instance.
    /// </summary>
    public string Type => "json";

    /// <summary>
    /// Gets the media type of the content format used by this instance.
    /// </summary>
    public string ContentType => "application/json";

    /// <summary>
    /// Serializes the specified message to its JSON representation.
    /// </summary>
    /// <param name="message">The message to serialize. Cannot be null.</param>
    /// <returns>A JSON string that represents the serialized message.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
    /// <exception cref="BadRequestException">Thrown if an error occurs during serialization.</exception>
    public string Serialize(Message message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        try
        {
            var authenticationInfoConverter = message.Authentication != null ?
                new InnerAuthenticationInfo(message.Authentication.User, message.Authentication.Password, message.Authentication.Token) :
                null;

            JsonElement? data = message.Data != null ? 
                Variant.ToJsonElement(message.Data) : 
                null;

            var convertMessage = new InnerMessage(message.Code,
                message.Cid,
                message.Address,
                data,
                message.Reply,
                authenticationInfoConverter);

            var json = JsonSerializer.Serialize(convertMessage, _serializeOptions);

            return json;
        }
        catch (Exception e)
        {
            throw new BadRequestException(e.Message);
        }
    }

    /// <summary>
    /// Deserializes the specified JSON string into a Message object.
    /// </summary>
    /// <param name="json">A JSON-formatted string representing the message to deserialize. Cannot be null.</param>
    /// <returns>A Message object that represents the data contained in the JSON string.</returns>
    /// <exception cref="IoTCoreException">Thrown if the input JSON is null, invalid, or cannot be converted to a Message object.</exception>
    public Message Deserialize(string json)
    {
        if (json == null) throw new IoTCoreException(ResponseCodes.BadRequest, "Json is null");

        try
        {
            var innerMessage = JsonSerializer.Deserialize<InnerMessage>(json, _serializeOptions) ?? throw new BadRequestException("Convert json failed");

            Variant data = null;
            try
            {
                if (innerMessage.Data.HasValue)
                {
                    data = Variant.FromJsonElement(innerMessage.Data.Value, _converterMode == MessageDeserializationMode.ThrowOnNull);
                }
            }
            catch (Exception e)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, e.Message);
            }

            // Handle non-conform error response message
            if (!RequestCodes.IsRequestOrEvent(innerMessage.Code) && !ResponseCodes.IsSuccess(innerMessage.Code))
            {
                if (innerMessage.Error != null || innerMessage.Msg != null)
                {
                    data = new VariantObject();
                    if (innerMessage.Error != null) ((VariantObject)data).Add((VariantValue)"error", (VariantValue)innerMessage.Error);
                    if (innerMessage.Msg != null) ((VariantObject)data).Add((VariantValue)"msg", (VariantValue)innerMessage.Msg);
                }
            }

            var authenticationInfo = innerMessage.AuthenticationInfo != null
                ? new Message.AuthenticationInfo(innerMessage.AuthenticationInfo.User,
                    innerMessage.AuthenticationInfo.Password, 
                    innerMessage.AuthenticationInfo.Token)
                : null;

            var message = new Message(innerMessage.Code,
                innerMessage.Cid,
                innerMessage.Address,
                data,
                innerMessage.Reply,
                authenticationInfo);

            return message;
        }
        catch (IoTCoreException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new IoTCoreException(ResponseCodes.BadRequest, e.Message);
        }
    }

    private class InnerAuthenticationInfo(string user, string password, string token)
    {
        [JsonPropertyName("user" )]
        public string User { get; } = user;

        [JsonPropertyName("passwd" )]
        public string Password { get; } = password;

        [JsonPropertyName("token")]
        public string Token { get; } = token;
    }

    private class InnerMessage
    {
        [JsonIgnore]
        public int Code
        {
            get => InnerCode.GetInt32();
            set
            {
                var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(value.ToString()));
                InnerCode = JsonElement.ParseValue(ref reader);
            }
        }

        [JsonPropertyName("code"), JsonRequired] 
        public JsonElement InnerCode { get; set; }

        [JsonPropertyName("cid"), JsonRequired]
        public int Cid { get; set; }

        // Allow adr field to be null, because older iotcore do not have this in the response message.
        [JsonPropertyName("adr")]
        public string Address { get; set; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public JsonElement? Data { get; set; }

        [JsonPropertyName("reply")]
        public string Reply { get; set; }

        [JsonPropertyName("auth")]
        public InnerAuthenticationInfo AuthenticationInfo { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        public InnerMessage(int code, int cid, string address, JsonElement? data, string reply, InnerAuthenticationInfo authenticationInfo, string error = null, string msg = null)
        {
            Code = code;
            Cid = cid;
            Address = address;
            Data = data;
            Reply = reply;
            AuthenticationInfo = authenticationInfo;
            Error = error;
            Msg = msg;
        }

        [JsonConstructor]
        public InnerMessage(JsonElement innerCode, int cid, string address, JsonElement? data, string reply, InnerAuthenticationInfo authenticationInfo, string error = null, string msg = null)
        {
            switch (innerCode.ValueKind)
            {
                case JsonValueKind.String:
                {
                    var codeAsString = innerCode.GetString();
                    switch (codeAsString?.ToLower())
                    {
                        case "event":
                            Code = RequestCodes.Event;
                            break;
                        case "request":
                            Code = RequestCodes.Request;
                            break;
                        default:
                            throw new BadRequestException($"Bad code '{codeAsString}'");
                    }

                    break;
                }
                case JsonValueKind.Number:
                {
                    Code = innerCode.GetInt32();
                    break;
                }
                case JsonValueKind.Undefined:
                    break;
                case JsonValueKind.Null:
                    throw new BadRequestException("Bad code 'null' not allowed");
                case JsonValueKind.Object:
                    throw new BadRequestException("Bad code 'object' not allowed");
                case JsonValueKind.Array:
                    throw new BadRequestException("Bad code 'array' not allowed");
                case JsonValueKind.True:
                    throw new BadRequestException("Bad code 'true' not allowed");
                case JsonValueKind.False:
                    throw new BadRequestException("Bad code 'false' not allowed");
                default:
                    throw new BadRequestException("Bad code 'unknown type' not allowed");
            }

            Cid = cid;
            Address = address;
            Data = data;
            Reply = reply;
            AuthenticationInfo = authenticationInfo;
            Error = error;
            Msg = msg;
        }
    }
}