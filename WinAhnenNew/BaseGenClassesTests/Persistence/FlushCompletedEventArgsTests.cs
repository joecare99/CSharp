using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using NSubstitute;

namespace BaseGenClasses.Persistence.Tests;

[TestClass]
public class FlushCompletedEventArgsTests
{
    [TestMethod]
    public void Constructor_StoresRequestedEntityAndScope()
    {
        IGenEntity requestedEntity = Substitute.For<IGenEntity>();

        var eventArgs = new FlushCompletedEventArgs(requestedEntity, GenealogyPersistenceScope.Auto);

        Assert.AreSame(requestedEntity, eventArgs.GenRequestedEntity);
        Assert.AreEqual(GenealogyPersistenceScope.Auto, eventArgs.eScope);
    }

    [TestMethod]
    public void Constructor_AllowsNullEntity()
    {
        var eventArgs = new FlushCompletedEventArgs(null, GenealogyPersistenceScope.Auto);

        Assert.IsNull(eventArgs.GenRequestedEntity);
        Assert.AreEqual(GenealogyPersistenceScope.Auto, eventArgs.eScope);
    }
}
