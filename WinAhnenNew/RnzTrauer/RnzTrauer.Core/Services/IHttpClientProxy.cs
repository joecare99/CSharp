namespace RnzTrauer.Core;

/// <summary>
/// Provides an abstraction over <see cref="HttpClient"/> for testable HTTP access.
/// </summary>
public interface IHttpClientProxy : IDisposable
{
    /// <summary>
    /// Sends a GET request to the specified URI.
    /// </summary>
    Task<HttpResponseMessage> GetAsync(string sRequestUri);
}
