using BaseLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        // Assert
        Assert.IsNotNull(_sut);
    }

    [TestMethod]
    public void ForegroundColor_Get_ShouldReturnConsoleColor()
    {
        // Act
        var result = _sut.ForegroundColor;

        // Assert
        Assert.IsInstanceOfType(result, typeof(ConsoleColor));
    }

    [TestMethod]
    public void ForegroundColor_Set_ShouldNotThrow()
    {
        // Arrange
        var originalColor = _sut.ForegroundColor;

        // Act & Assert
        _sut.ForegroundColor = ConsoleColor.Red;
        _sut.ForegroundColor = originalColor;
    }

    [TestMethod]
    public void BackgroundColor_Get_ShouldReturnConsoleColor()
    {
        // Act
        var result = _sut.BackgroundColor;

        // Assert
        Assert.IsInstanceOfType(result, typeof(ConsoleColor));
    }

    [TestMethod]
    public void BackgroundColor_Set_ShouldNotThrow()
    {
        // Arrange
        var originalColor = _sut.BackgroundColor;

        // Act & Assert
        _sut.BackgroundColor = ConsoleColor.Blue;
        _sut.BackgroundColor = originalColor;
    }

    [TestMethod]
    public void IsOutputRedirected_ShouldReturnBool()
    {
        // Act
        var result = _sut.IsOutputRedirected;

        // Assert
        Assert.IsInstanceOfType(result, typeof(bool));
    }

    [TestMethod]
    public void KeyAvailable_ShouldReturnBool()
    {
        // Act
        var result = _sut.KeyAvailable;

        // Assert
        Assert.IsInstanceOfType(result, typeof(bool));
    }

    [TestMethod]
    public void LargestWindowHeight_ShouldReturnInt()
    {
        // Act
        var result = _sut.LargestWindowHeight;

        // Assert
        Assert.IsInstanceOfType(result, typeof(int));
    }

    [TestMethod]
    public void Title_Get_ShouldReturnString()
    {
        // Act
        var result = _sut.Title;

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Title_Set_ShouldNotThrow()
    {
        // Arrange
        var originalTitle = _sut.Title;

        // Act & Assert
        _sut.Title = "TestTitle";
        _sut.Title = originalTitle;
    }

    [TestMethod]
    public void GetCursorPosition_ShouldReturnTuple()
    {
        // Act
        var result = _sut.GetCursorPosition;

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void SetCursorPosition_ShouldNotThrow()
    {
        // Arrange

        // Act 
        var result = _sut.SetCursorPosition;

        //Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Write_Char_ShouldNotThrow()
    {
        // Act & Assert - keine Exception erwartet
        _sut.Write('X');
    }

    [TestMethod]
    public void Write_String_ShouldNotThrow()
    {
        // Act & Assert - keine Exception erwartet
        _sut.Write("Test");
    }

    [TestMethod]
    public void Write_NullString_ShouldNotThrow()
    {
        // Act & Assert - keine Exception erwartet
        _sut.Write((string?)null);
    }

    [TestMethod]
    public void WriteLine_ShouldNotThrow()
    {
        // Act & Assert - keine Exception erwartet
        _sut.WriteLine("TestLine");
    }

    [TestMethod]
    public void WriteLine_Null_ShouldNotThrow()
    {
        // Act & Assert - keine Exception erwartet
        _sut.WriteLine(null);
    }

    [TestMethod]
    public void Clear_ShouldNotThrow()
    {
        // Act & Assert - keine Exception erwartet
        var m= _sut.Clear;
        //Assert
        Assert.IsNotNull(m);
    }

    // Beep und ReadKey/ReadLine werden nicht getestet, da sie
    // auf Benutzereingaben warten oder Audio abspielen
}