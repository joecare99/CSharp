using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Abstractions.Interfaces;

/// <summary>
/// Provider-neutral contract for importing genealogy data from various sources.
/// Implementations may read GEDCOM files, WinAhnen databases, SecureStore archives, or any other format.
/// </summary>
public interface IFamilyDataSource
{
    /// <summary>
    /// Gets the unique identifier of this data source provider.
    /// Used for selection in CLI and DI registration (e.g., "gedcom", "winahnen", "securestore").
    /// </summary>
    string SourceId { get; }

    /// <summary>
    /// Gets a human-readable display name for this data source provider.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Checks whether the given stream appears to be a valid input for this data source.
    /// Performs a non-destructive peek at the stream's beginning bytes or text.
    /// </summary>
    /// <param name="stream">The stream to inspect (must support seeking).</param>
    /// <returns>true if the stream format matches; false otherwise.</returns>
    bool CanRead(Stream stream);

    /// <summary>
    /// Imports genealogy data from the given stream.
    /// The consumer is responsible for opening/closing the stream.
    /// </summary>
    /// <param name="stream">The input stream to read from.</param>
    /// <param name="cancellationToken">Token for cancelling the import process.</param>
    /// <returns>An IGenealogy payload with families, persons, and places populated.</returns>
    Task<IGenealogy> ImportAsync(
        Stream stream,
        CancellationToken cancellationToken = default );
}

/// <summary>
/// Registry for data source providers — enables runtime discovery and selection.
/// Implementations may be DI-based, static, or a combination of both.
/// </summary>
public interface IFamilyDataSourceRegistry
{
    /// <summary>
    /// Registers a data source provider by its SourceId.
    /// If a provider with the same SourceId already exists, it is replaced.
    /// </summary>
    void RegisterProvider(IFamilyDataSource provider);

    /// <summary>
    /// Gets a registered data source provider by its SourceId.
    /// Returns null if no provider with that ID is registered.
    /// </summary>
    IFamilyDataSource? GetProvider(string sourceId);

    /// <summary>
    /// Lists all registered providers in registration order.
    /// </summary>
    IReadOnlyList<IFamilyDataSource> ListProviders();

    /// <summary>
    /// Automatically detects the best matching provider for the given stream by calling CanRead on each registered provider.
    /// Returns the first matching provider, or null if no provider can read the stream.
    /// </summary>
    IFamilyDataSource? DetectProvider(Stream stream);
}
