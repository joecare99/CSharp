using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FroniusMonitor.Core.Configuration;
using FroniusMonitor.Core.Contracts;
using FroniusMonitor.Core.Services;
using FroniusMonitor.Gateway.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddSingleton(new FroniusDeviceEndpointOptions
{
    Host = "192.168.0.27",
});
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<IFroniusPowerFlowClient, FroniusPowerFlowClient>();
builder.Services.AddSingleton<FroniusPowerFlowJsonParser>();
builder.Services.AddSingleton<IFroniusSnapshotService, FroniusSnapshotService>();
builder.Services.AddSingleton<IFroniusHostReachabilityProbe, HttpHostReachabilityProbe>();

WebApplication app = builder.Build();
app.UseCors();

app.MapGet("/health", () => Results.Ok(new { Status = "Ok" }));

app.MapGet("/api/config", (FroniusDeviceEndpointOptions options) =>
    Results.Ok(new
    {
        options.Scheme,
        options.Host,
        options.Port,
        PowerFlowUri = options.BuildPowerFlowUri().ToString(),
    }));

app.MapGet("/api/snapshot", async (IFroniusSnapshotService snapshotService, CancellationToken cancellationToken) =>
{
    FroniusMonitor.Core.Models.FroniusSnapshotResult result = await snapshotService.GetSnapshotAsync(cancellationToken).ConfigureAwait(false);
    return Results.Ok(result);
});

app.MapGet("/api/snapshot/live", async (int count, int intervalMs, IFroniusSnapshotService snapshotService, CancellationToken cancellationToken) =>
{
    int iCount = Math.Clamp(count <= 0 ? 5 : count, 1, 60);
    int iIntervalMs = Math.Clamp(intervalMs <= 0 ? 1000 : intervalMs, 250, 10000);

    List<FroniusMonitor.Core.Models.FroniusSnapshotResult> results = [];
    for (int i = 0; i < iCount; i++)
    {
        results.Add(await snapshotService.GetSnapshotAsync(cancellationToken).ConfigureAwait(false));
        if (i < (iCount - 1))
        {
            await Task.Delay(iIntervalMs, cancellationToken).ConfigureAwait(false);
        }
    }

    return Results.Ok(results);
});

app.Run();
