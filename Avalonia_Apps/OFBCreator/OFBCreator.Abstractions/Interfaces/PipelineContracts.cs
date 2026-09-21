using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Abstractions.Interfaces;

/// <summary>
/// Provider-neutral contract for selecting families from imported genealogy data.
/// Implementations may use any algorithm (place-based, name-based, manual, etc.).
/// </summary>
public interface IFamilySelectionProvider
{
    /// <summary>
    /// Selects families and their related persons from the source genealogy.
    /// Returns a selection snapshot carrying all downstream input without raw traversal.
    /// </summary>
    /// <param name="source">The imported genealogy data.</param>
    /// <param name="cancellationToken">Token for cancelling the selection process.</param>
    /// <returns>A selection snapshot with selected families, virtual persons, and exclusion diagnostics.</returns>
    Task<OFBSourceSelection> SelectAsync(
        IGenealogy source,
        CancellationToken cancellationToken = default );
}

/// <summary>
/// Provider-neutral contract for grouping families by name or other criteria.
/// Implementations decide how to compute group membership (surname combining, phonetic matching, etc.).
/// </summary>
public interface IFamilyGroupingProvider
{
    /// <summary>
    /// Groups the selected families into name-based or custom groups.
    /// </summary>
    /// <param name="families">Families from a selection step.</param>
    /// <param name="cancellationToken">Token for cancelling the grouping process.</param>
    /// <returns>A mapping of group keys to families in that group.</returns>
    Task<Dictionary<string, IReadOnlyList<IGenFamily>>> GroupAsync(
        IEnumerable<IGenFamily> families,
        CancellationToken cancellationToken = default );
}

/// <summary>
/// Base contract for index generators producing alphabetical cross-reference data.
/// Index consumers consume selected, numbered OFB family data (not raw genealogy).
/// </summary>
public interface IIndexGenerator<TEntry>
{
    /// <summary>
    /// Generates index entries from the supplied selected families.
    /// Entries must carry a SortKey for alphabetical ordering and a Ref for cross-referencing.
    /// </summary>
    Task<IReadOnlyList<TEntry>> GenerateAsync(
        IReadOnlyList<OFBFamilyModel> selectedFamilies,
        CancellationToken cancellationToken = default );
}

/// <summary>
/// Non-generic base for type discovery (allows IIndexGenerator to be used as a constraint or marker).
/// </summary>
public interface IIndexGenerator : IIndexGenerator<OFBIndexEntry> { }

/// <summary>
/// Contract for generating the person index (alphabetical listing of all persons with references).
/// </summary>
public interface IPersonIndexGenerator : IIndexGenerator<PersonIndexEntry> { }

/// <summary>
/// Contract for generating the occupation index (mapping of occupations to persons/families).
/// </summary>
public interface IOccupationIndexGenerator : IIndexGenerator<OFBIndexEntry> { }

/// <summary>
/// Contract for generating the property/estate index (mapping of properties to families).
/// </summary>
public interface IPropertyIndexGenerator : IIndexGenerator<OFBIndexEntry> { }

/// <summary>
/// Contract for composing OFB document sections (title page, preface, indices, family bodies).
/// Uses IUserDocumentFactory for format-agnostic document creation.
/// </summary>
public interface IDocumentComposer
{
    /// <summary>
    /// Composes the complete OFB document from selection data and indices.
    /// Writes title page, preface, indices, and family body sections in OFB-specific order.
    /// </summary>
    /// <param name="document">The document created via IUserDocumentFactory.</param>
    /// <param name="selection">The selection snapshot carrying all downstream input.</param>
    /// <param name="personIndex">Generated person index entries.</param>
    /// <param name="occupationIndex">Generated occupation index entries.</param>
    /// <param name="propertyIndex">Generated property index entries.</param>
    /// <param name="placeHierarchyIndex">Generated hierarchical place index nodes.</param>
    /// <param name="placeAlphaIndex">Generated alphabetical place index entries.</param>
    /// <param name="cancellationToken">Token for cancelling the composition process.</param>
    Task ComposeAsync(
        object document,  // IUserDocument (avoid introducing UI dependency via using directive)
        OFBSourceSelection selection,
        IReadOnlyList<OFBIndexEntry> personIndex,
        IReadOnlyList<OFBIndexEntry> occupationIndex,
        IReadOnlyList<OFBIndexEntry> propertyIndex,
        IReadOnlyList<OFBPlaceHierarchyNode> placeHierarchyIndex,
        IReadOnlyList<OFBIndexEntry> placeAlphaIndex,
        CancellationToken cancellationToken = default );
}

/// <summary>
/// Collects diagnostic messages (warnings, informational notes) during pipeline execution.
/// Non-blocking — errors are thrown; diagnostics accumulate in this collector.
/// </summary>
public interface IDiagnosticCollector
{
    /// <summary>
    /// Records a warning-level diagnostic message with optional context.
    /// </summary>
    void AddWarning( string message, string? context = null );

    /// <summary>
    /// Records an informational diagnostic message.
    /// </summary>
    void AddInfo( string message );

    /// <summary>
    /// Returns all collected diagnostics in chronological order.
    /// </summary>
    IReadOnlyList<IDiagnosticEntry> GetDiagnostics();
}

/// <summary>
/// Represents a single diagnostic entry (warning or info) with optional context.
/// </summary>
public interface IDiagnosticEntry
{
    /// <summary>
    /// Whether this is a warning or info level message.
    /// </summary>
    bool IsWarning { get; }

    /// <summary>
    /// The diagnostic message text.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Optional context (e.g., entity ID, file path) related to this message.
    /// May be null if no specific context applies.
    /// </summary>
    string? Context { get; }
}

/// <summary>
/// Tracks progress of the OFB pipeline with cancellable steps.
/// Each stage reports its current position and total expected work units.
/// </summary>
public interface IProgressTracker<TProgress> where TProgress : class
{
    /// <summary>
    /// Notified when a pipeline stage completes.
    /// Progress must be serializable for cross-process scenarios (e.g., UI binding).
    /// </summary>
    event Action<TProgress>? ProgressChanged;

    /// <summary>
    /// Cancels the ongoing pipeline operation.
    /// Implementations should honor this and throw OperationCanceledException.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Returns the current progress snapshot.
    /// </summary>
    TProgress GetCurrent();
}

/// <summary>
/// Abstract base for cancellable pipeline stages.
/// Provides a common mechanism for progress reporting and cancellation checks.
/// </summary>
public abstract class CancellableStageBase
{
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    /// Creates a stage with an associated cancellation token.
    /// </summary>
    protected CancellableStageBase( CancellationToken cancellationToken )
    {
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Checks for cancellation and throws OperationCanceledException if requested.
    /// Also verifies that the output has not already been cancelled (e.g., by UI).
    /// </summary>
    protected void EnsureNotCancelled()
    {
        _cancellationToken.ThrowIfCancellationRequested();
    }
}
