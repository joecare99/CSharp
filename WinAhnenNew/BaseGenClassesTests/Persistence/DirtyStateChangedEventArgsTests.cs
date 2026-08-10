using GenInterfaces.Interfaces.Genealogic;
using NSubstitute;

namespace BaseGenClasses.Persistence.Tests;

[TestClass]
public class DirtyStateChangedEventArgsTests
{
    [TestMethod]
    public void Constructor_StoresValuesAndLegacyAliases()
    {
        IGenEntity changedEntity = Substitute.For<IGenEntity>();

        var eventArgs = new DirtyStateChangedEventArgs(true, changedEntity, "entity updated");

        Assert.IsTrue(eventArgs.IsDirty);
        Assert.IsTrue(eventArgs.xIsDirty);
        Assert.AreSame(changedEntity, eventArgs.ChangedEntity);
        Assert.AreSame(changedEntity, eventArgs.GenChangedEntity);
        Assert.AreEqual("entity updated", eventArgs.Reason);
    }

    [TestMethod]
    public void Constructor_AllowsNullValues()
    {
        var eventArgs = new DirtyStateChangedEventArgs(false, null, null);

        Assert.IsFalse(eventArgs.IsDirty);
        Assert.IsFalse(eventArgs.xIsDirty);
        Assert.IsNull(eventArgs.ChangedEntity);
        Assert.IsNull(eventArgs.GenChangedEntity);
        Assert.IsNull(eventArgs.Reason);
    }
}
