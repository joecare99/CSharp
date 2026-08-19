namespace Ollama.Wpf.TextAnalysis.Services;

/// <summary>
/// Represents text loaded from a user-selected file.
/// </summary>
public sealed record TextFileSelection(string FilePath, string Content);
