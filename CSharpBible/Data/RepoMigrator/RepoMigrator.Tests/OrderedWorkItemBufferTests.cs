using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class OrderedWorkItemBufferTests
{
    [TestMethod]
    public void TryTakeNext_ReturnsBufferedItemsInOrder()
    {
        var orderedBuffer = new OrderedWorkItemBuffer<string>();
        orderedBuffer.Add(2, "two");
        orderedBuffer.Add(0, "zero");
        orderedBuffer.Add(1, "one");

        Assert.IsTrue(orderedBuffer.TryTakeNext(0, out var sFirstItem));
        Assert.IsTrue(orderedBuffer.TryTakeNext(1, out var sSecondItem));
        Assert.IsTrue(orderedBuffer.TryTakeNext(2, out var sThirdItem));

        Assert.AreEqual("zero", sFirstItem);
        Assert.AreEqual("one", sSecondItem);
        Assert.AreEqual("two", sThirdItem);
    }

    [TestMethod]
    public void Add_ThrowsWhenIndexAlreadyExists()
    {
        var orderedBuffer = new OrderedWorkItemBuffer<string>();
        orderedBuffer.Add(1, "one");

        try
        {
            orderedBuffer.Add(1, "duplicate");
            Assert.Fail("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException ex)
        {
            StringAssert.Contains(ex.Message, "index 1");
        }
    }
}
