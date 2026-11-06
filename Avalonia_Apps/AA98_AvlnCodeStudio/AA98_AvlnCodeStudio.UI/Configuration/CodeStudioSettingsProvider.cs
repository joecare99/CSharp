using Config.Service;
using System;

namespace AA98_AvlnCodeStudio.UI.Configuration;

/// <summary>
/// Supplies CodeStudio-owned metadata and defaults to the shared configuration service.
/// </summary>
public sealed class CodeStudioSettingsProvider : IConfigSectionProvider
{
    /// <inheritdoc/>
    public string Name => "Workbench";

    /// <inheritdoc/>
    public string DisplayName => "Workbench settings";

    /// <inheritdoc/>
    public string? Description => "Choose an AI provider and credential manager. Enable provider profiles to use separate values per provider; otherwise the legacy global overrides apply. Empty values use the selected template.";

    /// <inheritdoc/>
    public int Order => 0;

    /// <inheritdoc/>
    public Type ModelType => typeof(CodeStudioSettings);

    /// <inheritdoc/>
    public object CreateModel() => new CodeStudioSettings();
}
