using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terminal.Core;

namespace Terminal.Core.Tests;

[TestClass]
public class PosixTerminalEnvironmentTests
{
    [TestMethod]
    public void ResolveShell_ShouldPreferBashOverFish()
    {
        static bool FileExists(string path)
        {
            return path switch
            {
                "/bin/fish" => true,
                "/bin/bash" => true,
                _ => false
            };
        }

        var shell = PosixTerminalEnvironment.ResolveShell("/bin/fish", FileExists);

        Assert.AreEqual("/bin/bash", shell);
    }

    [TestMethod]
    public void ResolveShell_ShouldFallbackToShWhenPreferredShellIsMissing()
    {
        static bool FileExists(string path)
        {
            return path == "/bin/sh";
        }

        var shell = PosixTerminalEnvironment.ResolveShell("/missing/shell", FileExists);

        Assert.AreEqual("/bin/sh", shell);
    }

    [TestMethod]
    public void ResolveTerm_ShouldFallbackToVt100WhenXtermEntriesAreUnavailable()
    {
        static bool DirectoryExists(string path)
        {
            return path == "/usr/share/terminfo";
        }

        static bool FileExists(string path)
        {
            return path == "/usr/share/terminfo/v/vt100";
        }

        var term = PosixTerminalEnvironment.ResolveTerm(null, DirectoryExists, FileExists);

        Assert.AreEqual("vt100", term);
    }

    [TestMethod]
    public void ResolveTerm_ShouldFallbackToDumbWhenNoTerminfoEntryExists()
    {
        static bool DirectoryExists(string _)
        {
            return false;
        }

        static bool FileExists(string _)
        {
            return false;
        }

        var term = PosixTerminalEnvironment.ResolveTerm("xterm-256color", DirectoryExists, FileExists);

        Assert.AreEqual("dumb", term);
    }
}