using FBParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class EnumTests
{
    [TestMethod]
    public void ParseMessageKind_HasExpectedOrder()
    {
        Assert.AreEqual(0, (int)ParseMessageKind.Error);
        Assert.AreEqual(1, (int)ParseMessageKind.Warning);
        Assert.AreEqual(2, (int)ParseMessageKind.Debug);
    }

    [TestMethod]
    public void ParserEventType_HasExpectedKeyValues()
    {
        Assert.AreEqual(0, (int)ParserEventType.evt_ID);
        Assert.AreEqual(1, (int)ParserEventType.evt_Birth);
        Assert.AreEqual(3, (int)ParserEventType.evt_Marriage);
        Assert.AreEqual(20, (int)ParserEventType.evt_Anull);
        Assert.AreEqual(43, (int)ParserEventType.evt_Stillborn);
        Assert.AreEqual(50, (int)ParserEventType.evt_Last);
    }
}
