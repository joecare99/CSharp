using System;
using System.Collections.Generic;
using System.Linq;
using AA98_AvlnCodeStudio.Model.Workspaces;

namespace AA98_AvlnCodeStudio.UI.Services;

public sealed class AxamlMvvmSemanticResolution
{
    public AxamlMvvmSemanticResolution(
        string? projectPath,
        string? viewTypeName,
        string? viewModelTypeName,
        bool hasRuntimeDataContext,
        IEnumerable<AxamlMvvmSemanticMember> viewModelMembers,
        IEnumerable<string> diagnostics)
    {
        ProjectPath = projectPath;
        ViewTypeName = viewTypeName;
        ViewModelTypeName = viewModelTypeName;
        HasRuntimeDataContext = hasRuntimeDataContext;
        ViewModelMembers = viewModelMembers?.ToArray()
            ?? throw new ArgumentNullException(nameof(viewModelMembers));
        Diagnostics = diagnostics?.ToArray()
            ?? throw new ArgumentNullException(nameof(diagnostics));
    }

    public string? ProjectPath { get; }

    public string? ViewTypeName { get; }

    public string? ViewModelTypeName { get; }

    public bool HasRuntimeDataContext { get; }

    public IReadOnlyList<AxamlMvvmSemanticMember> ViewModelMembers { get; }

    public IReadOnlyList<string> Diagnostics { get; }

    public bool IsSuccessful => Diagnostics.Count == 0;

    public static AxamlMvvmSemanticResolution Failure(string? projectPath, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new AxamlMvvmSemanticResolution(
            projectPath,
            null,
            null,
            false,
            [],
            [message]);
    }
}
