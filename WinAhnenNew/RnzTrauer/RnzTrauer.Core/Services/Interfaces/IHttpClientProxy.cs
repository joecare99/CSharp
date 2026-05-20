using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RnzTrauer.Core.Services.Interfaces;

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
