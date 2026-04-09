using System;
using System.Threading;
using System.Threading.Tasks;
using BaseGenClasses.Model;
using BaseGenClasses.Persistence;
using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;

namespace BaseGenClassesTests
{
    [TestClass]
    public class GenealogyPersistenceContextTests
    {
        [TestMethod]
        public async Task FlushAsync_WhenGenealogyIsDirty_DelegatesToPersistenceProviderAndClearsDirty()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger)
            {
                UId = Guid.NewGuid()
            };
            var prvPersistence = Substitute.For<IGenealogyPersistenceProvider>();

            genGenealogy.AttachPersistenceProvider(prvPersistence);
            genGenealogy.MarkDirty(null, "Test change");
            Assert.IsTrue(genGenealogy.xDirty);
 
            await genGenealogy.FlushAsync();
 
            Assert.IsFalse(genGenealogy.xDirty);
            await prvPersistence.Received(1).FlushAsync(genGenealogy, null, GenealogyFlushScope.Auto, Arg.Any<CancellationToken>());
        }
    }
}
