using Microsoft.Extensions.DependencyInjection;

namespace BaseLib.Show;

/// <summary>
/// Provides the composition root for the BaseLib showcase application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Builds the showcase services and starts the interactive application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    private static void Main(string[] args)
    {
        using ServiceProvider serviceProvider = new ServiceCollection()
            .AddBaseLibShowcase()
            .BuildServiceProvider();

        serviceProvider.GetRequiredService<ShowcaseApplication>().Run();
    }
}
