using System;
using System.CommandLine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Document.Base.Factories;
using Document.Docx;
using Genealogy.Drivers;
using Genealogy.Gedcom;
using OFBCreator.Publishing.Models;
using OFBCreator.Publishing.Services;
using OFBCreator.Publishing.Services.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;
using PortableProjectStore = OFBCreator.Projects.Services.OFBProjectStore;

namespace OFBCreator.Console;


public static class Program
{
    /// <summary>
    /// Entry point for the OFB Creator console application.
    /// </summary>
    private static async Task<int> Main(string[] args)
    {
        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });

        var services = new ServiceCollection();

        // Register provider-neutral genealogy import and output drivers.
        services.AddSingleton<IGenealogyInputDriver, GedcomInputDriver>();
        services.AddSingleton<IGenealogyOutputDriver, GedcomOutputDriver>();

        // Register data source providers
        services.AddSingleton<CanonicalGenealogyAdapter>();
        services.AddSingleton<CanonicalGedcomFamilyDataSource>();
        services.AddSingleton<PortableProjectStore>();
        services.AddSingleton<EntryTemplateStore>();
        services.AddSingleton<Func<IFamilyDataSource, OFBExportService>>(provider => dataSource =>
            new OFBExportService(
                dataSource,
                provider.GetRequiredService<IUserDocumentFactory>(),
                provider.GetRequiredService<EntryTemplateStore>()));
        services.AddSingleton<WinAhnenDataSource>();
        services.AddSingleton<SecureStoreDataSource>();

        // Register the registry and populate with default providers
        services.AddSingleton<IFamilyDataSourceRegistry>(sp =>
        {
            var registry = new DefaultFamilyDataSourceRegistry();
            registry.RegisterProvider(sp.GetRequiredService<CanonicalGedcomFamilyDataSource>());
            registry.RegisterProvider(sp.GetRequiredService<WinAhnenDataSource>());
            registry.RegisterProvider(sp.GetRequiredService<SecureStoreDataSource>());
            return registry;
        });

        // Register document factory (wraps CSharpBible DocumentUtils)
        services.AddSingleton<IUserDocumentFactory, UserDocumentFactoryImpl>();

        var serviceProvider = services.BuildServiceProvider();

        var rootCommand = new RootCommand("Ortsfamilienbuch Generator");
        var generateCmd = GenerateCommand(serviceProvider);
        var exportCmd = ExportCommand(serviceProvider);
        var roundTripCmd = GedcomRoundTripCommand(serviceProvider);
        var migrateProjectCmd = MigrateProjectCommand(serviceProvider);
        rootCommand.Add(generateCmd);
        rootCommand.Add(exportCmd);
        rootCommand.Add(roundTripCmd);
        rootCommand.Add(migrateProjectCmd);

        return await rootCommand.Parse(args).InvokeAsync();
    }

    /// <summary>
    /// Creates the 'generate' command with all GEDCOM-to-OFB options.
    /// </summary>
    private static Command GenerateCommand(IServiceProvider serviceProvider)
    {
        var inputOption = new Option<string?>("--input") { Description = "Override project source path" };
        var outputOption = new Option<string?>("--output") { Description = "Override project output path" };
        var titleOption = new Option<string?>("--title") { Description = "Title of the Ortsfamilienbuch (defaults to the selected project name)" };
        var projectOption = new Option<string?>("--project") { Description = "Portable .ofbproject path or catalog name; a missing project is created" };
        var projectTemplateOption = new Option<string?>("--project-template") { Description = "Initialize a new project from an existing project path or catalog name" };
        var placeIdOption = new Option<string?>("--place-id") { Description = "GedCom reference ID for the primary place filter (optional)" };
        var includeDescendants = new Option<bool>("--include-descendants") { Description = "Include descendants of families in this place" };
        var excludeDescendants = new Option<bool>("--no-include-descendants")
        {
            Description = "Disable the project's include-descendants setting for this export"
        };
        var prefaceOption = new Option<string?>("--preface") { Description = "Preface/introduction text for the OFB" };
        var legendOption = new Option<string?>("--legend") { Description = "Character explanation/legend text" };
        var templateOption = new Option<string?>("--template") { Description = "Override the project entry template: 'gc', 'ak', or a JSON template path" };
        var docxOption = new Option<bool>("--docx") { Description = "Use DOCX format (default)" };
        var odtOption = new Option<bool>("--odt") { Description = "Use ODF (ODT) format instead of DOCX" };
        var dataSourceOption = new Option<string>("--data-source", "-s")
        {
            Description = "Data source provider: 'gedcom' (default), 'winahnen', 'securestore', or leave empty for auto-detect"
        };

        var command = new Command("generate", "Generate OFB from genealogical data")
        {
            inputOption, outputOption, titleOption, projectOption, projectTemplateOption, placeIdOption,
            includeDescendants, excludeDescendants, prefaceOption, legendOption, templateOption, docxOption,
            odtOption, dataSourceOption
        };

        command.SetAction(async context =>
        {
            var dataSourceOverride = string.IsNullOrWhiteSpace(context.GetValue(dataSourceOption))
                ? null
                : context.GetValue(dataSourceOption);

            var projectName = context.GetValue(projectOption);
            var projectTemplateName = context.GetValue(projectTemplateOption);
            if (string.IsNullOrWhiteSpace(projectName) && !string.IsNullOrWhiteSpace(projectTemplateName))
                throw new ArgumentException("--project-template can only be used together with --project.");
            if (context.GetValue(includeDescendants) && context.GetValue(excludeDescendants))
                throw new ArgumentException("--include-descendants and --no-include-descendants cannot be used together.");

            var projectResult = string.IsNullOrWhiteSpace(projectName)
                ? null
                : serviceProvider.GetRequiredService<PortableProjectStore>().LoadOrCreate(projectName, projectTemplateName);
            var project = projectResult?.Project;
            var dataSource = dataSourceOverride ?? project?.DataSource;
            if (projectResult?.RequiresMigrationSave == true)
                System.Console.Error.WriteLine(
                    $"Legacy project '{projectResult.ProjectPath}' was loaded without changes. Use 'project-migrate --project \"{projectResult.ProjectPath}\" --output <path.ofbproject>' to save the portable version.");

            var inputPath = context.GetValue(inputOption)
                ?? PortableProjectStore.ResolveProjectPath(projectResult?.ProjectPath ?? string.Empty, project?.InputPath);
            var outputPath = context.GetValue(outputOption)
                ?? PortableProjectStore.ResolveProjectPath(projectResult?.ProjectPath ?? string.Empty, project?.OutputPath);
            var title = context.GetValue(titleOption) ?? project?.Title;
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Specify --title or select a named --project.");
            if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Specify --input and --output, or set both paths in the selected project.");

            var docxUsed = context.GetValue(docxOption);
            var odtUsed = context.GetValue(odtOption);
            var template = context.GetValue(templateOption)
                ?? (project is null
                    ? "gc"
                    : PortableProjectStore.ResolveEntryTemplate(projectResult!.ProjectPath, project.EntryTemplate));

            var options = new OFBGenerateOptions()
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = title,
                PlaceId = context.GetValue(placeIdOption) ?? project?.PlaceId,
                IncludeDescendants = context.GetValue(includeDescendants)
                    || (!context.GetValue(excludeDescendants) && project?.IncludeDescendants == true),
                Preface = context.GetValue(prefaceOption) ?? project?.Preface,
                Legend = context.GetValue(legendOption) ?? project?.Legend,
                Template = template,
                UseDocxFormat = docxUsed || (!odtUsed && !context.GetValue(odtOption)),
                DataSource = dataSource,
                ExportRules = project?.ExportRules ?? [],
                GroupingPolicy = project?.GroupingPolicy ?? new OFBGroupingPolicy(),
                GroupingDecisions = project?.GroupingDecisions ?? [],
            };

            await RunExportAsync(options, serviceProvider);
        });

        return command;
    }

    private static Command MigrateProjectCommand(IServiceProvider serviceProvider)
    {
        var projectOption = new Option<string>("--project")
        {
            Description = "Legacy version-1 project path or catalog name"
        };
        var outputOption = new Option<string>("--output")
        {
            Description = "Destination portable .ofbproject path"
        };
        var command = new Command("project-migrate", "Migrate a legacy project explicitly to the portable project format")
        {
            projectOption,
            outputOption
        };
        command.SetAction(context =>
        {
            var projectPath = context.GetValue(projectOption);
            var outputPath = context.GetValue(outputOption);
            if (string.IsNullOrWhiteSpace(projectPath) || string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Both --project and --output are required.");

            var store = serviceProvider.GetRequiredService<PortableProjectStore>();
            var loaded = store.Open(projectPath);
            if (!loaded.RequiresMigrationSave)
                throw new InvalidOperationException($"Project '{loaded.ProjectPath}' is already using the portable schema.");
            var migratedPath = store.SaveMigration(loaded, outputPath);
            System.Console.WriteLine($"Portable project saved to '{migratedPath}'.");
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

    private static Command GedcomRoundTripCommand(IServiceProvider serviceProvider)
    {
        var inputOption = new Option<string>("--input") { Description = "Path to the source GEDCOM file" };
        var outputOption = new Option<string>("--output") { Description = "Path for the roundtripped GEDCOM file" };
        var versionOption = new Option<string?>("--version")
        {
            Description = $"Target GEDCOM version ({GedcomVersion.Gedcom551} or {GedcomVersion.Gedcom70}); defaults to the highest supported version"
        };
        var allowRecoveryOption = new Option<bool>("--allow-recovery")
        {
            Description = "Explicitly allow best-effort output after import recovery; skipped or repaired data may be lost"
        };
        var command = new Command("gedcom-roundtrip", "Import a GEDCOM file into the genealogy model and write it through the GEDCOM output driver")
        {
            inputOption,
            outputOption,
            versionOption,
            allowRecoveryOption
        };

        command.SetAction(async context =>
        {
            var inputPath = context.GetValue(inputOption);
            var outputPath = context.GetValue(outputOption);
            if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Both --input and --output are required.");

            var inputDriver = serviceProvider.GetRequiredService<IGenealogyInputDriver>();
            var outputDriver = serviceProvider.GetRequiredService<IGenealogyOutputDriver>();
            await using var source = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var genealogy = await inputDriver.ReadAsync(source);
            var allowRecovery = context.GetValue(allowRecoveryOption);
            if (genealogy.HasRecoveryIssues && !allowRecovery)
            {
                foreach (var diagnostic in genealogy.Diagnostics)
                    System.Console.Error.WriteLine($"{diagnostic.Severity} {diagnostic.Code}: {diagnostic.Message}");
                throw new InvalidDataException(
                    "GEDCOM import required recovery; accepted roundtrip output is blocked. Use --allow-recovery only to request potentially lossy best-effort output.");
            }

            await using var destination = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await outputDriver.WriteAsync(
                genealogy,
                destination,
                new GenealogyOutputOptions
                {
                    TargetVersion = context.GetValue(versionOption),
                    AllowRecovery = allowRecovery
                });
            foreach (var diagnostic in genealogy.Diagnostics)
                System.Console.Error.WriteLine($"{diagnostic.Severity} {diagnostic.Code}: {diagnostic.Message}");
            System.Console.WriteLine($"GEDCOM roundtrip written to '{outputPath}'.");
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

            var exportService = serviceProvider.GetRequiredService<Func<IFamilyDataSource, OFBExportService>>()(dataSource);
            await exportService.ExportAsync(options, progress: new ConsoleExportProgress());

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

    private sealed class ConsoleExportProgress : IProgress<OFBExportProgress>
    {
        public void Report(OFBExportProgress value)
        {
            if (value.IsWarning)
                System.Console.Error.WriteLine(value.Message);
            else
                System.Console.WriteLine(value.Message);
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
