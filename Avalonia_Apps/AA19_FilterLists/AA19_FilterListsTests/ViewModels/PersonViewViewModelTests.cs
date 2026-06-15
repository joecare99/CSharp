// ***********************************************************************
// Assembly         : ListBindingTests
// Author           : Mir
// Created          : 06-17-2022
//
// Last Modified By : Mir
// Last Modified On : 06-17-2022
// ***********************************************************************
// <copyright file="PersonViewViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AA19_FilterLists.Model;

namespace AA19_FilterLists.ViewModels.Tests
{
    /// <summary>
    /// Defines test class PersonViewViewModelTests.
    /// </summary>
    [TestClass()]
    public class PersonViewViewModelTests
    {
        /// <summary>
        /// Defines the test method PersonViewViewModelTest.
        /// </summary>
        [TestMethod()]
        public void PersonViewViewModelTest()
        {
            var model = new PersonViewViewModel(new TestPersons(new Person("Mustermann", "Max", "Dr.")));
            Assert.IsNotNull(model.ActPerson);
            Assert.IsNotNull(model.Persons);
            Assert.AreEqual(1, model.Persons.Count);
            Assert.IsTrue(model.ActPerson.IsEmpty);
        }

        [TestMethod()]
        public void ClearFilterCommandResetsFilterAndRestoresAllPersons()
        {
            var model = new PersonViewViewModel(new TestPersons(
                new Person("Mustermann", "Max", "Dr."),
                new Person("Miller", "Jane")));

            model.Filter = "dr.";
            Assert.AreEqual(1, model.FilteredPersons.Count);

            model.ClearFilterCommand.Execute(null);

            Assert.AreEqual(string.Empty, model.Filter);
            Assert.AreEqual(2, model.FilteredPersons.Count);
        }

        [TestMethod()]
        public void NewPersonCommandResetsSelectedExistingPerson()
        {
            var existingPerson = new Person("Mustermann", "Max", "Dr.") { Id = 7 };
            var model = new PersonViewViewModel(new TestPersons(existingPerson));

            model.ActPerson = existingPerson;
            model.NewPersonCommand.Execute(null);

            Assert.AreNotSame(existingPerson, model.ActPerson);
            Assert.AreEqual(0, model.ActPerson.Id);
            Assert.IsTrue(model.ActPerson.IsEmpty);
        }

        [TestMethod()]
        public void AddCommandWritesNewPersonIntoDataService()
        {
            var model = new PersonViewViewModel(new TestPersons(new Person("Mustermann", "Max", "Dr.")));
            model.ActPerson.LastName = "Care";
            model.ActPerson.FirstName = "Joe";
            model.ActPerson.Title = "Prof.";

            model.AddPersonCommand.Execute(null);

            Assert.AreEqual(2, model.Persons.Count);
            Assert.AreEqual("Care", model.Persons[1].LastName);
            Assert.AreEqual(2, model.Persons[1].Id);
            Assert.IsTrue(model.ActPerson.IsEmpty);
        }
    }
}