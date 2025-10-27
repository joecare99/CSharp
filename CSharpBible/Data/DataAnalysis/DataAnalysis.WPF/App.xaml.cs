using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DataAnalysis.Core.Models;
using DataAnalysis.Core.Export;
using DataAnalysis.Core.Import;

namespace DataAnalysis.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost AppHost { get; private set; } = null!;

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                // Exporter (später optional durch UI ausgelöst)
                services.AddSingleton<ISyslogAnalysisExporter, SyslogExcelExporter>();

                services.AddSingleton<IDelimitedTableParsingProfile>(sp => new DelimitedTableParsingProfile
                {
                    HasHeaderRow = true,
                    Delimiter = '\t',
                    Quote = '\0',
                    TrimWhitespace = false,
                    FixedColumns =
                    {
                        new FixedColumnMapping { Source = "eventdatetime", Target = "Timestamp" , IsDateTime = true},
                        new FixedColumnMapping { Source = "eventlevel", Target = "Severity" },
                        new FixedColumnMapping { Source = "eventhostname", Target = "Source" },
                        new FixedColumnMapping { Source = "EventName", Target = "Message" },
                    },
                    ExtractionRules =
                    {
                        new FieldExtractionRule { SourceColumn = "eventmessage", RegexPattern = "-\\s*(?<EventName>.+?)(?<Rest>X=.*|;.*|$)" },
                        new FieldExtractionRule { SourceColumn = "Rest", RegexPattern = "(?<key>[A-Za-z][A-Za-z0-9_]*)=(?<value>[^;]*)(;(?<Rest>.*)|$)", Multible=true  }
                    }
                });
                services.AddSingleton<DelimitedTableReader>();
                services.AddSingleton<ITableReader>(sp => sp.GetRequiredService<DelimitedTableReader>());
                services.AddSingleton<ISyslogEntryReader>(sp =>
                {
                    var table = sp.GetRequiredService<ITableReader>();
                    return new TableToEntryAdapter(table, (raw, text) => Enum.TryParse<SyslogSeverity>(text, true, out var sev) ? sev : SyslogSeverity.Unknown);
                });

                // ViewModels
                services.AddSingleton<ViewModels.MainViewModel>();
                // Models aus Core (nutzt weiterhin ISyslogEntryReader)
                services.AddSingleton<AnalysisModel>();
                // Views
                services.AddSingleton<Views.MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();

        var mainWindow = AppHost.Services.GetRequiredService<Views.MainWindow>();
        mainWindow.DataContext = AppHost.Services.GetRequiredService<ViewModels.MainViewModel>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        AppHost.Dispose();
        base.OnExit(e);
    }
}

