using Microsoft.Extensions.DependencyInjection;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Git;

namespace RepoMigrator.App.Wpf;

/// <summary>
/// Resolves normalized destination providers used by the WPF application archive workflow.
/// </summary>
public sealed class ArchiveMigrationDestinationProviderFactory : IMigrationDestinationProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveMigrationDestinationProviderFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The application service provider used to resolve destination provider dependencies.</param>
    public ArchiveMigrationDestinationProviderFactory(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc />
    public IMigrationDestinationProvider Create(MigrationDestinationDefinition destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        var provider = new VersionControlMigrationDestinationProvider(_serviceProvider.GetRequiredService<GitProvider>());
        return provider.CanHandle(destination)
            ? provider
            : throw new NotSupportedException("The supplied archive migration destination definition is not supported by the WPF application.");
    }
}
