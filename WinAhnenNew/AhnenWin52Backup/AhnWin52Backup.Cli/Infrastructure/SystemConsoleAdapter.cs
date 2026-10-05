using System;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using AhnWin52Backup.Cli.Abstractions;

namespace AhnWin52Backup.Cli.Infrastructure;

internal sealed class SystemConsoleAdapter : IConsoleAdapter
{
    public void WriteLine(string value) => Console.WriteLine(value);

    public void WriteErrorLine(string value) => Console.Error.WriteLine(value);

    public char[] ReadPassword(string prompt)
    {
        if (Console.IsInputRedirected)
        {
            throw new IOException("Password entry requires an interactive console.");
        }

        Console.Write(prompt);
        char[] buffer = new char[256];
        int length = 0;
        try
        {
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    char[] password = buffer.AsSpan(0, length).ToArray();
                    CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(buffer.AsSpan()));
                    return password;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (length > 0)
                    {
                        buffer[--length] = '\0';
                    }

                    continue;
                }

                if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    throw new OperationCanceledException("Password entry was cancelled.");
                }

                if (!char.IsControl(key.KeyChar))
                {
                    if (length == buffer.Length)
                    {
                        throw new IOException("The password exceeds the supported maximum length.");
                    }

                    buffer[length++] = key.KeyChar;
                }
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(buffer.AsSpan()));
        }
    }
}
