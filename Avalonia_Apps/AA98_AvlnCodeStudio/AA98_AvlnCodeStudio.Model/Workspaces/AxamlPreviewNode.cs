using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// A safe, UI-neutral representation of an AXAML control used for design preview.
/// </summary>
public sealed class AxamlPreviewNode
{
    public string TypeName { get; init; } = string.Empty;

    public string? StableName { get; init; }

    public string? ParentTypeName { get; init; }

    public int ParentSourceStartOffset { get; init; } = -1;

    public bool ParentHasSafeGridLayout { get; init; }

    public string? Text { get; init; }

    public bool IsSupported { get; init; }

    public bool IsRecoveryRoot { get; init; }

    public string? Diagnostic { get; init; }

    public int SourceStartOffset { get; init; } = -1;

    public string? SourceStartTag { get; init; }

    public bool HasTextPropertyElement { get; init; }

    public bool HasUnsafeTextWrappingSource { get; init; }

    public bool HasUnsafeTextAlignmentSource { get; init; }

    public bool HasInlineText { get; init; }

    public bool HasChildElements { get; init; }

    public bool CanAppendChildren { get; init; }

    public bool CanAddGridChild { get; init; }

    public bool HasSafeGridLayout { get; init; }

    public bool CanRemoveSingleChild { get; init; }

    public IReadOnlyList<string> PropertyElementNames { get; init; } = [];

    public IReadOnlyDictionary<string, string> Properties { get; init; } =
        new Dictionary<string, string>();

    /// <summary>Inert preview values; source editing always uses Properties.</summary>
    public IReadOnlyDictionary<string, string>? PreviewProperties { get; init; }

    public IReadOnlyDictionary<string, string> RenderProperties => PreviewProperties ?? Properties;

    /// <summary>A safe literal stacking value; null means the default stacking order.</summary>
    public int? PreviewZIndex { get; init; }

    /// <summary>A bounded, literal gradient for a Border background, never executable XAML.</summary>
    public AxamlPreviewLinearGradient? BorderBackgroundGradient { get; init; }

    /// <summary>A bounded application-style gradient for a Button background.</summary>
    public AxamlPreviewLinearGradient? ButtonBackgroundGradient { get; init; }

    public IReadOnlyList<AxamlPreviewNode> Children { get; init; } = [];

    public IReadOnlyList<string> RowDefinitions { get; init; } = [];

    public IReadOnlyList<string> ColumnDefinitions { get; init; } = [];
}

public sealed record AxamlPreviewGradientStop(double Offset, string Color);

public sealed record AxamlPreviewLinearGradient(
    double StartX, double StartY, double EndX, double EndY,
    IReadOnlyList<AxamlPreviewGradientStop> Stops);
