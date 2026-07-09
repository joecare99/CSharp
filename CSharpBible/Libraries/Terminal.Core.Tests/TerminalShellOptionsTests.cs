using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Terminal.Core;

namespace Terminal.Core.Tests;

[TestClass]
public class TerminalShellOptionsTests
{
    [TestMethod]
    public void CreateDefault_WithoutWorkingDirectory_UsesCurrentDirectory()
    {
        var options = TerminalShellOptions.CreateDefault();

        Assert.AreEqual(Environment.CurrentDirectory, options.WorkingDirectory);
        Assert.AreEqual(new TerminalSize(80, 25), options.InitialSize);
    }

    [TestMethod]
    public void CreateDefault_WithWorkingDirectory_UsesFullPath()
    {
        var relativeDirectory = Path.Combine(".", "Terminal.Core.Tests");

        var options = TerminalShellOptions.CreateDefault(relativeDirectory);

        Assert.AreEqual(Path.GetFullPath(relativeDirectory), options.WorkingDirectory);
    }

    [TestMethod]
    public void CreateDefault_ShouldUsePlatformSpecificShell()
    {
        var options = TerminalShellOptions.CreateDefault();

        if (OperatingSystem.IsWindows())
        {
            Assert.AreEqual(Environment.GetEnvironmentVariable("COMSPEC") ?? "cmd.exe", options.FileName);
            Assert.AreEqual(string.Empty, options.Arguments);
            return;
        }

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            Assert.AreEqual(PosixTerminalEnvironment.ResolveShell(Environment.GetEnvironmentVariable("SHELL")), options.FileName);
            Assert.AreEqual("-i", options.Arguments);
            return;
        }

        Assert.Inconclusive("The current platform is not supported by TerminalShellOptions.");
    }
}
