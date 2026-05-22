using RepoMigrator.App.Logic.Models;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.App.Logic.Services;

/// <summary>
/// Loads source and target selection data from version control providers.
/// </summary>
public sealed class RepositorySelectionService
{
    private readonly IProviderFactory _providerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositorySelectionService" /> class.
    /// </summary>
    public RepositorySelectionService(IProviderFactory providerFactory)
        => _providerFactory = providerFactory;

    /// <summary>
    /// Loads source-side branch tag and revision selection data.
    /// </summary>
    public async Task<SourceSelectionResult> LoadSourceSelectionAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        await using var provider = _providerFactory.Create(endpoint.ProviderKey);
        var capabilities = await provider.GetCapabilitiesAsync(endpoint, ct);
        var selectionData = capabilities.SupportsBranchSelection || capabilities.SupportsTagSelection || capabilities.SupportsRevisionSelection
            ? await provider.GetSelectionDataAsync(endpoint, ct)
            : new RepositorySelectionData();

        return new SourceSelectionResult
        {
            Capabilities = capabilities,
            Branches = capabilities.SupportsBranchSelection ? selectionData.Branches : Array.Empty<RepositoryReferenceInfo>(),
            Tags = capabilities.SupportsTagSelection ? selectionData.Tags : Array.Empty<RepositoryReferenceInfo>(),
            DefaultBranch = selectionData.DefaultBranch,
            Revisions = capabilities.SupportsRevisionSelection ? selectionData.Revisions : Array.Empty<RepositoryRevisionInfo>(),
            SuggestedFromRevisionId = selectionData.SuggestedFromRevisionId,
            SuggestedToRevisionId = selectionData.SuggestedToRevisionId
        };
    }

    /// <summary>
    /// Loads target-side Git branch selection data.
    /// </summary>
    public async Task<TargetSelectionResult> LoadTargetSelectionAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        await using var provider = _providerFactory.Create(endpoint.ProviderKey);
        var capabilities = await provider.GetCapabilitiesAsync(endpoint, ct);
        if (!capabilities.SupportsBranchSelection)
            return new TargetSelectionResult { Capabilities = capabilities };

        var selectionData = await provider.GetSelectionDataAsync(endpoint, ct);
        return new TargetSelectionResult
        {
            Capabilities = capabilities,
            Branches = selectionData.Branches.Select(branchInfo => branchInfo.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            DefaultBranch = selectionData.DefaultBranch
        };
    }
}
