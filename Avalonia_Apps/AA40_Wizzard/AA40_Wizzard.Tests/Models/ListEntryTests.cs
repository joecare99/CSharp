using AA40_Wizzard.Model;

namespace AA40_Wizzard.Tests.Models;

[TestClass]
public class ListEntryTests
{
    [TestMethod]
    public void ConstructorStoresValuesTest()
    {
        var entry = new ListEntry(7, "Seven");

        Assert.AreEqual(7, entry.ID);
        Assert.AreEqual("Seven", entry.Text);
        Assert.AreEqual("Seven", entry.ToString());
    }
}
