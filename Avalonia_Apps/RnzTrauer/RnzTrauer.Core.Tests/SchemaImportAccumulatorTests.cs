using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class SchemaImportAccumulatorTests
{
    [TestMethod]
    public void Process_AModeStoresAnchorPathAndStandardText()
    {
        var accumulator = new SchemaImportAccumulator();

        accumulator.Process(0, "A");
        accumulator.Process(2, "<a href=\"/rnz/notice.pdf?x=1\">");
        accumulator.Process(3, "A-100");

        Assert.AreEqual(0, accumulator.CompletedRows.Count);
        Assert.IsNotNull(accumulator.CurrentRow);
        Assert.AreEqual("/rnz/notice.pdf", accumulator.CurrentRow[2]);
        Assert.AreEqual("A-100", accumulator.CurrentRow[1]);
    }

    [TestMethod]
    public void Process_DModeAdvancesColumnForTableCells()
    {
        var accumulator = new SchemaImportAccumulator();

        accumulator.Process(0, "D");
        accumulator.Process(2, "<td class=\"a\">");
        accumulator.Process(3, "Surname");

        Assert.IsNotNull(accumulator.CurrentRow);
        Assert.AreEqual("Surname", accumulator.CurrentRow[3]);
    }

    [TestMethod]
    public void Process_NModeRaisesFilenameBeforeQueryString()
    {
        var accumulator = new SchemaImportAccumulator();

        accumulator.Process(0, "N");
        accumulator.Process(2, "<img src=\"/rnz/next.pdf?download=1\">");

        Assert.AreEqual(1, accumulator.NewFiles.Count);
        Assert.AreEqual("/rnz/next.pdf", accumulator.NewFiles[0]);
    }
}
