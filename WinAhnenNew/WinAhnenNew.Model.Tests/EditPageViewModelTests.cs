using System;
using System.Linq;
using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;
using CommunityToolkit.Mvvm.Messaging;
using BaseGenClasses.Model;
using BaseLib.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using NSubstitute;
using WinAhnenNew.Messages;
using WinAhnenNew.Services;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Model.Tests
{
    [TestClass]
    public class EditPageViewModelTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            IoC.GetReqSrv = static tServiceType => tServiceType == typeof(IGenILBuilder)
                ? new GenILBuilder()
                : throw new InvalidOperationException($"No service for {tServiceType}");
            IoC.GetSrv = static tServiceType => tServiceType == typeof(IGenILBuilder)
                ? new GenILBuilder()
                : null;
        }

        [TestMethod]
        public void LastNameChange_UpdatesUnderlyingSelectedPersonWithoutPersistingImmediately()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger) { UId = Guid.NewGuid() };
            var genPerson = CreatePerson(genGenealogy, 1, "P-1", "Anna", "Meyer", "Enger");
            var svcPersonSelection = Substitute.For<IPersonSelectionService>();
            svcPersonSelection.SelectedPerson.Returns(genPerson);
            svcPersonSelection.GetSelectablePersons().Returns(new[] { genPerson });

            var vmEdit = new EditPageViewModel(svcPersonSelection, msgMessenger);

            vmEdit.LastName = "Schulze";

            Assert.AreEqual("Schulze", genPerson.Surname);
            svcPersonSelection.DidNotReceive().SaveChanges();
        }

        [TestMethod]
        public void PersistSelectedPersonChanges_UpdatesUnderlyingSelectedPerson()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger) { UId = Guid.NewGuid() };
            var genPerson = CreatePerson(genGenealogy, 1, "P-1", "Anna", "Meyer", "Enger");
            var svcPersonSelection = Substitute.For<IPersonSelectionService>();
            svcPersonSelection.SelectedPerson.Returns(genPerson);
            svcPersonSelection.GetSelectablePersons().Returns(new[] { genPerson });

            var vmEdit = new EditPageViewModel(svcPersonSelection, msgMessenger);

            vmEdit.LastName = "Schulze";
            vmEdit.FirstNames = "Maria";
            vmEdit.FarmName = "Hof Schulze";
            vmEdit.BirthPlace = "Herford";
            vmEdit.PersistSelectedPersonChanges();

            Assert.AreEqual("Schulze", genPerson.Surname);
            Assert.AreEqual("Maria", genPerson.GivenName);
            Assert.AreEqual("Hof Schulze", GetFactData(genPerson, EFactType.Property));
            Assert.AreEqual("Herford", genPerson.BirthPlace?.Name);
            svcPersonSelection.Received().SaveChanges();
        }

        [TestMethod]
        public void SelectedPersonChangedMessage_RestoresPersistedValuesAfterSwitchingBackToPerson()
        {
            var msgMessenger = new WeakReferenceMessenger();
            var genGenealogy = new Genealogy(msgMessenger) { UId = Guid.NewGuid() };
            var genFirstPerson = CreatePerson(genGenealogy, 1, "P-1", "Anna", "Meyer", "Enger");
            var genSecondPerson = CreatePerson(genGenealogy, 2, "P-2", "Karl", "Becker", "Bünde");
            var svcPersonSelection = Substitute.For<IPersonSelectionService>();
            svcPersonSelection.SelectedPerson.Returns(genFirstPerson);
            svcPersonSelection.GetSelectablePersons().Returns(new[] { genFirstPerson, genSecondPerson });

            var vmEdit = new EditPageViewModel(svcPersonSelection, msgMessenger);

            vmEdit.LastName = "Schulze";
            vmEdit.FirstNames = "Maria";
            vmEdit.PersistSelectedPersonChanges();

            svcPersonSelection.SelectedPerson.Returns(genSecondPerson);
            msgMessenger.Send(new SelectedPersonChangedMessage(genSecondPerson));

            Assert.AreEqual("Becker", vmEdit.LastName);
            Assert.AreEqual("Karl", vmEdit.FirstNames);

            svcPersonSelection.SelectedPerson.Returns(genFirstPerson);
            msgMessenger.Send(new SelectedPersonChangedMessage(genFirstPerson));

            Assert.AreEqual("Schulze", vmEdit.LastName);
            Assert.AreEqual("Maria", vmEdit.FirstNames);
            Assert.AreEqual("Schulze", genFirstPerson.Surname);
            Assert.AreEqual("Maria", genFirstPerson.GivenName);
        }

        private static GenPerson CreatePerson(IGenealogy genGenealogy, int iId, string sReferenceId, string sGivenName, string sSurname, string sBirthPlace)
        {
            var genPerson = new GenPerson
            {
                UId = Guid.NewGuid(),
                ID = iId
            };

            ((IHasOwner<IGenealogy>)genPerson).SetOwner(genGenealogy);
            genGenealogy.Entitys.Add(genPerson);

            var genPlace = new GenPlace(sBirthPlace)
            {
                UId = Guid.NewGuid(),
                ID = genGenealogy.Places.Count + 1
            };
            ((IHasOwner<IGenealogy>)genPlace).SetOwner(genGenealogy);
            genGenealogy.Places.Add(genPlace);

            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Reference)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 1,
                Data = sReferenceId
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Info)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 2,
                Data = string.Empty
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Givenname)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 3,
                Data = sGivenName
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Surname)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 4,
                Data = sSurname
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Occupation)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 5,
                Data = string.Empty
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Property)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 6,
                Data = string.Empty
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Sex)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 7,
                Data = "F"
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Religion)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 8,
                Data = "ev."
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Adoption)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 9,
                Data = string.Empty
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Description)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 10,
                Data = string.Empty
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Birth)
            {
                UId = Guid.NewGuid(),
                ID = iId * 100 + 11,
                Date = new GenDate(new DateTime(1900 + iId, 1, 2))
                {
                    UId = Guid.NewGuid(),
                    ID = iId * 100 + 12,
                    DateText = $"02.01.{1900 + iId}"
                },
                Place = genPlace
            });

            return genPerson;
        }

        private static string GetFactData(IGenPerson genPerson, EFactType eFactType)
            => genPerson.Facts.FirstOrDefault(genFact => genFact?.eFactType == eFactType)?.Data ?? string.Empty;
    }
}
