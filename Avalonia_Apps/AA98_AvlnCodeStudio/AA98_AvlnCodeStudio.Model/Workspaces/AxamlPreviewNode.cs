using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// A safe, UI-neutral representation of an AXAML control used for design preview.
/// </summary>
public sealed class AxamlPreviewNode
{
    public string TypeName { get; init; } = string.Empty;

    public string? Text { get; init; }

    public bool IsSupported { get; init; }

    public int SourceStartOffset { get; init; } = -1;

    public string? SourceStartTag { get; init; }

    public bool HasTextPropertyElement { get; init; }

    public bool HasInlineText { get; init; }

    public IReadOnlyList<string> PropertyElementNames { get; init; } = [];

    public IReadOnlyDictionary<string, string> Properties { get; init; } =
        new Dictionary<string, string>();

    public IReadOnlyList<AxamlPreviewNode> Children { get; init; } = [];

    public IReadOnlyList<string> RowDefinitions { get; init; } = [];

    public IReadOnlyList<string> ColumnDefinitions { get; init; } = [];
}
