using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using Avln_Bubbles.View.Services;
using Avln_Bubbles.View.ViewModels;
using Avln_Bubbles.View.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Avln_Bubbles.View;

/// <summary>
/// Avalonia application root for the shared Bubbles UI.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the configured services.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    /// <summary>
    /// Gets or sets the requested host kind.
    /// </summary>
    public HostPlatformKind HostKind { get; set; } = HostPlatformKind.Desktop;

    /// <inheritdoc/>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                HostKind = HostPlatformKind.Desktop;
                Services = CreateServices(HostKind);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>()
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                HostKind = HostPlatformKind.Browser;
                Services = CreateServices(HostKind);
                singleViewPlatform.MainView = new BrowserMainView
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>()
                };
            }
        }
        catch (Exception ex)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    Content = CreateStartupErrorView(ex)
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = CreateStartupErrorView(ex);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static Control CreateStartupErrorView(Exception ex)
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.Parse("#15111E")),
            Padding = new Thickness(24),
            Child = new ScrollViewer
            {
                Content = new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Browser startup failed",
                            FontSize = 28,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = Brushes.Orange
                        },
                        new TextBlock
                        {
                            Text = ex.Message,
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = Brushes.White
                        },
                        new SelectableTextBlock
                        {
                            Text = ex.ToString(),
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = Brushes.Gainsboro,
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                    }
                }
            }
        };
    }

    private static IServiceProvider CreateServices(HostPlatformKind hostKind)
    {
        var services = new ServiceCollection();
        services
            .AddSingleton<IHostFeatureDescriptor>(_ => new HostFeatureDescriptor(hostKind))
            .AddSingleton<IGameSessionFactory, GameSessionFactory>()
            .AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
