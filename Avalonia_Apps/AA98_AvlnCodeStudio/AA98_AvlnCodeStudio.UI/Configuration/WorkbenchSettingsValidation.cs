using AA98_AvlnCodeStudio.Base.AI.Services;
using Config.UI;
using Config.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Configuration;

/// <summary>Validates the workbench AI draft before it is persisted.</summary>
public static class WorkbenchSettingsValidation
{
    public static async Task<string?> ValidateAsync(
        IReadOnlyList<IConfigUiSection> sections,
        CodeStudioSettings currentSettings,
        ConfigLayeredSettingsResolver? resolver = null)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(currentSettings);
        IConfigUiSection? section = sections.SingleOrDefault(
            static section => string.Equals(section.Section.Name, "Workbench", StringComparison.Ordinal));
        if (section is null)
        {
            return null;
        }

        string? endpoint = section.Properties
            .Single(property => property.Name == nameof(CodeStudioSettings.AIEndpoint)).Value as string;
        object? providerValue = section.Properties
            .Single(property => property.Name == nameof(CodeStudioSettings.AIProvider)).Value;
        if (providerValue is not AIProviderPreset provider || !Enum.IsDefined(provider))
        {
            return "Choose a valid AI provider.";
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            if (provider == AIProviderPreset.Custom)
            {
                return "Enter an AI endpoint for the custom provider.";
            }

            List<AIProviderTemplate>? templates = currentSettings.KnownAIProviderTemplates;
            if (resolver is not null)
            {
                IReadOnlyDictionary<string, ConfigValueResolution> resolved = await resolver.ResolveSectionAsync("Workbench");
                templates = resolved[nameof(CodeStudioSettings.KnownAIProviderTemplates)].Value as List<AIProviderTemplate>;
            }

            AIProviderTemplate? template = templates?
                .Find(item => item is not null && string.Equals(item.Id, provider.ToString(), StringComparison.OrdinalIgnoreCase));
            if (template is null || string.IsNullOrWhiteSpace(template.Endpoint))
            {
                return $"The {provider} provider template has no endpoint. Configure an endpoint override or repair the system template.";
            }

            endpoint = template.Endpoint;
        }

        try
        {
            AIEndpointValidator.Validate(endpoint);
            return null;
        }
        catch (InvalidOperationException exception)
        {
            return exception.Message;
        }
    }
}
