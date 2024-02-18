using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestStatements.DependencyInjection
{
    public sealed class Worker2(
        ILogger<Worker> logger,
        IServiceScopeFactory serviceScopeFactory)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        logger.LogInformation(
                            "Starting scoped work, provider hash: {hash}.",
                            scope.ServiceProvider.GetHashCode());

                        var store = scope.ServiceProvider.GetRequiredService<IObjectStore>();
                        var next = await store.GetNextAsync();
                        logger.LogInformation("{next}", next);

                        var processor = scope.ServiceProvider.GetRequiredService<IObjectProcessor>();
                        await processor.ProcessAsync(next);
                        logger.LogInformation("Processing {name}.", next.Name);

                        var relay = scope.ServiceProvider.GetRequiredService<IObjectRelay>();
                        await relay.RelayAsync(next);
                        logger.LogInformation("Processed results have been relayed.");

                        var marked = await store.MarkAsync(next);
                        logger.LogInformation("Marked as processed: {next}", marked);
                    }
                    finally
                    {
                        logger.LogInformation(
                            "Finished scoped work, provider hash: {hash}.{nl}",
                            scope.ServiceProvider.GetHashCode(), Environment.NewLine);
                    }
                }
            }
        }
    }
}
