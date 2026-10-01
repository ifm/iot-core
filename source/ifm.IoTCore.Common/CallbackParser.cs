namespace ifm.IoTCore.Common;

using System;
using System.Web;
using System.Text.RegularExpressions;
using ifm.Common;

/// <summary>
/// Provides methods to process callback strings.
/// </summary>
public static class CallbackParser
{
    /// <summary>
    /// Represents the individual components of a callback string, including scheme, authority, path, query, and client identifier.
    /// </summary>
    /// <param name="scheme">The scheme component of the URI, such as "http" or "https".</param>
    /// <param name="authority">The authority component of the URI, typically representing the host and optional port.</param>
    /// <param name="path">The path component of the URI, specifying the resource location.</param>
    /// <param name="query">The query component of the URI, containing any query parameters.</param>
    /// <param name="clientId">The client identifier associated with the URI components.</param>
    public class Components(string scheme, string authority, string path, string query, string clientId)
    {
        /// <summary>Gets the scheme component.</summary>
        public string Scheme { get; } = scheme;
        /// <summary>Gets the authority component.</summary>
        public string Authority { get; } = authority;
        /// <summary>Gets the path component.</summary>
        public string Path { get; } = path;
        /// <summary>Gets the query component.</summary>
        public string Query { get; } = query;
        /// <summary>Gets the client identifier component.</summary>
        public string ClientId { get; } = clientId;
    }

    /// <summary>
    /// Checks if the provided address is an absolute URI.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>true, if the address is an absolute URI; otherwise false.</returns>
    public static bool IsAbsolute(string address)
    {
        return address.IndexOf("://", StringComparison.Ordinal) != -1;
    }

    /// <summary>
    /// Checks if the provided address is a valid URI, where in IoTCore [] are allowed.
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns>true if the address is a valid URI; otherwise false.</returns>
    public static bool IsValid(string address)
    {
        // A scheme must be provided
        var pos = address.IndexOf(':');
        if (pos == -1) return false;
        var scheme = address.Substring(0, pos);
        if (string.IsNullOrEmpty(scheme)) return false;

        // Patch address
        address = Regex.Replace(address, @"[\]\[]", "_", RegexOptions.None);

        // If address is absolute only the format of the address must be valid
        pos = address.IndexOf("://", StringComparison.Ordinal);
        if (pos != -1)
        {
            return Uri.IsWellFormedUriString(address, UriKind.Absolute);
        }

        // If address is not absolute it refers to a connected client, so the query with clientID must be provided
        var target = address.Right(':');
        var query = target.Right('?');
        if (query == null) return false;
        var clientId = HttpUtility.ParseQueryString(query).Get("clientid");
        return !string.IsNullOrEmpty(clientId);
    }

    /// <summary>
    /// Gets the components of the address.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>The components.</returns>
    public static Components Parse(string address)
    {
        if (!Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out var uri))
        {
            return null;
        }
        var clientId = HttpUtility.ParseQueryString(uri.Query).Get("clientid");
        return new Components(uri.Scheme, uri.Authority, uri.AbsolutePath, uri.Query, clientId);
    }

    /// <summary>
    /// Gets the scheme from an address.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>The scheme if provided; otherwise null.</returns>
    public static string GetScheme(string address)
    {
        var pos = address.IndexOf(':');
        if (pos == -1) return null;
        var scheme = address.Substring(0, pos);
        return string.IsNullOrEmpty(scheme) ? null : scheme;
    }
}