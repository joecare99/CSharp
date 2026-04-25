using GenFree.Views;
using GenFree.Interfaces.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using Gen_FreeWin;
using System.Linq;

namespace GenFree.Views.Tests;

[TestClass]
public class ApplUserTextsTests
{
    private ApplUserTexts _applUserTexts;

    [TestInitialize]
    public void Setup()
    {
        _applUserTexts = new ApplUserTexts();
    }

    [TestMethod]
    public void Constructor_ShouldInitializeWith560EmptyStrings()
    {
        Assert.AreEqual(560, _applUserTexts.Count);
        foreach (var text in _applUserTexts)
        {
            Assert.AreEqual(null, text);
        }
    }

    [TestMethod]
    [DataRow(0, "TestText")]
    [DataRow(559, "AnotherText")]
    public void Indexer_SetAndGetByInt_ShouldWorkCorrectly(int index, string value)
    {
        _applUserTexts[index] = value;
        Assert.AreEqual(value, _applUserTexts[index]);
    }

    [TestMethod]
    [DataRow(EUserText.tName, "NameText")]
    [DataRow(EUserText.tBirthdaySh, "BirthdayText")]
    public void Indexer_SetAndGetByEnum_ShouldWorkCorrectly(EUserText eUserText, string value)
    {
        _applUserTexts[eUserText] = value;
        Assert.AreEqual(value, _applUserTexts[eUserText]);
    }

    [TestMethod]
    public void Add_ShouldAddTextToList()
    {
        _applUserTexts.Add("NewText");
        Assert.AreEqual(561, _applUserTexts.Count);
        Assert.AreEqual("NewText", _applUserTexts[560]);
    }

    [TestMethod]
    public void Clear_ShouldRemoveAllTexts()
    {
        _applUserTexts.Clear();
        Assert.AreEqual(0, _applUserTexts.Count);
    }

    [TestMethod]
    public void Contains_ShouldReturnTrueIfTextExists()
    {
        _applUserTexts[0] = "ExistingText";
        Assert.IsTrue(_applUserTexts.Contains("ExistingText"));
    }

    [TestMethod]
    public void Contains_ShouldReturnFalseIfTextDoesNotExist()
    {
        Assert.IsFalse(_applUserTexts.Contains("NonExistingText"));
    }

    [TestMethod]
    public void CopyTo_ShouldCopyTextsToArray()
    {
        var array = new string[560];
        _applUserTexts.CopyTo(array, 0);
        CollectionAssert.AreEqual(_applUserTexts.ToArray(), array);
    }

    [TestMethod]
    public void GetEnumerator_ShouldEnumerateAllTexts()
    {
        var enumerator = _applUserTexts.GetEnumerator();
        var list = new List<string>();
        while (enumerator.MoveNext())
        {
            list.Add(enumerator.Current);
        }
        CollectionAssert.AreEqual(_applUserTexts.ToList(), list);
    }

    [TestMethod]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        _applUserTexts[10] = "IndexedText";
        Assert.AreEqual(10, _applUserTexts.IndexOf("IndexedText"));
    }

    [TestMethod]
    public void IsReadOnly_ShouldReturnFalse()
    {
        Assert.IsFalse(_applUserTexts.IsReadOnly);
    }
}
