using System;
using System.Globalization;
using System.Text.Json;
using Db.Core.Abstractions.Sql.Interfaaces;
using Db.Provider.MySql;
using RnzTrauer.Core.Domain;
using RnzTrauer.Places;
using RnzTrauer.Persistence.MySql;

namespace RnzTrauer.Persistence.Host;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || HasOption(args, "--help"))
        {
            PrintHelp();
            return args.Length == 0 ? 2 : 0;
        }

        var place = GetOption(args, "--place");
        if (place is not null)
            return WriteCoordinateStatement(args, place);
        if (HasOption(args, "--coordinate-migration"))
            return WriteCoordinateMigration();
        if (HasOption(args, "--probe"))
            return ProbeCoordinateSchema(args);

        var kind = ParseKind(GetOption(args, "--queue"));
        var statement = MySqlNoticeSql.BuildFind(new NoticeFilter(Kind: kind));
        Console.WriteLine(JsonSerializer.Serialize(
            new
            {
                Queue = kind.ToString(),
                statement.CommandText,
                Parameters = statement.Parameters,
            },
            new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    private static int WriteCoordinateMigration()
    {
        var statement = MySqlPlaceCoordinateSql.BuildCoordinateMigration();
        Console.WriteLine(JsonSerializer.Serialize(
            new
            {
                Operation = "CoordinateSchemaMigration",
                Execution = "InspectionOnly",
                Warning = "Review and execute this statement through an approved migration process; the host does not execute it.",
                statement.CommandText,
                Parameters = statement.Parameters,
            },
            new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    private static int WriteCoordinateStatement(string[] args, string place)
    {
        var latitudeText = GetOption(args, "--latitude");
        var longitudeText = GetOption(args, "--longitude");
        var schemaStatus = ParseSchemaStatus(GetOption(args, "--coordinate-schema"));
        SqlStatement statement;
        string operation;

        if (latitudeText is null && longitudeText is null)
        {
            statement = MySqlPlaceCoordinateSql.BuildGet(place);
            operation = "Read";
        }
        else
        {
            if (latitudeText is null || longitudeText is null
                || !double.TryParse(
                    latitudeText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var latitude)
                || !double.TryParse(
                    longitudeText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var longitude))
            {
                Console.Error.WriteLine("--latitude and --longitude must be supplied as invariant numbers.");
                return 2;
            }

            statement = MySqlPlaceCoordinateSql.BuildSave(
                new PlaceCoordinate(place, latitude, longitude, "host-inspection", false));
            operation = "Write";
        }

        Console.WriteLine(JsonSerializer.Serialize(
            new
            {
                Operation = operation,
                Schema = CoordinateSchemaReport.Create(schemaStatus),
                statement.CommandText,
                Parameters = statement.Parameters,
            },
            new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    private static int ProbeCoordinateSchema(string[] args)
    {
        var factory = new MySqlDbConnectionFactory();
        var settings = factory.CreateSettingsStub();
        settings["Server"] = GetOption(args, "--server")
            ?? Environment.GetEnvironmentVariable("RNZ_DB_SERVER")
            ?? "localhost";
        var portText = GetOption(args, "--port")
            ?? Environment.GetEnvironmentVariable("RNZ_DB_PORT");
        if (portText is not null && !uint.TryParse(portText, out var port))
        {
            Console.Error.WriteLine("--port must be an unsigned integer.");
            return 2;
        }

        settings["Port"] = portText is null
            ? 3306u
            : uint.Parse(portText, CultureInfo.InvariantCulture);
        settings["UserID"] = GetOption(args, "--user")
            ?? Environment.GetEnvironmentVariable("RNZ_DB_USER")
            ?? "root";
        settings["Password"] = Environment.GetEnvironmentVariable("RNZ_DB_PASSWORD") ?? string.Empty;
        settings["Database"] = GetOption(args, "--database")
            ?? Environment.GetEnvironmentVariable("RNZ_DB_NAME")
            ?? "RNZ";

        var store = new MySqlPlaceCoordinateStore(factory, settings);
        var report = store.ProbeAsync().GetAwaiter().GetResult();
        Console.WriteLine(JsonSerializer.Serialize(
            new
            {
                Report = report,
                Connection = new
                {
                    Server = settings["Server"],
                    Port = settings["Port"],
                    Database = settings["Database"],
                },
            },
            new JsonSerializerOptions { WriteIndented = true }));
        return report.CanPersist ? 0 : 1;
    }

    private static CoordinateSchemaStatus ParseSchemaStatus(string? value)
    {
        if (value is null)
            return CoordinateSchemaStatus.Unverified;
        if (Enum.TryParse<CoordinateSchemaStatus>(value, true, out var status))
            return status;
        throw new ArgumentException(
            $"Unknown --coordinate-schema value '{value}'. Use available, missing, or unverified.",
            nameof(value));
    }

    private static NoticeFilterKind ParseKind(string? value)
    {
        return value is null
            ? NoticeFilterKind.All
            : Enum.Parse<NoticeFilterKind>(value, ignoreCase: true);
    }

    private static bool HasOption(string[] args, string name)
    {
        foreach (var argument in args)
            if (string.Equals(argument, name, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    private static string? GetOption(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
                return args[index + 1];
        return null;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("RNZ persistence characterization host");
        Console.WriteLine("Usage: RnzTrauer.Persistence.Host --queue <queue-name>");
        Console.WriteLine("Coordinate SQL: --place <name> [--latitude <n> --longitude <n>] [--coordinate-schema <available|missing|unverified>]");
        Console.WriteLine("Coordinate schema migration SQL: --coordinate-migration (inspection only; never executes SQL)");
        Console.WriteLine("Live schema probe: --probe [--server <host> --port <n> --user <name> --database <name>]");
        Console.WriteLine("Password: RNZ_DB_PASSWORD only; it is never printed.");
    }
}
