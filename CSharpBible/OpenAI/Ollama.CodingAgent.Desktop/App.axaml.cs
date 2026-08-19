using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;

using Ollama.CodingAgent.Desktop.Host;
using Ollama.CodingAgent.Desktop.Views;

namespace Ollama.CodingAgent.Desktop;

/// <summary>
/// Initializes the Avalonia desktop application and resolves the DI-composed main window.
/// </summary>
public sealed class App : Avalonia.Application
{
    /// <inheritdoc />
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = DesktopComposition.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
