using System;
using System.Collections.Generic;

namespace OFBCreator.Core.Services;

/// <summary>
/// Union-Find (disjoint set) data structure for efficient group merging of strings.
/// Uses path compression and union by rank for O(α(n)) operations.
/// </summary>
internal sealed class UnionFind
{
    private readonly Dictionary<string, string> _parent = new();
    private readonly Dictionary<string, int> _rank = new();

    public UnionFind(IEnumerable<string> elements)
    {
        foreach (var e in elements)
        {
            _parent[e] = e;
            _rank[e] = 0;
        }
    }

    /// <summary>
    /// Finds the representative of the set containing element.
    /// </summary>
    public string Find(string element)
    {
        if (!_parent.TryGetValue(element, out var parent))
            throw new KeyNotFoundException($"Unknown element: {element}");

        // Path compression
        while (!ReferenceEquals(_parent[parent], parent))
        {
            var grandparent = _parent[parent];
            _parent[parent] = grandparent;
            parent = grandparent;
        }

        return parent;
    }

    /// <summary>
    /// Unions the sets containing element1 and element2.
    /// </summary>
    public void Union(string element1, string element2)
    {
        var root1 = Find(element1);
        var root2 = Find(element2);

        if (ReferenceEquals(root1, root2))
            return;

        // Union by rank
        int rank1 = _rank.GetValueOrDefault(root1, 0);
        int rank2 = _rank.GetValueOrDefault(root2, 0);

        if (rank1 < rank2)
            _parent[root1] = root2;
        else if (rank1 > rank2)
            _parent[root2] = root1;
        else
        {
            _parent[root2] = root1;
            _rank[root1] = rank1 + 1;
        }
    }
}
