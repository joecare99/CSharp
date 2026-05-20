using FBParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class PascalCompatTests
{
    [TestMethod]
    public void HasCharAt_ValidatesOneBasedBounds()
    {
        Assert.IsTrue(PascalCompat.HasCharAt("abc", 1));
        Assert.IsTrue(PascalCompat.HasCharAt("abc", 3));
        Assert.IsFalse(PascalCompat.HasCharAt("abc", 0));
        Assert.IsFalse(PascalCompat.HasCharAt("abc", 4));
    }

    [TestMethod]
    [DataRow("abcdef", 1, 3, "abc")]
    [DataRow("abcdef", 4, 10, "def")]
    [DataRow("abcdef", 7, 1, "")]
    [DataRow("abcdef", 1, 0, "")]
    public void Copy_ReturnsExpectedPascalCompatibleSubstring(string text, int startOneBased, int length, string expected)
    {
        Assert.AreEqual(expected, PascalCompat.Copy(text, startOneBased, length));
    }

    [TestMethod]
    [DataRow("abcdef", 3, "abc")]
    [DataRow("abcdef", 20, "abcdef")]
    [DataRow("abcdef", 0, "")]
    public void Left_ReturnsExpectedSegment(string text, int length, string expected)
    {
        Assert.AreEqual(expected, PascalCompat.Left(text, length));
    }

    [TestMethod]
    [DataRow("abcdef", 3, "def")]
    [DataRow("abcdef", 20, "abcdef")]
    [DataRow("abcdef", 0, "")]
    public void Right_ReturnsExpectedSegment(string text, int length, string expected)
    {
        Assert.AreEqual(expected, PascalCompat.Right(text, length));
    }

    [TestMethod]
    [DataRow("bc", "abcdef", 1, 2)]
    [DataRow("bc", "abcdef", 3, 0)]
    [DataRow("xx", "abcdef", 1, 0)]
    public void Pos_ReturnsOneBasedIndexOrZero(string value, string text, int startOneBased, int expected)
    {
        Assert.AreEqual(expected, PascalCompat.Pos(value, text, startOneBased));
    }

    [TestMethod]
    public void RemoveStart_RemovesCharactersFromBeginning()
    {
        Assert.AreEqual("def", PascalCompat.RemoveStart("abcdef", 3));
        Assert.AreEqual(string.Empty, PascalCompat.RemoveStart("abcdef", 10));
        Assert.AreEqual("abcdef", PascalCompat.RemoveStart("abcdef", 0));
        Assert.AreEqual(string.Empty, PascalCompat.RemoveStart(string.Empty, 3));
    }

    [TestMethod]
    public void LastIndexOfAny_ReturnsLastMatchingIndex()
    {
        var result = PascalCompat.LastIndexOfAny("ab12cd34", PascalCompat.Ziffern);
        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void IndexOfAny_WithCharacters_ReturnsFirstMatchingIndex()
    {
        var result = PascalCompat.IndexOfAny("abcd3ef", PascalCompat.Ziffern);
        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void IndexOfAny_WithStrings_ReturnsFirstMatchingSubstringIndex()
    {
        var result = PascalCompat.IndexOfAny("wohnhaft in Bern", new[] { "wohn", "Bern" });
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void CountChar_CountsOccurrences()
    {
        Assert.AreEqual(3, PascalCompat.CountChar("a,b,c,", ','));
    }

    [TestMethod]
    public void QuotedString_WrapsTextInSingleQuotes()
    {
        Assert.AreEqual("'abc'", PascalCompat.QuotedString("abc"));
    }

    [TestMethod]
    public void In_DetectsMembership()
    {
        Assert.IsTrue(PascalCompat.In('A', PascalCompat.UpperCharset));
        Assert.IsFalse(PascalCompat.In('a', PascalCompat.UpperCharset));
    }

    [TestMethod]
    public void CharsetDefinitions_ContainExpectedGermanCharacters()
    {
        Assert.IsTrue(PascalCompat.Charset.Contains('ä'));
        Assert.IsTrue(PascalCompat.UpperCharsetErw.Contains('Ä'));
        Assert.IsTrue(PascalCompat.LowerCharsetErw.Contains('ß'));
        Assert.IsTrue(PascalCompat.AlphaNum.Contains('9'));
        Assert.IsTrue(PascalCompat.AlphaNum.Contains('Ö'));
    }
}
