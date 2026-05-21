using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core.Services;

/// <summary>
/// Resolves the first registered structured change-set source that can handle a normalized migration source definition.
/// </summary>
public sealed class MigrationChangeSetSourceFactory : IMigrationChangeSetSourceFactory
{
    private readonly IReadOnlyList<IMigrationChangeSetSource> _sources;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationChangeSetSourceFactory"/> class.
    /// </summary>
    /// <param name="sources">The registered structured change-set sources.</param>
    public MigrationChangeSetSourceFactory(IEnumerable<IMigrationChangeSetSource> sources)
    {
        ArgumentNullException.ThrowIfNull(sources);
        _sources = sources.ToArray();
    }

    /// <inheritdoc />
    public IMigrationChangeSetSource Create(MigrationSourceDefinition source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return _sources.FirstOrDefault(candidate => candidate.CanHandle(source))
            ?? throw new NotSupportedException("No registered structured change-set source can handle the supplied source definition.");
    }
}
