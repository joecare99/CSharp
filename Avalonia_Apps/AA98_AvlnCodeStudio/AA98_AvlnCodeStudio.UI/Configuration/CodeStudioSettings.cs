using Config.UI.ConfigService;

namespace AA98_AvlnCodeStudio.UI.Configuration;

/// <summary>
/// Stores host-owned settings for the CodeStudio workbench configuration section.
/// </summary>
public sealed class CodeStudioSettings
{
    /// <summary>Gets or sets the OpenAI-compatible endpoint used by the internal AI chat.</summary>
    public string AIEndpoint { get; set; } = "http://localhost:11434/v1/chat/completions";

    /// <summary>Gets or sets the model identifier used by the internal AI chat.</summary>
    public string AIModelId { get; set; } = string.Empty;

    /// <summary>Gets or sets the initial workspace path shown by the workbench.</summary>
    [ConfigUiIgnore]
    public string? WorkspacePath { get; set; }

    /// <summary>Gets or sets whether files reopen when the workbench starts.</summary>
    [ConfigUiIgnore]
    public bool ReopenLastDocument { get; set; } = true;
}
