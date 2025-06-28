using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GenFree.Models.VB.Tests;

[TestClass]
public class CStringsTests
{
    private CStrings _strings;

    [TestInitialize]
    public void Setup()
    {
        _strings = new CStrings();
    }

    [TestMethod]
    [DataRow('A', (short)65)]
    [DataRow('z', (short)122)]
    public void Asc_Char_ReturnsAsciiValue(char input, short expected)
    {
        Assert.AreEqual(expected, _strings.Asc(input));
    }

    [TestMethod]
    [DataRow("A", (short)65)]
    [DataRow("z", (short)122)]
    [DataRow("", (short)0)]
    public void Asc_String_ReturnsAsciiValue(string input, short expected)
    {
        Assert.AreEqual(expected, _strings.Asc(input));
    }

    [TestMethod]
    [DataRow((short)65, 'A')]
    [DataRow((short)122, 'z')]
    public void Chr_ReturnsChar(short input, char expected)
    {
        Assert.AreEqual(expected, _strings.Chr(input));
    }

    [TestMethod]
    [DataRow(123.456, "{0:F2}", "123,46")]
    [DataRow(42, "Wert: {0}", "Wert: 42")]
    public void Format_FormatsValue(object value, string format, string expected)
    {
        Assert.AreEqual(expected, _strings.Format(value, format));
    }

    [TestMethod]
    [DataRow("abcdef", "cd", 3)]
    [DataRow("abcdef", "x", 0)]
    [DataRow("abcdef", "a", 1)]
    public void InStr_ReturnsOneBasedIndex(string text, string v, int expected)
    {
        Assert.AreEqual(expected, _strings.InStr(text, v));
    }

    [TestMethod]
    [DataRow("abcdef", 3, "abc")]
    [DataRow("ab", 5, "ab")]
    [DataRow("", 2, "")]
    [DataRow("abcdef", 0, "")]
    public void Left_ReturnsLeftmostChars(string v, int c, string expected)
    {
        Assert.AreEqual(expected, _strings.Left(v, c));
    }

    [TestMethod]
    [DataRow("abcdef", 6)]
    [DataRow("", 0)]
    public void Len_ReturnsLength(string input, int expected)
    {
        Assert.AreEqual(expected, _strings.Len(input));
    }

    [TestMethod]
    [DataRow("   abc", "abc")]
    [DataRow("abc", "abc")]
    [DataRow("   ", "")]
    public void LTrim_RemovesLeadingWhitespace(string input, string expected)
    {
        Assert.AreEqual(expected, _strings.LTrim(input));
    }

    [TestMethod]
    [DataRow("abcdef", 2, 3, "bcd")]
    [DataRow("abcdef", 2, -1, "bcdef")]
    [DataRow("abcdef", 7, 2, "")]
    [DataRow("", 1, 2, "")]
    [DataRow("abcdef", 0, 2, "")]
    public void Mid_ReturnsSubstring(string text, int start, int length, string expected)
    {
        Assert.AreEqual(expected, _strings.Mid(text, start, length));
    }

    [TestMethod]
    public void MidStmtStr_ReplacesSubstring()
    {
        string s = "abcdef";
        _strings.MidStmtStr(ref s, 2, 3, "XYZ");
        Assert.AreEqual("aXYZef", s);

        s = "abcdef";
        _strings.MidStmtStr(ref s, 7, 2, "Q");
        Assert.AreEqual("abcdef", s);

        s = "abcdef";
        _strings.MidStmtStr(ref s, 2, -1, "Q");
        Assert.AreEqual("abcdef", s);

        s = "";
        _strings.MidStmtStr(ref s, 1, 2, "Q");
        Assert.AreEqual("", s);
    }

    [TestMethod]
    [DataRow("abcabc", "a", "x", "xbcxbc")]
    [DataRow("abc", "d", "x", "abc")]
    public void Replace_ReplacesAllOccurrences(string v1, string v2, string v3, string expected)
    {
        Assert.AreEqual(expected, _strings.Replace(v1, v2, v3));
    }

    [TestMethod]
    [DataRow("abcdef", 3, "def")]
    [DataRow("ab", 5, "ab")]
    [DataRow("", 2, "")]
    [DataRow("abcdef", 0, "")]
    public void Right_ReturnsRightmostChars(string v, int c, string expected)
    {
        Assert.AreEqual(expected, _strings.Right(v, c));
    }

    [TestMethod]
    [DataRow("abc   ", "abc")]
    [DataRow("abc", "abc")]
    [DataRow("   ", "")]
    public void RTrim_RemovesTrailingWhitespace(string input, string expected)
    {
        Assert.AreEqual(expected, _strings.RTrim(input));
    }

    [TestMethod]
    [DataRow(3, "   ")]
    [DataRow(0, "")]
    public void Space_ReturnsSpaces(int v, string expected)
    {
        Assert.AreEqual(expected, _strings.Space(v));
    }

    [TestMethod]
    public void Split_SplitsString()
    {
        var result = _strings.Split("a,b,c", ",");
        CollectionAssert.AreEqual(new List<string> { "a", "b", "c" }, (List<string>)result);

        result = _strings.Split("", ",");
        CollectionAssert.AreEqual(new List<string>(), (List<string>)result);
    }

    [TestMethod]
    [DataRow("  abc  ", "abc")]
    [DataRow("abc", "abc")]
    [DataRow("   ", "")]
    public void Trim_RemovesWhitespace(string input, string expected)
    {
        Assert.AreEqual(expected, _strings.Trim(input));
    }

    [TestMethod]
    [DataRow("abc", "ABC")]
    [DataRow("AbC", "ABC")]
    public void UCase_ConvertsToUpper(string input, string expected)
    {
        Assert.AreEqual(expected, _strings.UCase(input));
    }
}
