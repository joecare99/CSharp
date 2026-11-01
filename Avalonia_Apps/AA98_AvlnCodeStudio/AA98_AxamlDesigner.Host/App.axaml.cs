using AA98_AxamlDesigner.Host.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Linq;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AxamlDesigner.Host.Commands;
using AA98_AxamlDesigner.Host.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AA98_AxamlDesigner.Host;

public partial class App : Application
{
    private IHost? _mcpHost;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            AxamlDesignerHostViewModel viewModel = new();
            string[] arguments = desktop.Args ?? [];
            bool mcpEnabled = arguments.Contains("--mcp", StringComparer.Ordinal);
            string? initialPath = arguments.FirstOrDefault(argument => argument != "--mcp");
            if (!string.IsNullOrWhiteSpace(initialPath))
            {
                viewModel.OpenDocument(initialPath);
            }

            if (mcpEnabled)
            {
                StartMcpServer(viewModel, desktop);
            }

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void StartMcpServer(AxamlDesignerHostViewModel viewModel, IClassicDesktopStyleApplicationLifetime desktop)
    {
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder([]);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
            options.LogToStandardErrorThreshold = LogLevel.Trace);
        builder.Services.AddSingleton(viewModel);
        builder.Services.AddAxamlDesignerMcpExports();
        builder.Services.AddComponentMcpServer("AA98 AXAML Designer", "1.0.0");
        _mcpHost = builder.Build();
        _mcpHost.StartAsync().GetAwaiter().GetResult();
        desktop.Exit += OnDesktopExit;
    }

    private async void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        if (_mcpHost is not null)
        {
            await _mcpHost.StopAsync();
            _mcpHost.Dispose();
            _mcpHost = null;
        }
    }
}
