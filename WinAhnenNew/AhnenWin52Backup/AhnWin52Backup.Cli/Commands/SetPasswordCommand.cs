using System;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using AhnWin52Backup.Cli.Abstractions;
using AhnWin52Backup.Cli.Credentials;

namespace AhnWin52Backup.Cli.Commands;

internal sealed class SetPasswordCommand
{
    private readonly IConsoleAdapter _console;
    private readonly IPasswordCredentialStore _credentialStore;

    public SetPasswordCommand(IConsoleAdapter console, IPasswordCredentialStore credentialStore)
    {
        _console = console;
        _credentialStore = credentialStore;
    }

    public int Run(string[] arguments)
    {
        if (arguments.Length != 1)
        {
            _console.WriteErrorLine("The --set-passwd option does not accept a password argument.");
            return 2;
        }

        char[] password = _console.ReadPassword("Enter Paradox database password: ");
        char[] confirmation = [];
        try
        {
            if (password.Length == 0)
            {
                _console.WriteErrorLine("The password cannot be empty.");
                return 2;
            }

            confirmation = _console.ReadPassword("Confirm Paradox database password: ");
            if (!password.AsSpan().SequenceEqual(confirmation))
            {
                _console.WriteErrorLine("The passwords do not match; no credential was saved.");
                return 2;
            }

            _credentialStore.SetPassword(password);
            _console.WriteLine("Paradox database password saved in Windows Credential Manager for the current Windows user.");
            return 0;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(password.AsSpan()));
            CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(confirmation.AsSpan()));
        }
    }
}
