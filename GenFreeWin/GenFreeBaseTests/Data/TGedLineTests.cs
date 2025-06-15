using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenFree.Data.Models.Tests;

[TestClass]
public class TGedLineTests
{
    [DataTestMethod]
    [DataRow("0 HEAD", 0, null, "HEAD", null)]
    [DataRow("1 @I1@ NAME John /Doe/", 1, "@I1@", "NAME", "John /Doe/")]
    [DataRow("2 DATE 1 JAN 1900", 2, null, "DATE", "1 JAN 1900")]
    [DataRow("3 @F1@ FAMC", 3, "@F1@", "FAMC", null)]
    [DataRow("1 NAME", 1, null, "NAME", null)]
    [DataRow("2", 2, null, "", null)]
    [DataRow("X INVALID", -1, null, "", "X INVALID")]
    public void Constructor_String_ParsesCorrectly(string input, int expectedLvl, string expectedLink, string expectedTag, string expectedData)
    {
        var line = new TGedLine(input);
        Assert.AreEqual(expectedLvl, line.iLvl,"iLvl");
        Assert.AreEqual(expectedLink, line.link,"link");
        Assert.AreEqual(expectedTag, line.sTag,"sTag");
        Assert.AreEqual(expectedData, line.sData,"sData");
    }

    [TestMethod]
    public void SetTag_SetsTag()
    {
        var line = new TGedLine(1, "NAME", null, "John");
        line.SetTag("DATE");
        Assert.AreEqual("DATE", line.sTag);
    }

    [TestMethod]
    public void SetLvl_SetsLvl()
    {
        var line = new TGedLine(1, "NAME", null, "John");
        line.SetLvl(2);
        Assert.AreEqual(2, line.iLvl);
    }

    [TestMethod]
    public void SetData_SetsData()
    {
        var line = new TGedLine(1, "NAME", null, "John");
        line.SetData("Jane");
        Assert.AreEqual("Jane", line.sData);
    }

    [TestMethod]
    public void tLvlTag_ReturnsTuple()
    {
        var line = new TGedLine(1, "NAME", null, "John");
        var tuple = line.tLvlTag;
        Assert.AreEqual((1, "NAME"), tuple);
    }

    [TestMethod]
    public void ImplicitTupleConversion_WorksBothWays()
    {
        TGedLine line = (2, "@X@", "TAG", "DATA");
        (int iLvl, string? link, string sTag, string? sData) tuple = line;
        Assert.AreEqual(2, tuple.iLvl);
        Assert.AreEqual("@X@", tuple.link);
        Assert.AreEqual("TAG", tuple.sTag);
        Assert.AreEqual("DATA", tuple.sData);

        TGedLine line2 = tuple;
        Assert.AreEqual(line, line2);
    }

    [TestMethod]
    public void ToString_FormatsCorrectly()
    {
        var line = new TGedLine(1, "NAME", "@I1@", "John");
        Assert.AreEqual("1 @I1@ NAME John", line.ToString());
    }

    [TestMethod]
    public void ImplicitStringConversion_Works()
    {
        TGedLine line = new TGedLine(1, "NAME", null, "John");
        string s = line;
        Assert.AreEqual("1 NAME John", s);
    }

    [TestMethod]
    public void Trim_And_TrimEnd_Work()
    {
        var line = new TGedLine(1, "NAME", null, "John ");
        Assert.AreEqual("1 NAME John", line.Trim());
        Assert.AreEqual("1 NAME John", line.TrimEnd());
    }

    [TestMethod]
    public void Length_ReturnsCorrectLength()
    {
        var line = new TGedLine(1, "NAME", null, "John");
        Assert.AreEqual("1 NAME John".Length, line.Length);
    }
}
