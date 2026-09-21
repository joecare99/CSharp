using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Generates a hierarchical place index from selected OFB families.
/// Builds a tree structure using the IGenPlace.Parent chain, then extracts it into OFBPlaceHierarchyNode nodes.
/// </summary>
public class HierarchicalPlaceIndexGenerator : IHierarchicalPlaceIndexGenerator
{
    private readonly ILogger<HierarchicalPlaceIndexGenerator>? _logger;

    public HierarchicalPlaceIndexGenerator(ILogger<HierarchicalPlaceIndexGenerator>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OFBPlaceHierarchyNode>> GenerateAsync(
        IReadOnlyList<OFBFamilyModel> selectedFamilies,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selectedFamilies);

        // Phase 1: Collect all unique place trees (IGenPlace objects with Parent chains)
        var placeMap = new Dictionary<string, IGenPlace>(StringComparer.Ordinal);

        foreach (var family in selectedFamilies)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Collect places from marriage, husband's birth, wife's birth, and children's births
            var placesToProcess = new List<IGenPlace?>();

            if (family.MarriagePlace != null)
                placesToProcess.Add(family.MarriagePlace);

            if (family.Husband?.BirthPlace != null)
                placesToProcess.Add(family.Husband.BirthPlace);

            if (family.Wife?.BirthPlace != null)
                placesToProcess.Add(family.Wife.BirthPlace);

            if (family.Children != null)
            {
                foreach (var child in family.Children)
                {
                    if (child != null && child.BirthPlace != null)
                        placesToProcess.Add(child.BirthPlace);
                }
            }

            // Process death places for adults
            if (family.Husband?.DeathPlace != null)
                placesToProcess.Add(family.Husband.DeathPlace);

            if (family.Wife?.DeathPlace != null)
                placesToProcess.Add(family.Wife.DeathPlace);

            foreach (var place in placesToProcess.Where(p => p != null))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var placeId = GetPlaceId(place!);

                if (!placeMap.ContainsKey(placeId))
                    placeMap[placeId] = place!;
            }
        }

        // Phase 2: Build the hierarchical tree structure
        var rootNodes = new Dictionary<string, OFBPlaceHierarchyNode>(StringComparer.Ordinal);

        foreach (var kvp in placeMap)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var place = kvp.Value;
            var nodeId = kvp.Key;

            var node = rootNodes.ContainsKey(nodeId)
                ? rootNodes[nodeId]
                : new OFBPlaceHierarchyNode(place.Name ?? "Unknown", nodeId);

            if (!rootNodes.ContainsKey(nodeId))
                rootNodes[nodeId] = node;

            // Build parent chain and link node to its parent
            if (place.Parent != null)
            {
                var parentId = GetPlaceId(place.Parent);

                if (!rootNodes.ContainsKey(parentId))
                {
                    // Only set ParentPlaceId on parentNode if the grandparent has a valid identifier
                    var grandParentId = place.Parent.Parent != null ? GetPlaceId(place.Parent.Parent) : null;

                    var parentNode = new OFBPlaceHierarchyNode(
                        place.Parent.Name ?? "Unknown",
                        parentId)
                    {
                        ParentPlaceId = grandParentId
                    };

                    rootNodes[parentId] = parentNode;
                }

                // Set ParentPlaceId on the child node so it links to its parent in Phase 4 filtering
                node.ParentPlaceId = parentId;

                // Build additional parent chain nodes for parents that aren't already in our set
                var currentParent = place.Parent;
                while (currentParent != null && currentParent.Parent != null)
                {
                    var nextParentId = GetPlaceId(currentParent.Parent);

                    if (!rootNodes.ContainsKey(nextParentId))
                    {
                        var greatGrandParentId = currentParent.Parent.Parent != null
                            ? GetPlaceId(currentParent.Parent.Parent)
                            : null;

                        var grandParentNode = new OFBPlaceHierarchyNode(
                            currentParent.Parent.Name ?? "Unknown",
                            nextParentId)
                        {
                            ParentPlaceId = greatGrandParentId
                        };

                        rootNodes[nextParentId] = grandParentNode;
                    }

                    currentParent = currentParent.Parent;
                }
            }
        }

        // Phase 3: Link children to parents using the original place parent chain
        var linkedNodeIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (var kvp in rootNodes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nodeId = kvp.Key;
            var node = kvp.Value;

            // Find the original place that corresponds to this nodeId
            // First check if it's in placeMap (Phase 1 collected places)
            var originalPlace = placeMap.TryGetValue(nodeId, out var mappedPlace)
                ? mappedPlace
                : null;

            // If not in placeMap, try to find a matching entry by name/ID comparison
            if (originalPlace == null)
            {
                foreach (var pm in placeMap)
                {
                    var pmPlace = pm.Value;
                    var pmId = GetPlaceId(pmPlace);

                    // Match by PlaceId of the PM's derived place chain
                    // Walk up the parent chain and see if any ancestor has a matching nodeId
                    var candidate = pmPlace;
                    while (candidate != null)
                    {
                        if (GetPlaceId(candidate).Equals(nodeId, StringComparison.Ordinal))
                        {
                            originalPlace = candidate;
                            break;
                        }

                        candidate = candidate.Parent;
                    }

                    if (originalPlace != null)
                        break;
                }
            }

            // Link the node to its parent in the hierarchy
            if (originalPlace == null)
                continue;

            var currentPlace = originalPlace;
            while (currentPlace.Parent != null && !linkedNodeIds.Contains(nodeId))
            {
                var parentPlaceId = GetPlaceId(currentPlace.Parent);

                if (rootNodes.TryGetValue(parentPlaceId, out var parentNode))
                {
                    // Check if node is already in parent's children to avoid duplicates
                    if (!parentNode.Children.Any(c => c.PlaceId == nodeId))
                    {
                        var updatedChildren = new List<OFBPlaceHierarchyNode>(parentNode.Children) { node };
                        parentNode.Children = updatedChildren.ToArray();
                    }

                    linkedNodeIds.Add(nodeId);
                }

                currentPlace = currentPlace.Parent;
            }
        }

        // Phase 4: Collect root nodes (those without a parent in our set)
        var roots = rootNodes.Values
            .Where(n => n.ParentPlaceId == null || !rootNodes.ContainsKey(n.ParentPlaceId))
            .OrderBy(n => n.Name, StringComparer.Ordinal)
            .ToList();

        _logger?.LogInformation("Generated hierarchical place index with {RootCount} top-level nodes from {FamilyCount} families.",
            roots.Count, selectedFamilies.Count);

        return roots.AsReadOnly();
    }

    /// <summary>
    /// Extracts a unique identifier for a place (GOV_ID or Name).
    /// </summary>
    private static string GetPlaceId(IGenPlace place)
    {
        if (!string.IsNullOrEmpty(place.GOV_ID))
            return place.GOV_ID!;

        // Fallback: use the name as ID (deterministic for same places)
        if (!string.IsNullOrEmpty(place.Name))
            return place.Name;

        // Ultimate fallback only when both GOV_ID and Name are null/empty
        return $"PLACE_{Guid.NewGuid():N}";
    }
}
