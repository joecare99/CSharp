using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core.Services;

/// <summary>
/// Resolves the first registered structured change-set sink that can handle a normalized migration destination definition.
/// </summary>
public sealed class MigrationChangeSetSinkFactory : IMigrationChangeSetSinkFactory
{
    private readonly IReadOnlyList<IMigrationChangeSetSink> _sinks;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationChangeSetSinkFactory"/> class.
    /// </summary>
    /// <param name="sinks">The registered structured change-set sinks.</param>
    public MigrationChangeSetSinkFactory(IEnumerable<IMigrationChangeSetSink> sinks)
    {
        ArgumentNullException.ThrowIfNull(sinks);
        _sinks = sourcesToList(sinks);
    }

    /// <inheritdoc />
    public IMigrationChangeSetSink Create(MigrationDestinationDefinition destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        return _sinks.FirstOrDefault(candidate => candidate.CanHandle(destination))
            ?? throw new NotSupportedException("No registered structured change-set sink can handle the supplied destination definition.");
    }

    private static IReadOnlyList<IMigrationChangeSetSink> sourcesToList(IEnumerable<IMigrationChangeSetSink> sinks)
        => sinks.ToArray();
}
