using AA98_AvlnCodeStudio.Base.AI.Models;
using Config.Service;
using Config.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Configuration;

/// <summary>Reloads saved workbench settings and activates their runtime consumers.</summary>
public sealed class WorkbenchSettingsRuntimeActivator
{
    private readonly ConfigLayeredSettingsResolver _resolver;
    private readonly CodeStudioSettings _settings;
    private readonly OpenAICompatibleAISettings _aiSettings;

    public WorkbenchSettingsRuntimeActivator(
        ConfigLayeredSettingsResolver resolver,
        CodeStudioSettings settings,
        OpenAICompatibleAISettings aiSettings)
    {
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _aiSettings = aiSettings ?? throw new ArgumentNullException(nameof(aiSettings));
    }

    public async Task ActivateAsync(string sectionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        if (!string.Equals(sectionName, "Workbench", StringComparison.Ordinal))
        {
            return;
        }

        IReadOnlyDictionary<string, ConfigValueResolution> values =
            await _resolver.ResolveSectionAsync("Workbench").ConfigureAwait(false);
        ApplyResolvedValues(_settings, values);
        string endpoint = _settings.GetEffectiveAIEndpoint();
        if (!string.Equals(_aiSettings.Endpoint, endpoint, StringComparison.Ordinal))
        {
            _aiSettings.ApiKey = string.Empty;
        }

        _aiSettings.Endpoint = endpoint;
        _aiSettings.ModelId = _settings.GetEffectiveAIModelId();
    }

    public Task<string?> ValidateChangesAsync(IReadOnlyList<IConfigUiSection> sections) =>
        WorkbenchSettingsValidation.ValidateAsync(sections, _settings, _resolver);

    public async Task<IReadOnlyList<AIProviderTemplate>?> LoadProviderTemplatesAsync()
    {
        IReadOnlyDictionary<string, ConfigValueResolution> values =
            await _resolver.ResolveSectionAsync("Workbench").ConfigureAwait(false);
        return values[nameof(CodeStudioSettings.KnownAIProviderTemplates)].Value as IReadOnlyList<AIProviderTemplate>;
    }

    private static void ApplyResolvedValues(
        CodeStudioSettings settings,
        IReadOnlyDictionary<string, ConfigValueResolution> values)
    {
        foreach (PropertyInfo property in typeof(CodeStudioSettings).GetProperties(
                     BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.CanWrite
                && property.GetIndexParameters().Length == 0
                && values.TryGetValue(property.Name, out ConfigValueResolution? resolution))
            {
                property.SetValue(settings, resolution.Value);
            }
        }
    }
}
