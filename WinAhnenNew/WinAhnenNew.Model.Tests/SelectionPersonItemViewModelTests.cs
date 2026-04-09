using System;
using BaseGenClasses.Model;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Model.Tests
{
    [TestClass]
    public class SelectionPersonItemViewModelTests
    {
        [TestMethod]
        public void DisplayName_UsesSurnameAndGivenNameFacts()
        {
            var genPerson = new GenPerson
            {
                UId = Guid.NewGuid(),
                ID = 17
            };
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Surname) { Data = "Meyer" });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Givenname) { Data = "Anna" });

            var vmPerson = new SelectionPersonItemViewModel(genPerson);

            Assert.AreEqual("Meyer, Anna", vmPerson.DisplayName);
        }
    }
}
