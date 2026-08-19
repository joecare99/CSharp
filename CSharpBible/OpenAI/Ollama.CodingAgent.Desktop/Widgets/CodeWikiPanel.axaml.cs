using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Avalonia.Controls;

namespace Ollama.CodingAgent.Desktop.Widgets;

/// <summary>
/// Provides desktop entry points for local CodeWikiVault import and search.
/// </summary>
public sealed partial class CodeWikiPanel : UserControl
{
    /// <summary>
    /// Initializes the CodeWikiVault panel.
    /// </summary>
    public CodeWikiPanel()
    {
        InitializeComponent();
    }
}
