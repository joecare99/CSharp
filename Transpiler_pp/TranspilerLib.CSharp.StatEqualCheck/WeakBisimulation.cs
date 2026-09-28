using System;
using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.CSharp.StatEqualCheck;

internal static class WeakBisimulation
{
    public static bool AreEquivalent(FlowGraph left, FlowGraph right, out int mismatchLeft, out int mismatchRight)
    {
        var leftTransitions = left.Nodes.Select(node => GetWeakTransitions(left, node.Id)).ToArray();
        var rightTransitions = right.Nodes.Select(node => GetWeakTransitions(right, node.Id)).ToArray();
        var leftTerminates = left.Nodes.Select(node => CanTerminateSilently(left, node.Id)).ToArray();
        var rightTerminates = right.Nodes.Select(node => CanTerminateSilently(right, node.Id)).ToArray();
        var relation = new bool[left.Nodes.Count, right.Nodes.Count];
        for (var i = 0; i < left.Nodes.Count; i++)
            for (var j = 0; j < right.Nodes.Count; j++)
                relation[i, j] = true;

        bool changed;
        do
        {
            changed = false;
            for (var leftId = 0; leftId < left.Nodes.Count; leftId++)
            {
                for (var rightId = 0; rightId < right.Nodes.Count; rightId++)
                {
                    if (!relation[leftId, rightId])
                        continue;
                    if (leftTerminates[leftId] != rightTerminates[rightId] ||
                        !TransitionsMatch(leftTransitions[leftId], rightTransitions[rightId], relation))
                    {
                        relation[leftId, rightId] = false;
                        changed = true;
                    }
                }
            }
        } while (changed);

        if (relation[left.Entry, right.Entry])
        {
            mismatchLeft = -1;
            mismatchRight = -1;
            return true;
        }

        var mismatch = FindMismatch(left.Entry, right.Entry, leftTransitions, rightTransitions,
            leftTerminates, rightTerminates, relation, []);
        mismatchLeft = mismatch.Left;
        mismatchRight = mismatch.Right;
        return false;
    }

    private static bool TransitionsMatch(IReadOnlyList<WeakTransition> left,
        IReadOnlyList<WeakTransition> right, bool[,] relation) =>
        EveryTransitionHasMatch(left, right, relation, reverse: false) &&
        EveryTransitionHasMatch(right, left, relation, reverse: true);

    private static bool EveryTransitionHasMatch(IReadOnlyList<WeakTransition> transitions,
        IReadOnlyList<WeakTransition> candidates, bool[,] relation, bool reverse)
    {
        foreach (var transition in transitions)
        {
            if (!candidates.Any(candidate => candidate.Label == transition.Label &&
                (reverse ? relation[candidate.Target, transition.Target] : relation[transition.Target, candidate.Target])))
                return false;
        }
        return true;
    }

    private static (int Left, int Right) FindMismatch(int leftId, int rightId,
        IReadOnlyList<WeakTransition>[] leftTransitions, IReadOnlyList<WeakTransition>[] rightTransitions,
        bool[] leftTerminates, bool[] rightTerminates, bool[,] relation, HashSet<(int, int)> visited)
    {
        if (!visited.Add((leftId, rightId)) || leftTerminates[leftId] != rightTerminates[rightId])
            return (leftId, rightId);

        foreach (var transition in leftTransitions[leftId])
        {
            var candidates = rightTransitions[rightId].Where(item => item.Label == transition.Label).ToArray();
            if (candidates.Length == 0)
                return (leftId, rightId);
            if (candidates.Any(candidate => relation[transition.Target, candidate.Target]))
                continue;
            var failedCandidate = candidates[0];
            return FindMismatch(transition.Target, failedCandidate.Target, leftTransitions, rightTransitions,
                leftTerminates, rightTerminates, relation, visited);
        }

        foreach (var transition in rightTransitions[rightId])
        {
            var candidates = leftTransitions[leftId].Where(item => item.Label == transition.Label).ToArray();
            if (candidates.Length == 0)
                return (leftId, rightId);
            if (candidates.Any(candidate => relation[candidate.Target, transition.Target]))
                continue;
            var failedCandidate = candidates[0];
            return FindMismatch(failedCandidate.Target, transition.Target, leftTransitions, rightTransitions,
                leftTerminates, rightTerminates, relation, visited);
        }

        return (leftId, rightId);
    }

    private static IReadOnlyList<WeakTransition> GetWeakTransitions(FlowGraph graph, int node)
    {
        var transitions = new HashSet<WeakTransition>();
        foreach (var before in EpsilonClosure(graph, node).Order())
        {
            foreach (var edge in graph.Nodes[before].Edges)
            {
                if (edge.Label is null)
                    continue;
                foreach (var target in EpsilonClosure(graph, edge.Target).Order())
                    transitions.Add(new WeakTransition(edge.Label, target));
            }
        }
        return transitions.OrderBy(item => item.Label, StringComparer.Ordinal).ThenBy(item => item.Target).ToArray();
    }

    private static bool CanTerminateSilently(FlowGraph graph, int node) =>
        EpsilonClosure(graph, node).Any(id => graph.Nodes[id].Edges.Count == 0);

    private static HashSet<int> EpsilonClosure(FlowGraph graph, int start)
    {
        var visited = new HashSet<int>();
        var pending = new Stack<int>();
        pending.Push(start);
        while (pending.TryPop(out var current))
        {
            if (!visited.Add(current))
                continue;
            foreach (var edge in graph.Nodes[current].Edges)
                if (edge.Label is null)
                    pending.Push(edge.Target);
        }
        return visited;
    }

    private sealed class WeakTransition(string label, int target) : IEquatable<WeakTransition>
    {
        public string Label { get; } = label;
        public int Target { get; } = target;
        public bool Equals(WeakTransition? other) => other is not null && Label == other.Label && Target == other.Target;
        public override bool Equals(object? obj) => obj is WeakTransition other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(StringComparer.Ordinal.GetHashCode(Label), Target);
    }
}

