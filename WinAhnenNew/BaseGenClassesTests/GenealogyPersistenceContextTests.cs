using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseGenClasses.Model;
using BaseGenClasses.Persistence;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
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

        [TestMethod]
        public void Receive_WhenTransactionIsRecorded_RaisesJournalEntryRecorded()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger)
            {
                UId = Guid.NewGuid()
            };
            var genTransaction = Substitute.For<IGenTransaction>();
            JournalEntryRecordedEventArgs? eRecordedArgs = null;

            genGenealogy.JournalEntryRecorded += (_, eArgs) => eRecordedArgs = eArgs;

            genGenealogy.Receive(genTransaction);

            Assert.IsNotNull(eRecordedArgs);
            Assert.AreSame(genTransaction, eRecordedArgs.GenTransaction);
            CollectionAssert.Contains(genGenealogy.JournalEntries.ToArray(), genTransaction);
        }

        [TestMethod]
        public void RecordSourceChange_StoresTypedSourceJournalSnapshot()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger)
            {
                UId = Guid.NewGuid()
            };
            var genSource = Substitute.For<IGenSource>();
            genSource.Description.Returns("Kirchenbuch");
            genSource.Data.Returns("Taufeintrag");
            genSource.Url.Returns(new Uri("https://example.org/source"));
            genSource.Medias.Returns(new System.Collections.Generic.List<IGenMedia>());

            genGenealogy.RecordSourceChange(genSource, SourceJournalValue.FromSource(genSource), null);

            Assert.AreEqual(1, genGenealogy.JournalEntries.Count);
            Assert.IsInstanceOfType<SourceJournalValue>(genGenealogy.JournalEntries[0].Data);
            Assert.AreEqual("Kirchenbuch", ((SourceJournalValue)genGenealogy.JournalEntries[0].Data!).Description);
        }

        [TestMethod]
        public void RecordMediaChange_StoresTypedMediaJournalSnapshot()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger)
            {
                UId = Guid.NewGuid()
            };
            var genMedia = Substitute.For<IGenMedia>();
            genMedia.eMediaType.Returns(EMediaType.Picture);
            genMedia.MediaName.Returns("Portrait");
            genMedia.MediaDescription.Returns("Porträtfoto");
            genMedia.MediaUri.Returns(new Uri("file:///c:/temp/portrait.jpg"));

            genGenealogy.RecordMediaChange(genMedia, MediaJournalValue.FromMedia(genMedia), null);

            Assert.AreEqual(1, genGenealogy.JournalEntries.Count);
            Assert.IsInstanceOfType<MediaJournalValue>(genGenealogy.JournalEntries[0].Data);
            Assert.AreEqual("Portrait", ((MediaJournalValue)genGenealogy.JournalEntries[0].Data!).MediaName);
        }

        [TestMethod]
        public void RecordRepositoryChange_StoresTypedRepositoryJournalSnapshot()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger)
            {
                UId = Guid.NewGuid()
            };
            var genSources = Substitute.For<IIndexedList<IGenSource>>();
            genSources.Count.Returns(2);

            var genRepository = Substitute.For<IGenRepository>();
            genRepository.Name.Returns("Landeskirchliches Archiv");
            genRepository.Info.Returns("Bestand A");
            genRepository.Uri.Returns(new Uri("https://example.org/archive"));
            genRepository.GenSources.Returns(genSources);

            genGenealogy.RecordRepositoryChange(genRepository, RepositoryJournalValue.FromRepository(genRepository), null);

            Assert.AreEqual(1, genGenealogy.JournalEntries.Count);
            Assert.IsInstanceOfType<RepositoryJournalValue>(genGenealogy.JournalEntries[0].Data);
            Assert.AreEqual("Landeskirchliches Archiv", ((RepositoryJournalValue)genGenealogy.JournalEntries[0].Data!).Name);
            Assert.AreEqual(2, ((RepositoryJournalValue)genGenealogy.JournalEntries[0].Data!).SourceCount);
        }
    }
}
