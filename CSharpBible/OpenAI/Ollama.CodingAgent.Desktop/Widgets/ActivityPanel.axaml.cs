using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Avalonia.Controls;

namespace Ollama.CodingAgent.Desktop.Widgets;

/// <summary>
/// Shows activity data exposed by the shared layer and explicit placeholders for deferred data.
/// </summary>
public sealed partial class ActivityPanel : UserControl
{
    /// <summary>
    /// Initializes the activity panel.
    /// </summary>
    public ActivityPanel()
    {
        InitializeComponent();
    }
}
