using DataAnalysis.Core.Export;
using DataAnalysis.Core.Export.Interfaces;
using DataAnalysis.Core.Import;
using DataAnalysis.Core.Import.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataConvert.Console;

internal class App
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                // Exporter: table only
                services.AddSingleton<ITableExporter, TableExcelExporter>();
                services.AddSingleton<IConsoleWriter, ConsoleWriter>();

                services.AddSingleton<IDelimitedTableParsingProfile>(sp => new DelimitedTableParsingProfile
                {
                    HasHeaderRow = true,
                    Delimiter = '\t',
                    Quote = '\0',
                    TrimWhitespace = false,
                    FixedColumns =
                    {
                        new FixedColumnMapping { Source = "eventdatetime", Target = "Timestamp", IsDateTime = true },
                        new FixedColumnMapping { Source = "eventlevel", Target = "Severity" },
                        new FixedColumnMapping { Source = "eventhostname", Target = "Source" },
                        new FixedColumnMapping { Source = "EventName", Target = "Message" },
                    },
                    ExtractionRules =
                    {
                        new FieldExtractionRule { SourceColumn = "eventmessage", RegexPattern = "^(.+? +- +)?(?<EventName>.+?)(?<Rest>X=.*|;.*|$)" },
                        new FieldExtractionRule { SourceColumn = "Rest", RegexPattern = "(?<key>[A-Za-z][A-Za-z0-9_]*)=(?<value>[^;]*)(;(?<Rest>.*)|$)", Multible = true }
                    }
                });
                services.AddSingleton<DelimitedTableReader>();
                services.AddSingleton<ITableReader>(sp => sp.GetRequiredService<DelimitedTableReader>());
            })
            .Build();
    }

    IConsoleWriter? console;
    
    public async Task<int> RunAsync(string[] args)
    {
        await _host.StartAsync();
        try
        {
            if (args.Length == 0)
            {
                System.Console.Error.WriteLine("Usage: DataConvert.Console <input-file>");
                return 1;
            }
            var inputPath = args[0];
            var tableReader = _host.Services.GetRequiredService<ITableReader>();
            var exporter = _host.Services.GetRequiredService<ITableExporter>();
            console = _host.Services.GetRequiredService<IConsoleWriter>();
            console.WriteLine("Reading ...");
            var table = await tableReader.ReadTableAsync(inputPath, CancellationToken.None,onProgress);
            console.WriteLine("\nConverting ...");
            var output = await exporter.ExportAsync(table, inputPath, null, CancellationToken.None,onProgress);
            console.WriteLine("\n"+output);
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.Error.WriteLine($"Error: {ex.Message}");
            return 2;
        }
        finally
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }

    private void onProgress(double obj)
    {
        console.Write("[");
        int totalBlocks = 70;
        int filledBlocks = (int)(obj * totalBlocks);
        console.Write(new string('#', filledBlocks));
        console.Write(new string('-', totalBlocks - filledBlocks));
        console.Write($"] {obj:P0}\r");
    }
}
