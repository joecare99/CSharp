using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Cxaml.Designer.Preview;

/// <summary>Resolves terminal cells to the front-most visible control that covers them.</summary>
public sealed class ConsolePreviewHitTester
{
    private readonly IReadOnlyList<PreviewControlMapping> _mappings;

    public ConsolePreviewHitTester(IReadOnlyList<PreviewControlMapping> mappings)
    {
        _mappings = mappings ?? throw new ArgumentNullException(nameof(mappings));
    }

    public PreviewControlMapping? HitTest(int x, int y)
    {
        PreviewControlMapping? result = null;
        foreach (var mapping in _mappings)
        {
            if (!Covers(mapping.ConsoleControl, x, y) ||
                !IsBetterMatch(mapping, result))
                continue;

            result = mapping;
        }

        return result;
    }

    private static bool Covers(IControl control, int x, int y)
    {
        if (!control.IsVisible)
            return false;

        var width = Math.Max(1, control.size.Width);
        var height = Math.Max(1, control.size.Height);
        return x >= control.Position.X &&
               y >= control.Position.Y &&
               x < control.Position.X + width &&
               y < control.Position.Y + height;
    }

    private static bool IsBetterMatch(
        PreviewControlMapping candidate,
        PreviewControlMapping? current)
    {
        if (current is null)
            return true;

        var candidateDepth = candidate.SourcePath.Count;
        var currentDepth = current.SourcePath.Count;
        if (candidateDepth != currentDepth)
            return candidateDepth > currentDepth;

        for (var index = 0; index < candidateDepth; index++)
        {
            if (candidate.SourcePath[index] != current.SourcePath[index])
                return candidate.SourcePath[index] < current.SourcePath[index];
        }

        return false;
    }
}
