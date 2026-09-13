using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Property.Editor.Tests;

[TestClass]
public sealed class PropertyItemOrderingTests
{
    [TestMethod]
    public void Order_WithEqualSortOrders_UsesCategoryAndPropertyNamesAsOrdinalTieBreakers()
    {
        var advanced = new PropertyCategory("Advanced", "Advanced");
        var general = new PropertyCategory("General", "General");
        var zeta = new PropertyItem(general, "Zeta", "Zeta", typeof(string), "z");
        var alpha = new PropertyItem(general, "Alpha", "Alpha", typeof(string), "a");
        var mode = new PropertyItem(advanced, "Mode", "Mode", typeof(string), "basic");

        var orderedNames = PropertyItemOrdering.Order([zeta, alpha, mode]).Select(static item => item.Name).ToArray();

        CollectionAssert.AreEqual(new[] { "Mode", "Alpha", "Zeta" }, orderedNames);
    }

    [TestMethod]
    public void Order_AcceptsInterfaceBasedConsumerItems()
    {
        var category = new PropertyCategory("General", "General");
        var item = Substitute.For<IPropertyItem>();
        item.Category.Returns(category);
        item.Name.Returns("Title");
        item.SortOrder.Returns(0);

        var ordered = PropertyItemOrdering.Order([item]);

        Assert.AreSame(item, ordered.Single());
    }
}
