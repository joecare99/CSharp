namespace RnzTrauer.Core;

/// <summary>
/// Delegates HTTP operations to <see cref="HttpClient"/>.
/// </summary>
public sealed class HttpClientProxy : IHttpClientProxy
{
    private readonly HttpClient _xHttpClient = new();

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetAsync(string sRequestUri)
    {
        return _xHttpClient.GetAsync(sRequestUri);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _xHttpClient.Dispose();
    }
}
