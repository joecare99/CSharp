namespace AhnWin52Backup.Cli.Abstractions;

internal interface IConsoleAdapter
{
    void WriteLine(string value);

    void WriteErrorLine(string value);

    char[] ReadPassword(string prompt);
}
