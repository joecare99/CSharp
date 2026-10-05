using System.Collections.Generic;
using System.Linq;
using AhnWin52Backup.Cli.Abstractions;
using AhnWin52Backup.Cli.Commands;
using AhnWin52Backup.Cli.Credentials;

namespace AhnWin52Backup.Tests.Cli;

[TestClass]
public sealed class SetPasswordCommandTests
{
    [TestMethod]
    public void Run_WithMatchingPasswords_SavesCredentialAndClearsPasswordBuffers()
    {
        char[] password = "secret-value".ToCharArray();
        char[] confirmation = "secret-value".ToCharArray();
        TestConsoleAdapter console = new(password, confirmation);
        TestCredentialStore credentialStore = new();
        SetPasswordCommand command = new(console, credentialStore);

        int exitCode = command.Run(["--set-passwd"]);

        Assert.AreEqual(0, exitCode);
        CollectionAssert.AreEqual("secret-value".ToCharArray(), credentialStore.SavedPassword);
        CollectionAssert.AreEqual(new char[password.Length], password);
        CollectionAssert.AreEqual(new char[confirmation.Length], confirmation);
        Assert.AreEqual(1, console.Output.Count);
        Assert.IsFalse(console.Output[0].Contains("secret-value"));
    }

    [TestMethod]
    public void Run_WithMismatchedPasswords_DoesNotSaveCredential()
    {
        char[] password = "first-value".ToCharArray();
        char[] confirmation = "different-value".ToCharArray();
        TestConsoleAdapter console = new(password, confirmation);
        TestCredentialStore credentialStore = new();
        SetPasswordCommand command = new(console, credentialStore);

        int exitCode = command.Run(["--set-passwd"]);

        Assert.AreEqual(2, exitCode);
        Assert.AreEqual(0, credentialStore.SaveCount);
        CollectionAssert.AreEqual(new char[password.Length], password);
        CollectionAssert.AreEqual(new char[confirmation.Length], confirmation);
    }

    [TestMethod]
    public void Run_WithPasswordArgument_RejectsItWithoutPrompting()
    {
        TestConsoleAdapter console = new();
        TestCredentialStore credentialStore = new();
        SetPasswordCommand command = new(console, credentialStore);

        int exitCode = command.Run(["--set-passwd", "must-not-be-accepted"]);

        Assert.AreEqual(2, exitCode);
        Assert.AreEqual(0, console.PasswordReadCount);
        Assert.AreEqual(0, credentialStore.SaveCount);
    }

    private sealed class TestConsoleAdapter(params char[][] passwords) : IConsoleAdapter
    {
        private int _nextPasswordIndex;

        public List<string> Output { get; } = [];

        public int PasswordReadCount { get; private set; }

        public void WriteLine(string value) => Output.Add(value);

        public void WriteErrorLine(string value) => Output.Add(value);

        public char[] ReadPassword(string prompt)
        {
            PasswordReadCount++;
            return passwords[_nextPasswordIndex++];
        }
    }

    private sealed class TestCredentialStore : IPasswordCredentialStore
    {
        public int SaveCount { get; private set; }

        public char[]? SavedPassword { get; private set; }

        public void SetPassword(char[] password)
        {
            SaveCount++;
            SavedPassword = password.ToArray();
        }
    }
}
