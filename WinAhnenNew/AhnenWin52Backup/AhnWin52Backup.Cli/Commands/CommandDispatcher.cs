using System;
using System.IO;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Cli.Abstractions;
using AhnWin52Backup.Cli.Credentials;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Cli.Commands;

internal sealed class CommandDispatcher
{
    private readonly IHejInspectionService _inspectionService;
    private readonly IHejDatabaseExportService _databaseExportService;
    private readonly IHejWriter _hejWriter;
    private readonly IParadoxTableReader _paradoxTableReader;
    private readonly IConsoleAdapter _console;
    private readonly SetPasswordCommand _setPasswordCommand;

    public CommandDispatcher(
        IHejInspectionService inspectionService,
        IHejDatabaseExportService databaseExportService,
        IHejWriter hejWriter,
        IParadoxTableReader paradoxTableReader,
        IConsoleAdapter console,
        IPasswordCredentialStore credentialStore)
    {
        _inspectionService = inspectionService;
        _databaseExportService = databaseExportService;
        _hejWriter = hejWriter;
        _paradoxTableReader = paradoxTableReader;
        _console = console;
        _setPasswordCommand = new SetPasswordCommand(console, credentialStore);
    }

    public int Run(string[] arguments)
    {
        if (arguments.Length == 0 || IsHelp(arguments[0]))
        {
            WriteUsage();
            return 0;
        }

        if (string.Equals(arguments[0], "inspect-db", StringComparison.OrdinalIgnoreCase))
        {
            return InspectDatabase(arguments);
        }

        if (string.Equals(arguments[0], "inventory-db", StringComparison.OrdinalIgnoreCase))
        {
            return InventoryDatabase(arguments);
        }

        if (string.Equals(arguments[0], "--set-passwd", StringComparison.OrdinalIgnoreCase))
        {
            return _setPasswordCommand.Run(arguments);
        }

        if (string.Equals(arguments[0], "backup", StringComparison.OrdinalIgnoreCase))
        {
            return BackupDatabase(arguments);
        }

        if (!string.Equals(arguments[0], "inspect", StringComparison.OrdinalIgnoreCase))
        {
            _console.WriteErrorLine($"Unknown command: {arguments[0]}");
            WriteUsage();
            return 2;
        }

        if (arguments.Length != 2)
        {
            _console.WriteErrorLine("The inspect command requires exactly one .hej file path.");
            WriteUsage();
            return 2;
        }

        string inputPath = Path.GetFullPath(arguments[1]);
        using FileStream source = new(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        HejInspection inspection = _inspectionService.Inspect(source);

        _console.WriteLine($"Valid HEJ backup: {inputPath}");
        foreach (HejSection section in Enum.GetValues<HejSection>())
        {
            _console.WriteLine($"{section}: {inspection.RecordCounts[section]} records");
        }

        foreach (string warning in inspection.Document.Warnings)
        {
            _console.WriteErrorLine($"Compatibility warning: {warning}");
        }

        return 0;
    }

    private int InspectDatabase(string[] arguments)
    {
        int requested = 0;
        bool showFirst = arguments.Length == 4 &&
            string.Equals(arguments[2], "--first", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(arguments[3], out requested) && requested is >= 1 and <= 200;
        if (arguments.Length != 2 && !showFirst)
        {
            _console.WriteErrorLine("Usage: inspect-db <file.db> [--first <1..200>]");
            WriteUsage();
            return 2;
        }

        string inputPath = Path.GetFullPath(arguments[1]);
        ParadoxTable table = _paradoxTableReader.Read(inputPath);
        _console.WriteLine($"Paradox table: {table.Name}");
        _console.WriteLine($"Active records: {table.Records.Count}");
        foreach (ParadoxField field in table.Fields)
        {
            _console.WriteLine($"{field.Name}: type 0x{field.TypeCode:X2}, {field.Length} bytes");
        }

        if (showFirst)
        {
            for (int row = 0; row < Math.Min(requested, table.Records.Count); row++)
            {
                _console.WriteLine($"Record {row + 1}:");
                for (int column = 0; column < table.Fields.Count; column++)
                {
                    _console.WriteLine($"  {table.Fields[column].Name}: {table.Records[row][column] ?? "<null>"}");
                }
            }
        }

        return 0;
    }

    private int InventoryDatabase(string[] arguments)
    {
        if (arguments.Length != 2)
        {
            _console.WriteErrorLine("The inventory-db command requires one database directory.");
            return 2;
        }

        var tables = new ParadoxDirectoryInventory().Read(Path.GetFullPath(arguments[1]));
        _console.WriteLine($"Paradox tables: {tables.Count}");
        foreach (var table in tables)
        {
            _console.WriteLine($"{table.FileName}: version 0x{table.Schema.Version:X2}, " +
                $"declared rows {table.Schema.DeclaredRecordCount}, blocks {table.Schema.DeclaredBlockCount}, " +
                $"encrypted {table.Schema.Encrypted}, PX {table.HasPrimaryIndex}, MB {table.HasMemoFile}, " +
                $"XG/YG pairs {table.SecondaryIndexes.Count}");
            foreach (ParadoxField field in table.Schema.Fields)
            {
                _console.WriteLine($"  {field.Name}: type 0x{field.TypeCode:X2}, {field.Length} bytes");
            }
        }

        return 0;
    }

    private int BackupDatabase(string[] arguments)
    {
        if (arguments.Length != 3)
        {
            _console.WriteErrorLine("The backup command requires a database directory and an output .hej path.");
            WriteUsage();
            return 2;
        }

        string databaseDirectory = Path.GetFullPath(arguments[1]);
        string outputPath = Path.GetFullPath(arguments[2]);
        if (!string.Equals(Path.GetExtension(outputPath), ".hej", StringComparison.OrdinalIgnoreCase))
        {
            _console.WriteErrorLine("The backup output must use the .hej file extension.");
            return 2;
        }

        string outputDirectory = Path.GetDirectoryName(outputPath)
            ?? throw new ArgumentException("The output path must include a directory.", nameof(arguments));
        if (!Directory.Exists(outputDirectory))
        {
            throw new DirectoryNotFoundException($"Output directory does not exist: {outputDirectory}");
        }

        if (File.Exists(outputPath))
        {
            throw new IOException($"Output file already exists; refusing to overwrite it: {outputPath}");
        }

        string temporaryPath = Path.Combine(
            outputDirectory,
            $".{Path.GetFileName(outputPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            HejDocument document = _databaseExportService.Export(databaseDirectory);
            using (FileStream destination = new(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.ReadWrite,
                FileShare.None))
            {
                _hejWriter.Write(document, destination);
                destination.Flush(flushToDisk: true);
                destination.Position = 0;
                _ = _inspectionService.Inspect(destination);
            }

            File.Move(temporaryPath, outputPath, overwrite: false);
            _console.WriteLine($"Created complete HEJ backup: {outputPath}");
            _console.WriteLine($"Individuals: {document.Individuals.Count} records");
            _console.WriteLine($"Marriages: {document.Marriages.Count} records");
            _console.WriteLine($"Adoptions: {document.Adoptions.Count} records");
            _console.WriteLine($"Places: {document.Places.Count} records");
            _console.WriteLine($"Sources: {document.Sources.Count} records");
            return 0;
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private void WriteUsage()
    {
        _console.WriteLine("AhnWin52Backup");
        _console.WriteLine("  inspect <file.hej>   Validate a HEJ backup and show section record counts.");
        _console.WriteLine("  inspect-db <file.db> [--first <1..200>] Read schema/count, optionally show records.");
        _console.WriteLine("  inventory-db <db-dir> List all Paradox table schemas and sidecar presence without reading records.");
        _console.WriteLine("  backup <db-dir> <file.hej> Export all five AhnWin tables without overwriting output.");
        _console.WriteLine("  --set-passwd        Securely save the Paradox password in Windows Credential Manager.");
    }

    private static bool IsHelp(string value) =>
        string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
}
