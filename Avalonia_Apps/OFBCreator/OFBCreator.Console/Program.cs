using System;
using System.CommandLine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Console;


public static class Program
{
    /// <summary>
    /// Entry point for the OFB Creator console application.
    /// </summary>
    private static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection();

        // Register data source providers
        services.AddSingleton<GedComDataSource>();
        services.AddSingleton<WinAhnenDataSource>();
        services.AddSingleton<SecureStoreDataSource>();

        // Register the registry and populate with default providers
        services.AddSingleton<IFamilyDataSourceRegistry>(sp =>
        {
            var registry = new DefaultFamilyDataSourceRegistry();
            registry.RegisterProvider(sp.GetRequiredService<GedComDataSource>());
            registry.RegisterProvider(sp.GetRequiredService<WinAhnenDataSource>());
            registry.RegisterProvider(sp.GetRequiredService<SecureStoreDataSource>());
            return registry;
        });

        // Register document factory (wraps CSharpBible DocumentUtils)
        services.AddSingleton<IUserDocumentFactory, OFBDocumentFactory>();

        var serviceProvider = services.BuildServiceProvider();

        var rootCommand = new RootCommand("Ortsfamilienbuch Generator");
        var generateCmd = GenerateCommand(serviceProvider);
        var exportCmd = ExportCommand(serviceProvider);
        rootCommand.Add(generateCmd);
        rootCommand.Add(exportCmd);

        return await rootCommand.Parse(args).InvokeAsync();
    }

    /// <summary>
    /// Creates the 'generate' command with all GEDCOM-to-OFB options.
    /// </summary>
    private static Command GenerateCommand(IServiceProvider serviceProvider)
    {
        var inputOption = new Option<string>("--input", "Path to the source data file (GEDCOM or other format)");
        var outputOption = new Option<string>("--output", "Path to the output OFB document (.docx/.odt)");
        var titleOption = new Option<string>("--title", "Title of the Ortsfamilienbuch");
        var placeIdOption = new Option<string?>("--place-id", "GedCom reference ID for the primary place filter (optional)");
        var includeDescendants = new Option<bool>("--include-descendants", "Include descendants of families in this place");
        var prefaceOption = new Option<string?>("--preface", "Preface/introduction text for the OFB");
        var legendOption = new Option<string?>("--legend", "Character explanation/legend text");
        var docxOption = new Option<bool>("--docx", "Use DOCX format (default)");
        var odtOption = new Option<bool>("--odt", "Use ODF (ODT) format instead of DOCX");
        var dataSourceOption = new Option<string>("--data-source", "-s",
            "Data source provider: 'gedcom' (default), 'winahnen', 'securestore', or leave empty for auto-detect");

        var command = new Command("generate", "Generate OFB from genealogical data")
        {
            inputOption, outputOption, titleOption, placeIdOption,
            includeDescendants, prefaceOption, legendOption, docxOption, odtOption, dataSourceOption
        };

        command.SetAction(async context =>
        {
            var dataSource = string.IsNullOrWhiteSpace(context.GetValue(dataSourceOption))
                ? null
                : context.GetValue(dataSourceOption);

            var docxUsed = context.GetValue(docxOption);
            var odtUsed = context.GetValue(odtOption);

            var options = new OFBGenerateOptions()
            {
                InputPath = context.GetValue(inputOption)!,
                OutputPath = context.GetValue(outputOption)!,
                Title = context.GetValue(titleOption)!,
                PlaceId = context.GetValue(placeIdOption),
                IncludeDescendants = context.GetValue(includeDescendants),
                Preface = context.GetValue(prefaceOption),
                Legend = context.GetValue(legendOption),
                UseDocxFormat = docxUsed || (!odtUsed && !context.GetValue(odtOption)),
                DataSource = dataSource,
            };

            await RunExportAsync(options, serviceProvider);
        });

        return command;
    }

    /// <summary>
    /// Creates the 'export' command for loading from a JSON config file.
    /// </summary>
    private static Command ExportCommand(IServiceProvider serviceProvider)
    {
        var configOption = new Option<string>("--config");

        var command = new Command("export", "Export OFB using JSON configuration")
        {
            configOption
        };

        command.SetAction(context =>
        {
            var configPath = context.GetValue(configOption);
            if (string.IsNullOrEmpty(configPath))
            {
                System.Console.Error.WriteLine("Error: Config path is required");
                Environment.Exit(1);
                return;
            }

            var options = LoadConfigAsync(configPath).GetAwaiter().GetResult();
            if (options is null)
            {
                System.Console.Error.WriteLine($"Error: Failed to load config from '{configPath}'");
                Environment.Exit(1);
                return;
            }

            RunExportAsync(options, serviceProvider).GetAwaiter().GetResult();
        });

        return command;
    }

    /// <summary>
    /// Orchestrates the full OFB generation pipeline.
    /// </summary>
    private static async Task RunExportAsync(OFBGenerateOptions options, IServiceProvider serviceProvider)
    {
        try
        {
            System.Console.WriteLine("Starting OFB Creator...");
            System.Console.WriteLine($"  Input:    {options.InputPath}");
            System.Console.WriteLine($"  Output:   {options.OutputPath}");
            System.Console.WriteLine($"  Title:    {options.Title}");

            // Resolve data source provider (or auto-detect)
            var registry = serviceProvider.GetRequiredService<IFamilyDataSourceRegistry>();
            IFamilyDataSource dataSource;

            if (!string.IsNullOrWhiteSpace(options.DataSource))
            {
                dataSource = registry.GetProvider(options.DataSource)
                    ?? throw new ArgumentException($"Unknown data source: '{options.DataSource}'. Available: {string.Join(", ", registry.ListProviders().Select(p => p.SourceId))}");
                System.Console.WriteLine($"  Data Source: {dataSource.DisplayName}");
            }
            else
            {
                // Auto-detect by probing the stream with each provider's CanRead
                using var probeStream = new FileStream(options.InputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dataSource = registry.DetectProvider(probeStream)
                    ?? throw new ArgumentException($"Unable to detect data source for '{options.InputPath}'. Use --data-source to specify.");
                System.Console.WriteLine($"  Data Source: {dataSource.DisplayName} (auto-detected)");
            }

            var exportService = new ConsoleExportService(dataSource, serviceProvider.GetRequiredService<IUserDocumentFactory>());
            await exportService.ExportAsync(options);

            System.Console.WriteLine("OFB generation completed successfully.");
        }
        catch (Exception ex)
        {
            System.Console.Error.WriteLine($"\nError during OFB generation:");
            System.Console.Error.WriteLine(ex.Message);

            if (Environment.GetEnvironmentVariable("OFBCREATOR_DEBUG") == "1")
            {
                System.Console.Error.WriteLine($"\nStack trace:\n{ex.StackTrace}");
            }

            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Loads OFB generation options from a JSON configuration file.
    /// </summary>
    private static async Task<OFBGenerateOptions?> LoadConfigAsync(string configPath)
    {
        if (!File.Exists(configPath))
            return null;

        var json = await File.ReadAllTextAsync(configPath);
        return System.Text.Json.JsonSerializer.Deserialize<OFBGenerateOptions>(json);
    }
}
