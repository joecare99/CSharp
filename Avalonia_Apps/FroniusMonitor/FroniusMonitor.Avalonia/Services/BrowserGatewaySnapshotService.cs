using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using FroniusMonitor.Core.Configuration;
using FroniusMonitor.Core.Contracts;
using FroniusMonitor.Core.Enums;
using FroniusMonitor.Core.Models;

namespace FroniusMonitor.Avalonia.Services;

/// <summary>
/// Retrieves Fronius snapshots through the gateway API for browser-hosted execution.
/// </summary>
public sealed class BrowserGatewaySnapshotService : IFroniusSnapshotService
{
    private readonly HttpClient _httpClient;
    private readonly FroniusDeviceEndpointOptions _endpointOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserGatewaySnapshotService"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client configured with gateway base address.</param>
    /// <param name="endpointOptions">Current endpoint options containing optional target host.</param>
    public BrowserGatewaySnapshotService(HttpClient httpClient, FroniusDeviceEndpointOptions endpointOptions)
    {
        _httpClient = httpClient;
        _endpointOptions = endpointOptions;
    }

    /// <inheritdoc />
    public async Task<FroniusSnapshotResult> GetSnapshotAsync(CancellationToken cancellationToken)
    {
        string sPath = string.IsNullOrWhiteSpace(_endpointOptions.Host)
            ? "api/snapshot"
            : $"api/snapshot?host={Uri.EscapeDataString(_endpointOptions.Host)}";

        using HttpResponseMessage response = await _httpClient.GetAsync(sPath, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return FroniusSnapshotResult.Failure(FroniusSnapshotFailureReason.ConnectionFailed, $"Gateway returned status {(int)response.StatusCode}.");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        bool xIsSuccess = root.TryGetProperty(nameof(FroniusSnapshotResult.IsSuccess), out JsonElement isSuccessElement)
            && isSuccessElement.ValueKind == JsonValueKind.True;

        if (!xIsSuccess)
        {
            FroniusSnapshotFailureReason failureReason = ParseFailureReason(root);
            string? sTechnicalMessage = root.TryGetProperty(nameof(FroniusSnapshotResult.TechnicalMessage), out JsonElement technicalMessageElement)
                ? technicalMessageElement.GetString()
                : null;
            return FroniusSnapshotResult.Failure(failureReason, sTechnicalMessage);
        }

        if (!root.TryGetProperty(nameof(FroniusSnapshotResult.Snapshot), out JsonElement snapshotElement))
        {
            return FroniusSnapshotResult.Failure(FroniusSnapshotFailureReason.InvalidPayload, "Gateway did not include snapshot payload.");
        }

        FroniusSnapshot? snapshot = JsonSerializer.Deserialize<FroniusSnapshot>(snapshotElement.GetRawText(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        return snapshot is null
            ? FroniusSnapshotResult.Failure(FroniusSnapshotFailureReason.InvalidPayload, "Gateway snapshot payload could not be deserialized.")
            : FroniusSnapshotResult.Success(snapshot);
    }

    private static FroniusSnapshotFailureReason ParseFailureReason(JsonElement root)
    {
        if (!root.TryGetProperty(nameof(FroniusSnapshotResult.FailureReason), out JsonElement failureReasonElement))
        {
            return FroniusSnapshotFailureReason.Unknown;
        }

        return failureReasonElement.ValueKind switch
        {
            JsonValueKind.Number when failureReasonElement.TryGetInt32(out int iFailureReason)
                => Enum.IsDefined(typeof(FroniusSnapshotFailureReason), iFailureReason)
                    ? (FroniusSnapshotFailureReason)iFailureReason
                    : FroniusSnapshotFailureReason.Unknown,
            JsonValueKind.String when Enum.TryParse(failureReasonElement.GetString(), out FroniusSnapshotFailureReason failureReason)
                => failureReason,
            _ => FroniusSnapshotFailureReason.Unknown,
        };
    }
}
