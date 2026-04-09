using CommunityToolkit.Mvvm.Messaging;
using BaseGenClasses.Model;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using NSubstitute;
using WinAhnenNew.Messages;
using WinAhnenNew.Services;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Model.Tests
{
    [TestClass]
    public class SelectionPageViewModelTests
    {
        [TestMethod]
        public void SelectPersonCommand_SetsSelectedPersonAndRequestsEditTabNavigation()
        {
            var svcPersonSelection = Substitute.For<IPersonSelectionService>();
            var msgMessenger = new WeakReferenceMessenger();
            var tstRecipient = new NavigateToEditTabRecipient();
            msgMessenger.Register<NavigateToEditTabMessage>(tstRecipient, static (objRecipient, _) =>
            {
                if (objRecipient is NavigateToEditTabRecipient rcpRecipient)
                {
                    rcpRecipient.XReceived = true;
                }
            });

            var genPerson = new GenPerson
            {
                UId = System.Guid.NewGuid(),
                ID = 17
            };
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Surname) { Data = "Meyer" });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Givenname) { Data = "Anna" });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Reference) { Data = "P-17" });

            svcPersonSelection.GetSelectablePersons().Returns(new[] { genPerson });

            var vmSelection = new SelectionPageViewModel(svcPersonSelection, msgMessenger)
            {
                SelectedPerson = new SelectionPersonItemViewModel(genPerson)
            };

            vmSelection.SelectPersonCommand.Execute(null);

            svcPersonSelection.Received(1).SetSelectedPerson(genPerson);
            Assert.IsTrue(tstRecipient.XReceived);
        }

        private sealed class NavigateToEditTabRecipient
        {
            public bool XReceived { get; set; }
        }
    }
}
