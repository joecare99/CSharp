using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
[DoNotParallelize]
public sealed class SystemConsoleAdapterTests
{
    [TestMethod]
    public void Adapter_DelegatesAllConsoleOperationsWhenWindowsCapabilitiesExist()
    {
        ISystemConsoleRuntime runtime = Substitute.For<ISystemConsoleRuntime>();
        IPlatformInfo platform = Substitute.For<IPlatformInfo>();
        platform.IsWindows.Returns(true);
        runtime.IsInputRedirected.Returns(false);
        runtime.KeyAvailable.Returns(true);
        runtime.ReadKey().Returns(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        runtime.GetCursorPosition().Returns((4, 5));
        runtime.ReadLine().Returns("line");
        runtime.Title.Returns("title");
        runtime.CursorVisible.Returns(true);
        runtime.LargestWindowHeight.Returns(30);
        runtime.LargestWindowWidth.Returns(100);
        runtime.WindowHeight.Returns(20);
        runtime.WindowWidth.Returns(80);
        runtime.WindowLeft.Returns(1);
        runtime.WindowTop.Returns(2);
        runtime.BufferWidth.Returns(120);
        runtime.BufferHeight.Returns(40);
        runtime.IsOutputRedirected.Returns(true);
        IConsole adapter = new SystemConsoleAdapter(runtime, platform);

        adapter.ForegroundColor = ConsoleColor.Cyan;
        adapter.BackgroundColor = ConsoleColor.Black;
        adapter.Title = "updated";
        adapter.WindowHeight = 21;
        adapter.WindowWidth = 81;
        adapter.WindowLeft = 3;
        adapter.WindowTop = 4;
        adapter.CursorVisible = false;
        adapter.Beep(440, 1);
        adapter.Clear();
        adapter.ResetColor();
        adapter.SetCursorPosition(6, 7);
        adapter.SetWindowPosition(8, 9);
        adapter.SetWindowSize(82, 22);
        adapter.Write('x');
        adapter.Write("text");
        adapter.WriteLine("line");

        Assert.AreEqual(ConsoleColor.Cyan, adapter.ForegroundColor);
        Assert.AreEqual(ConsoleColor.Black, adapter.BackgroundColor);
        Assert.AreEqual("updated", adapter.Title);
        Assert.IsFalse(adapter.CursorVisible);
        Assert.IsTrue(adapter.IsOutputRedirected);
        Assert.IsTrue(adapter.KeyAvailable);
        Assert.AreEqual(30, adapter.LargestWindowHeight);
        Assert.AreEqual(100, adapter.LargestWindowWidth);
        Assert.AreEqual(21, adapter.WindowHeight);
        Assert.AreEqual(81, adapter.WindowWidth);
        Assert.AreEqual(3, adapter.WindowLeft);
        Assert.AreEqual(4, adapter.WindowTop);
        Assert.AreEqual(120, adapter.BufferWidth);
        Assert.AreEqual(40, adapter.BufferHeight);
        Assert.AreEqual((4, 5), adapter.GetCursorPosition());
        Assert.IsNotNull(adapter.ReadKey());
        Assert.AreEqual("line", adapter.ReadLine());
        runtime.Received().Beep(440, 1);
        runtime.Received().SetWindowPosition(8, 9);
        runtime.Received().WriteLine("line");
    }

    [TestMethod]
    public void Adapter_SuppressesWindowsOnlyOperationsOnOtherPlatforms()
    {
        ISystemConsoleRuntime runtime = Substitute.For<ISystemConsoleRuntime>();
        IPlatformInfo platform = Substitute.For<IPlatformInfo>();
        platform.IsWindows.Returns(false);
        runtime.IsInputRedirected.Returns(true);
        runtime.ReadLine().Returns((string?)null);
        IConsole adapter = new SystemConsoleAdapter(runtime, platform);

        adapter.Title = "ignored";
        adapter.WindowLeft = 1;
        adapter.WindowTop = 2;
        adapter.CursorVisible = true;
        adapter.Beep(440, 1);

        Assert.AreEqual(string.Empty, adapter.Title);
        Assert.IsFalse(adapter.CursorVisible);
        Assert.IsFalse(adapter.KeyAvailable);
        Assert.IsNull(adapter.ReadKey());
        Assert.AreEqual(string.Empty, adapter.ReadLine());
        runtime.DidNotReceive().Beep(Arg.Any<int>(), Arg.Any<int>());
        runtime.DidNotReceive().SetWindowPosition(Arg.Any<int>(), Arg.Any<int>());
    }

    [TestMethod]
    public void PlatformInfo_ReportsTheCurrentOperatingSystem()
    {
        Assert.AreEqual(OperatingSystem.IsWindows(), new SystemPlatformInfo().IsWindows);
    }

    [TestMethod]
    public void Runtime_ExecutesPlatformDependentAndConsoleForwardingPaths()
    {
        IPlatformInfo windows = Substitute.For<IPlatformInfo>();
        windows.IsWindows.Returns(true);
        SystemConsoleRuntime runtime = new(
            windows,
            _ => { },
            _ => { },
            _ => { },
            (_, _) => { },
            () => new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false),
            () => "line");

        InvokeIgnoringConsoleRestriction(() => runtime.ForegroundColor = ConsoleColor.Cyan);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.ForegroundColor);
        InvokeIgnoringConsoleRestriction(() => runtime.BackgroundColor = ConsoleColor.Black);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.BackgroundColor);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.IsOutputRedirected);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.IsInputRedirected);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.KeyAvailable);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.LargestWindowHeight);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.LargestWindowWidth);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.Title);
        InvokeIgnoringConsoleRestriction(() => runtime.Title = "title");
        InvokeIgnoringConsoleRestriction(() => _ = runtime.WindowHeight);
        InvokeIgnoringConsoleRestriction(() => runtime.WindowHeight = 1);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.WindowWidth);
        InvokeIgnoringConsoleRestriction(() => runtime.WindowWidth = 1);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.WindowLeft);
        InvokeIgnoringConsoleRestriction(() => runtime.WindowLeft = 0);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.WindowTop);
        InvokeIgnoringConsoleRestriction(() => runtime.WindowTop = 0);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.CursorVisible);
        InvokeIgnoringConsoleRestriction(() => runtime.CursorVisible = true);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.BufferWidth);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.BufferHeight);
        InvokeIgnoringConsoleRestriction(() => runtime.Beep(440, 1));
        InvokeIgnoringConsoleRestriction(runtime.Clear);
        InvokeIgnoringConsoleRestriction(() => _ = runtime.GetCursorPosition());
        InvokeIgnoringConsoleRestriction(() => _ = runtime.ReadKey());
        InvokeIgnoringConsoleRestriction(() => _ = runtime.ReadLine());
        InvokeIgnoringConsoleRestriction(runtime.ResetColor);
        InvokeIgnoringConsoleRestriction(() => runtime.SetCursorPosition(0, 0));
        InvokeIgnoringConsoleRestriction(() => runtime.SetWindowPosition(0, 0));
        InvokeIgnoringConsoleRestriction(() => runtime.SetWindowSize(1, 1));
        InvokeIgnoringConsoleRestriction(() => runtime.Write('x'));
        InvokeIgnoringConsoleRestriction(() => runtime.Write("text"));
        InvokeIgnoringConsoleRestriction(() => runtime.WriteLine("line"));

        IPlatformInfo nonWindows = Substitute.For<IPlatformInfo>();
        nonWindows.IsWindows.Returns(false);
        SystemConsoleRuntime unsupportedRuntime = new(nonWindows);
        Assert.AreEqual(string.Empty, unsupportedRuntime.Title);
        unsupportedRuntime.Title = "ignored";
        unsupportedRuntime.WindowLeft = 0;
        unsupportedRuntime.WindowTop = 0;
        Assert.IsFalse(unsupportedRuntime.CursorVisible);
        unsupportedRuntime.CursorVisible = true;
        unsupportedRuntime.Beep(440, 1);
        unsupportedRuntime.SetWindowPosition(0, 0);
        Assert.IsNotNull(new SystemConsoleRuntime());
    }

    private static void InvokeIgnoringConsoleRestriction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception is IOException or InvalidOperationException or PlatformNotSupportedException)
        {
        }
    }
}
