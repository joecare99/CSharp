using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class SchemaFilterTests
{
    [TestMethod]
    public void Test_EmitsPlusLineAndAdvances()
    {
        var filter = Create("+Auftragsnummer:");

        var result = filter.Test("ignored");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Auftragsnummer:", result[0].Text);
        Assert.AreEqual(1, filter.TestLine);
    }

    [TestMethod]
    public void Test_MatchingBracketLineEnablesModeAndEmitsFollowingPlus()
    {
        var filter = Create("[TS: a", "+NextFile:");

        var result = filter.Test("TS: a href");

        Assert.IsTrue(filter.FilterMode);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("NextFile:", result[0].Text);
    }

    [TestMethod]
    public void Test_UppercaseJumpEnablesModeAndEmitsDestinationOutput()
    {
        var filter = Create("J03Next", "+Ignored", "+Destination");

        var result = filter.Test("Next item");

        Assert.IsTrue(filter.FilterMode);
        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(3, filter.TestLine);
    }

    [TestMethod]
    public void Test_ResetReturnsToFirstSchemaLine()
    {
        var filter = Create("+First");
        filter.Test("ignored");

        filter.Reset();

        Assert.AreEqual(0, filter.TestLine);
        Assert.IsFalse(filter.FilterMode);
    }

    private static ISchemaFilter Create(params string[] schema)
    {
        var filter = new SchemaFilter();
        filter.SetSchema(new List<string>(schema));
        return filter;
    }
}
