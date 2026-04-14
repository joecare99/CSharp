using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BaseLib.Helper.TestHelper;

namespace TestStatements.UnitTesting;
/// <summary>
/// Class ConsoleTestsBase.
/// </summary>
public class ConsoleTestsBase
{
    private static string NormalizeConsoleText(string value)
        => (value ?? string.Empty).Replace("\r\n", "\n").Replace("\r", "\n").Trim();

    protected static string GetConsoleOutput(Action ToTest)
    {
        using var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            ToTest?.Invoke();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        return NormalizeConsoleText(sw.ToString());
    }

    protected static void AssertConsoleOutput(string Expected, Action ToTest)
    {
        var expected = NormalizeConsoleText(Expected);
        var result = GetConsoleOutput(ToTest);
        AssertAreEqual(expected, result);
    }

    protected static void AssertConsoleOutputArgs(string Expected, string[] Args, Action<String[]> ToTest)
    {
        using var sw = new StringWriter();
        var originalOut = Console.Out;

        Console.SetOut(sw);

        try
        {
            ToTest?.Invoke(Args);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var expected = NormalizeConsoleText(Expected);
        var result = NormalizeConsoleText(sw.ToString());
        AssertAreEqual(expected, result);
    }
    protected void AssertConsoleInOutputArgs(string Expected, string TestInput, string[] Args, Action<string[]> ToTest)
    {
        using var sw = new StringWriter();
        using var sr = new StringReader(TestInput);
        var originalOut = Console.Out;
        var originalIn = Console.In;
        Console.SetOut(sw);
        Console.SetIn(sr);

        try
        {
            ToTest?.Invoke(Args);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);
        }

        var expected = NormalizeConsoleText(Expected);
        var result = NormalizeConsoleText(sw.ToString());
        AssertAreEqual(expected, result);
    }

    protected static string GetConsoleInOutputArgs(string TestInput, string[] Args, Action<string[]> ToTest)
    {
        using var sw = new StringWriter();
        using var sr = new StringReader(TestInput);
        var originalOut = Console.Out;
        var originalIn = Console.In;
        Console.SetOut(sw);
        Console.SetIn(sr);

        try
        {
            ToTest?.Invoke(Args);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);
        }

        return NormalizeConsoleText(sw.ToString());
    }
}
