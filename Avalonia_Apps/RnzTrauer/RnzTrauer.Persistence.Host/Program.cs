using System;
using System.Text.Json;
using RnzTrauer.Core.Domain;
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
    }
}
