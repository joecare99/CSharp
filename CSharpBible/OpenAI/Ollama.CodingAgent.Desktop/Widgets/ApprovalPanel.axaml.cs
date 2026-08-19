using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Avalonia.Controls;

namespace Ollama.CodingAgent.Desktop.Widgets;

/// <summary>
/// Displays and resolves shared pending operation approvals.
/// </summary>
public sealed partial class ApprovalPanel : UserControl
{
    /// <summary>
    /// Initializes the approval panel.
    /// </summary>
    public ApprovalPanel()
    {
        InitializeComponent();
    }
}
