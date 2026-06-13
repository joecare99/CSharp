using AA19_FilterLists.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA19_FilterLists.Model.Tests;

[TestClass]
public class PersonDataServiceTests
{
    [TestMethod]
    public void DefaultServiceProvidesInitialPersonFromModelService()
    {
        var service = new PersonDataService();

        Assert.AreEqual(1, service.Persons.Count);
        Assert.AreEqual("Mustermann", service.Persons[0].LastName);
        Assert.AreEqual(1, service.Persons[0].Id);
    }

    [TestMethod]
    public void AddPersonAssignsNextId()
    {
        var service = new PersonDataService(
            new[]
            {
                new Person("Miller", "Jane") { Id = 4 },
                new Person("Mustermann", "Max") { Id = 7 }
            });
        var person = new Person("Care", "Joe");

        service.AddPerson(person);

        Assert.AreEqual(8, person.Id);
        Assert.AreEqual(3, service.Persons.Count);
        Assert.AreSame(person, service.Persons[2]);
    }
}
