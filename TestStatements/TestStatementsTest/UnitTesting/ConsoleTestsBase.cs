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

    protected static void AssertConsoleOutput(string Expected, Action ToTest)
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

        var result = sw.ToString().Trim();
        AssertAreEqual(Expected, result);
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

        var result = sw.ToString().Trim();
        AssertAreEqual(Expected, result);
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

        var result = sw.ToString().Trim();
        AssertAreEqual(Expected, result);
    }
}
