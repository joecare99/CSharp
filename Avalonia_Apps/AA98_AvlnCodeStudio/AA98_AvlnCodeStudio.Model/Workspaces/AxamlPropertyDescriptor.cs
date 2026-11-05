using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public enum AxamlPropertyEditorKind
{
    Text,
    Boolean,
    Number,
    Enum,
}

public sealed record AxamlPropertyDescriptor(
    string Name,
    string Value,
    AxamlPropertyEditorKind EditorKind,
    IReadOnlyList<string> Options,
    bool IsReadOnly,
    string? ReadOnlyReason,
    string? ValidationError);
