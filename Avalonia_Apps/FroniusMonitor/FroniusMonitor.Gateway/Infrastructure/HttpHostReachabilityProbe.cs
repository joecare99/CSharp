using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FroniusMonitor.Core.Configuration;
using FroniusMonitor.Core.Contracts;
using FroniusMonitor.Core.Models;

namespace FroniusMonitor.Gateway.Infrastructure;

/// <summary>
/// Browser-friendly reachability probe that verifies Fronius host availability via HTTP endpoint checks.
/// </summary>
public sealed class HttpHostReachabilityProbe : IFroniusHostReachabilityProbe
{
    private readonly FroniusDeviceEndpointOptions _endpointOptions;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHostReachabilityProbe"/> class.
    /// </summary>
    /// <param name="endpointOptions">The configured endpoint options.</param>
    /// <param name="httpClient">The HTTP client used for endpoint probes.</param>
    public HttpHostReachabilityProbe(FroniusDeviceEndpointOptions endpointOptions, HttpClient httpClient)
    {
        _endpointOptions = endpointOptions;
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<FroniusHostReachabilityResult> CheckAsync(CancellationToken cancellationToken)
    {
        string sHost = _endpointOptions.Host.Trim();
        if (string.IsNullOrWhiteSpace(sHost))
        {
            return FroniusHostReachabilityResult.Unreachable("The configured inverter host is empty.");
        }

        try
        {
            HttpRequestMessage request = new(HttpMethod.Head, _endpointOptions.BuildPowerFlowUri());
            using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response.IsSuccessStatusCode
                ? FroniusHostReachabilityResult.Reachable()
                : FroniusHostReachabilityResult.Unreachable($"HTTP probe returned status {(int)response.StatusCode}.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return FroniusHostReachabilityResult.Unreachable(ex.Message);
        }
    }
}
