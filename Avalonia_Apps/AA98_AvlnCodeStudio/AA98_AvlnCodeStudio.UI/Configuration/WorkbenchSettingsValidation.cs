using AA98_AvlnCodeStudio.Base.AI.Services;
using Config.UI;
using Config.Service;
using Property.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Configuration;

/// <summary>Identifies an invalid configured profile endpoint without including its value.</summary>
public sealed record AIProfileEndpointIssue(AIProviderPreset Provider, string Message);

/// <summary>Validates the workbench AI draft before it is persisted.</summary>
public static class WorkbenchSettingsValidation
{
    /// <summary>Checks every nonempty profile endpoint, including inactive profiles.</summary>
    public static IReadOnlyList<AIProfileEndpointIssue> GetInvalidProfileEndpoints(IConfigUiSection section)
    {
        ArgumentNullException.ThrowIfNull(section);
        List<AIProfileEndpointIssue> issues = [];
        foreach (AIProviderPreset candidate in Enum.GetValues<AIProviderPreset>())
        {
            string? configuredEndpoint = section.Properties.FirstOrDefault(
                property => property.Name == CodeStudioSettings.GetProfileEndpointPropertyName(candidate))?.Value as string;
            if (string.IsNullOrWhiteSpace(configuredEndpoint))
            {
                continue;
            }

            try
            {
                AIEndpointValidator.Validate(configuredEndpoint);
            }
            catch (InvalidOperationException exception)
            {
                issues.Add(new AIProfileEndpointIssue(candidate, exception.Message));
            }
        }

        return issues;
    }

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

        object? providerValue = section.Properties
            .Single(property => property.Name == nameof(CodeStudioSettings.AIProvider)).Value;
        if (providerValue is not AIProviderPreset provider || !Enum.IsDefined(provider))
        {
            return "Choose a valid AI provider.";
        }

        bool useProfiles = section.Properties.FirstOrDefault(
            property => property.Name == nameof(CodeStudioSettings.UseAIProviderProfiles))?.Value is true;
        IReadOnlyList<AIProfileEndpointIssue> profileIssues = GetInvalidProfileEndpoints(section);
        if (profileIssues.Count > 0)
        {
            string details = string.Join(Environment.NewLine,
                profileIssues.Select(static issue => $"{issue.Provider} profile: {issue.Message}"));
            return $"{details}{Environment.NewLine}In Workbench settings, select 'Show all provider fields' to inspect these endpoints without changing the selected provider.";
        }

        string endpointProperty = useProfiles
            ? CodeStudioSettings.GetProfileEndpointPropertyName(provider)
            : nameof(CodeStudioSettings.AIEndpoint);
        IPropertyItem? endpointItem = section.Properties.FirstOrDefault(property => property.Name == endpointProperty);
        if (endpointItem is null)
        {
            return "The selected AI endpoint setting is unavailable.";
        }

        string? endpoint = endpointItem.Value as string;
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
