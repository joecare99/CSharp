using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Property.Editor.Tests;

[TestClass]
public sealed class PropertyItemChangeTests
{
    [TestMethod]
    public void TrySetValue_WithValidChangedValue_RaisesOneChangeNotification()
    {
        var item = new PropertyItem(new PropertyCategory("General", "General"), "Title", "Title", typeof(string), "Initial");
        var notificationCount = 0;
        PropertyValueChangedEventArgs? receivedEvent = null;
        item.ValueChanged += (_, eventArgs) =>
        {
            notificationCount++;
            receivedEvent = eventArgs;
        };

        var updated = item.TrySetValue("Updated");

        Assert.IsTrue(updated);
        Assert.AreEqual(1, notificationCount);
        Assert.IsNotNull(receivedEvent);
        Assert.AreEqual("Initial", receivedEvent.OldValue);
        Assert.AreEqual("Updated", receivedEvent.NewValue);
    }

    [TestMethod]
    public void TrySetValue_WithUnchangedValue_DoesNotRaiseChangeNotification()
    {
        var item = new PropertyItem(new PropertyCategory("General", "General"), "Title", "Title", typeof(string), "Initial");
        var notificationCount = 0;
        item.ValueChanged += (_, _) => notificationCount++;

        var updated = item.TrySetValue("Initial");

        Assert.IsTrue(updated);
        Assert.AreEqual(0, notificationCount);
    }
}
