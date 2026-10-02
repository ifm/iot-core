namespace ifm.Logger;

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

/// <summary>
/// An appender that sends log events to a specified HTTP endpoint using POST requests.
/// </summary>
public class HttpAppender : AppenderSkeleton, IDisposable
{
    private readonly HttpClient _httpClient = new();

    /// <summary>
    /// Gets or sets the endpoint URL that log events are posted to.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the content type used for the request body. Defaults to text/plain.
    /// </summary>
    public string ContentType { get; set; } = "text/plain";

    /// <summary>
    /// Gets or sets an optional Authorization header value (e.g. a bearer token).
    /// </summary>
    public string AuthorizationHeader { get; set; }

    /// <summary>
    /// Appends a logging event by sending it to the configured HTTP endpoint.
    /// </summary>
    /// <param name="loggingEvent">The logging event to append.</param>
    protected override void Append(LoggingEvent loggingEvent)
    {
        if (string.IsNullOrWhiteSpace(Url))
        {
            ErrorHandler.Error("HttpAppender requires the Url property to be set.");
            return;
        }

        using var writer = new StringWriter();
        Layout?.Format(writer, loggingEvent);

        SendAsync(writer.ToString()).GetAwaiter().GetResult();
    }

    private async Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var content = new StringContent(message, Encoding.UTF8, ContentType);
            using var request = new HttpRequestMessage(HttpMethod.Post, Url);
            request.Content = content;

            if (!string.IsNullOrWhiteSpace(AuthorizationHeader))
            {
                request.Headers.TryAddWithoutValidation("Authorization", AuthorizationHeader);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            ErrorHandler.Error("Failed to send log event via HTTP.", ex);
        }
    }

    /// <summary>
    /// Disposes the HttpClient instance used by the appender.
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
