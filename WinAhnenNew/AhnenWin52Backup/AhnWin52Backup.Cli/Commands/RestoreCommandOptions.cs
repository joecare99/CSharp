using System;
using System.IO;

namespace AhnWin52Backup.Cli.Commands;

internal enum RestoreMode
{
    ExistingTemplate,
    FromScratch,
    Override
}

internal sealed record RestoreCommandOptions(
    string InputPath,
    string DestinationPath,
    RestoreMode Mode,
    bool Force)
{
    public static bool TryParse(string[] arguments, out RestoreCommandOptions? options, out string error)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        options = null;
        error = string.Empty;
        if (arguments.Length < 3 || !string.Equals(arguments[0], "restore", StringComparison.OrdinalIgnoreCase))
        {
            error = "Usage: restore [--fromscratch|--override] [--force] <input.hej> <database-directory>";
            return false;
        }

        RestoreMode mode = RestoreMode.ExistingTemplate;
        bool modeSpecified = false;
        bool force = false;
        string[] positional = new string[2];
        int positionalCount = 0;
        for (int index = 1; index < arguments.Length; index++)
        {
            string argument = arguments[index];
            if (string.Equals(argument, "--fromscratch", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(argument, "--override", StringComparison.OrdinalIgnoreCase))
            {
                if (modeSpecified)
                {
                    error = "Choose only one of --fromscratch and --override.";
                    return false;
                }

                modeSpecified = true;
                mode = string.Equals(argument, "--fromscratch", StringComparison.OrdinalIgnoreCase)
                    ? RestoreMode.FromScratch
                    : RestoreMode.Override;
                continue;
            }

            if (string.Equals(argument, "--force", StringComparison.OrdinalIgnoreCase))
            {
                if (force)
                {
                    error = "--force may be specified only once.";
                    return false;
                }

                force = true;
                continue;
            }

            if (argument.StartsWith("--", StringComparison.Ordinal))
            {
                error = $"Unknown restore option: {argument}";
                return false;
            }

            if (positionalCount == positional.Length)
            {
                error = "Restore accepts exactly an input .hej path and a destination directory.";
                return false;
            }

            positional[positionalCount++] = argument;
        }

        if (positionalCount != positional.Length)
        {
            error = "Restore requires an input .hej path and a destination directory.";
            return false;
        }

        if (mode == RestoreMode.FromScratch && Directory.Exists(positional[1]))
        {
            error = $"The from-scratch destination already exists: {positional[1]}";
            return false;
        }

        if (!string.Equals(Path.GetExtension(positional[0]), ".hej", StringComparison.OrdinalIgnoreCase))
        {
            error = "The restore input must use the .hej file extension.";
            return false;
        }

        options = new RestoreCommandOptions(
            Path.GetFullPath(positional[0]),
            Path.GetFullPath(positional[1]),
            mode,
            force);
        return true;
    }
}
