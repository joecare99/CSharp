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
    public string? Description => "Choose an AI provider and credential manager. Non-empty endpoint and model overrides take precedence over the selected provider template; clear them to use the template values.";

    /// <inheritdoc/>
    public int Order => 0;

    /// <inheritdoc/>
    public Type ModelType => typeof(CodeStudioSettings);

    /// <inheritdoc/>
    public object CreateModel() => new CodeStudioSettings();
}
