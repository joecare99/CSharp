using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BaseLib.Models.Tests;

[TestClass]
public class ConsoleProxyTests
{
    private ConsoleProxy _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _sut = new ConsoleProxy();
    }

    [TestMethod]
    public void Constructor_ShouldCreateInstance()
    {
        Assert.IsNotNull(_sut);
    }

    [TestMethod]
    public void ForegroundColor_Get_ShouldReturnConsoleColor()
    {
        var result = _sut.ForegroundColor;

        Assert.IsInstanceOfType<ConsoleColor>(result);
    }

    [TestMethod]
    public void ForegroundColor_Set_ShouldNotThrow()
    {
        var originalColor = _sut.ForegroundColor;

        AssertDoesNotThrow(() => _sut.ForegroundColor = ConsoleColor.Red);
        AssertDoesNotThrow(() => _sut.ForegroundColor = originalColor);
    }

    [TestMethod]
    public void BackgroundColor_Get_ShouldReturnConsoleColor()
    {
        var result = _sut.BackgroundColor;

        Assert.IsInstanceOfType<ConsoleColor>(result);
    }

    [TestMethod]
    public void BackgroundColor_Set_ShouldNotThrow()
    {
        var originalColor = _sut.BackgroundColor;

        AssertDoesNotThrow(() => _sut.BackgroundColor = ConsoleColor.Blue);
        AssertDoesNotThrow(() => _sut.BackgroundColor = originalColor);
    }

    [TestMethod]
    public void IsOutputRedirected_ShouldReturnBool()
    {
        var result = _sut.IsOutputRedirected;

        Assert.IsInstanceOfType<bool>(result);
    }

    [TestMethod]
    public void KeyAvailable_ShouldReturnBool()
    {
        var result = _sut.KeyAvailable;

        Assert.IsInstanceOfType<bool>(result);
    }

    [TestMethod]
    public void LargestWindowHeight_ShouldReturnInt()
    {
        var result = _sut.LargestWindowHeight;

        Assert.IsInstanceOfType<int>(result);
    }

    [TestMethod]
    public void LargestWindowWidth_ShouldReturnInt()
    {
        var result = _sut.LargestWindowWidth;

        Assert.IsInstanceOfType<int>(result);
    }

    [TestMethod]
    public void Title_Get_ShouldReturnString()
    {
        var result = _sut.Title;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Title_Set_ShouldNotThrow()
    {
        var originalTitle = _sut.Title;

        AssertDoesNotThrow(() => _sut.Title = "TestTitle");
        AssertDoesNotThrow(() => _sut.Title = originalTitle);
    }

    [TestMethod]
    public void WindowHeight_Set_ShouldNotThrow()
    {
        var originalValue = _sut.WindowHeight;

        AssertDoesNotThrow(() => _sut.WindowHeight = originalValue > 0 ? originalValue : 25);
    }

    [TestMethod]
    public void WindowWidth_Set_ShouldNotThrow()
    {
        var originalValue = _sut.WindowWidth;

        AssertDoesNotThrow(() => _sut.WindowWidth = originalValue > 0 ? originalValue : 80);
    }

    [TestMethod]
    public void WindowLeft_Set_ShouldNotThrow()
    {
        var originalValue = _sut.WindowLeft;

        AssertDoesNotThrow(() => _sut.WindowLeft = originalValue);
    }

    [TestMethod]
    public void WindowTop_Set_ShouldNotThrow()
    {
        var originalValue = _sut.WindowTop;

        AssertDoesNotThrow(() => _sut.WindowTop = originalValue);
    }

    [TestMethod]
    public void CursorVisible_Get_ShouldReturnBool()
    {
        var result = _sut.CursorVisible;

        Assert.IsInstanceOfType<bool>(result);
    }

    [TestMethod]
    public void CursorVisible_Set_ShouldNotThrow()
    {
        var originalValue = _sut.CursorVisible;

        AssertDoesNotThrow(() => _sut.CursorVisible = !originalValue);
        AssertDoesNotThrow(() => _sut.CursorVisible = originalValue);
    }

    [TestMethod]
    public void BufferWidth_ShouldReturnInt()
    {
        var result = _sut.BufferWidth;

        Assert.IsInstanceOfType<int>(result);
    }

    [TestMethod]
    public void BufferHeight_ShouldReturnInt()
    {
        var result = _sut.BufferHeight;

        Assert.IsInstanceOfType<int>(result);
    }

    [TestMethod]
    public void Beep_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.Beep(800, 10));
    }

    [TestMethod]
    public void Clear_ShouldNotThrow()
    {
        AssertDoesNotThrow(_sut.Clear);
    }

    [TestMethod]
    public void GetCursorPosition_ShouldReturnTuple()
    {
        var result = _sut.GetCursorPosition();

        Assert.IsInstanceOfType<ValueTuple<int, int>>(result);
    }

    [TestMethod]
    public void SetCursorPosition_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.SetCursorPosition(0, 0));
    }

    [TestMethod]
    public void SetWindowPosition_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.SetWindowPosition(_sut.WindowLeft, _sut.WindowTop));
    }

    [TestMethod]
    public void SetWindowSize_ShouldNotThrow()
    {
        var width = _sut.WindowWidth > 0 ? _sut.WindowWidth : 80;
        var height = _sut.WindowHeight > 0 ? _sut.WindowHeight : 25;

        AssertDoesNotThrow(() => _sut.SetWindowSize(width, height));
    }

    [TestMethod]
    public void ReadLine_ShouldReturnString()
    {
        var originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("TestInput" + Environment.NewLine));

            var result = _sut.ReadLine();

            Assert.AreEqual("TestInput", result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    [TestMethod]
    public void ResetColor_ShouldNotThrow()
    {
        AssertDoesNotThrow(_sut.ResetColor);
    }

    [TestMethod]
    public void Write_Char_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.Write('X'));
    }

    [TestMethod]
    public void Write_String_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.Write("Test"));
    }

    [TestMethod]
    public void Write_NullString_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.Write((string?)null));
    }

    [TestMethod]
    public void WriteLine_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.WriteLine("TestLine"));
    }

    [TestMethod]
    public void WriteLine_Null_ShouldNotThrow()
    {
        AssertDoesNotThrow(() => _sut.WriteLine(null));
    }

    private static void AssertDoesNotThrow(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected no exception, but got {ex.GetType().Name}: {ex.Message}");
        }
    }
}
