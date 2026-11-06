using Config.UI.ConfigService;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.UI.Configuration;

public enum CredentialManagerSelection
{
    None,
    WindowsCredentialManager,
    LinuxSecretService,
}

public enum AIProviderPreset
{
    Custom,
    OpenAI,
    Ollama,
    IFM,
}

public sealed class AIProviderTemplate
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string DefaultModelId { get; set; } = string.Empty;
}

/// <summary>
/// Stores host-owned settings for the CodeStudio workbench configuration section.
/// </summary>
public sealed class CodeStudioSettings
{
    /// <summary>Gets or sets the OpenAI-compatible endpoint used by the internal AI chat.</summary>
    public string AIEndpoint { get; set; } = string.Empty;

    /// <summary>Gets or sets the model identifier used by the internal AI chat.</summary>
    public string AIModelId { get; set; } = string.Empty;

    /// <summary>Gets or sets the system-supplied provider profile used when endpoint/model overrides are empty.</summary>
    public AIProviderPreset AIProvider { get; set; } = AIProviderPreset.Ollama;

    /// <summary>Uses the separately configured provider values instead of the legacy global overrides.</summary>
    public bool UseAIProviderProfiles { get; set; }

    public string AIProfileOpenAIEndpoint { get; set; } = string.Empty;
    public string AIProfileOpenAIModelId { get; set; } = string.Empty;
    public string AIProfileOllamaEndpoint { get; set; } = string.Empty;
    public string AIProfileOllamaModelId { get; set; } = string.Empty;
    public string AIProfileIFMEndpoint { get; set; } = string.Empty;
    public string AIProfileIFMModelId { get; set; } = string.Empty;
    public string AIProfileCustomEndpoint { get; set; } = string.Empty;
    public string AIProfileCustomModelId { get; set; } = string.Empty;

    /// <summary>Gets or sets the explicitly selected credential manager.</summary>
    public CredentialManagerSelection CredentialManager { get; set; }

    /// <summary>
    /// Gets provider templates supplied by the system configuration. These contain public
    /// connection metadata only; credentials are always stored by the selected credential manager.
    /// </summary>
    [ConfigUiIgnore]
    public List<AIProviderTemplate> KnownAIProviderTemplates { get; set; } =
    [
        new()
        {
            Id = "openai",
            DisplayName = "OpenAI",
            Endpoint = "https://api.openai.com/v1/chat/completions",
            DefaultModelId = "gpt-4o-mini",
        },
        new()
        {
            Id = "ollama",
            DisplayName = "Ollama (local)",
            Endpoint = "http://localhost:11434/v1/chat/completions",
            DefaultModelId = "qwen2.5-coder:7b",
        },
        new()
        {
            Id = "ifm",
            DisplayName = "IFM",
            Endpoint = "https://api.ifm.ai/v1/chat/completions",
            DefaultModelId = "IFM/K2-Horizon-375B-A23B",
        },
    ];

    public string GetEffectiveAIEndpoint()
    {
        string endpoint = UseAIProviderProfiles ? GetProfileEndpoint(AIProvider) : AIEndpoint;
        return string.IsNullOrWhiteSpace(endpoint)
            ? GetSelectedProviderTemplate()?.Endpoint ?? string.Empty
            : endpoint;
    }

    public string GetEffectiveAIModelId()
    {
        string model = UseAIProviderProfiles ? GetProfileModel(AIProvider) : AIModelId;
        return string.IsNullOrWhiteSpace(model)
            ? GetSelectedProviderTemplate()?.DefaultModelId ?? string.Empty
            : model;
    }

    public string GetProfileEndpoint(AIProviderPreset provider) => provider switch
    {
        AIProviderPreset.OpenAI => AIProfileOpenAIEndpoint,
        AIProviderPreset.Ollama => AIProfileOllamaEndpoint,
        AIProviderPreset.IFM => AIProfileIFMEndpoint,
        AIProviderPreset.Custom => AIProfileCustomEndpoint,
        _ => throw new System.ArgumentOutOfRangeException(nameof(provider)),
    };

    public string GetProfileModel(AIProviderPreset provider) => provider switch
    {
        AIProviderPreset.OpenAI => AIProfileOpenAIModelId,
        AIProviderPreset.Ollama => AIProfileOllamaModelId,
        AIProviderPreset.IFM => AIProfileIFMModelId,
        AIProviderPreset.Custom => AIProfileCustomModelId,
        _ => throw new System.ArgumentOutOfRangeException(nameof(provider)),
    };

    public static string GetProfileEndpointPropertyName(AIProviderPreset provider) => provider switch
    {
        AIProviderPreset.OpenAI => nameof(AIProfileOpenAIEndpoint),
        AIProviderPreset.Ollama => nameof(AIProfileOllamaEndpoint),
        AIProviderPreset.IFM => nameof(AIProfileIFMEndpoint),
        AIProviderPreset.Custom => nameof(AIProfileCustomEndpoint),
        _ => throw new System.ArgumentOutOfRangeException(nameof(provider)),
    };

    public static string GetProfileModelPropertyName(AIProviderPreset provider) => provider switch
    {
        AIProviderPreset.OpenAI => nameof(AIProfileOpenAIModelId),
        AIProviderPreset.Ollama => nameof(AIProfileOllamaModelId),
        AIProviderPreset.IFM => nameof(AIProfileIFMModelId),
        AIProviderPreset.Custom => nameof(AIProfileCustomModelId),
        _ => throw new System.ArgumentOutOfRangeException(nameof(provider)),
    };

    private AIProviderTemplate? GetSelectedProviderTemplate() =>
        AIProvider == AIProviderPreset.Custom
            ? null
            : KnownAIProviderTemplates.Find(template =>
                string.Equals(template.Id, AIProvider.ToString(), System.StringComparison.OrdinalIgnoreCase));

    /// <summary>Gets or sets the initial workspace path shown by the workbench.</summary>
    [ConfigUiIgnore]
    public string? WorkspacePath { get; set; }

    /// <summary>Gets or sets whether files reopen when the workbench starts.</summary>
    [ConfigUiIgnore]
    public bool ReopenLastDocument { get; set; } = true;
}
