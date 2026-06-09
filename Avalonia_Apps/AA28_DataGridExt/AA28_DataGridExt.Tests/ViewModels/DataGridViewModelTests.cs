using AA28_DataGridExt.Model;
using AA28_DataGridExt.ViewModels;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AA28_DataGridExt.Tests.ViewModels;

[TestClass]
public class DataGridViewModelTests
{
    private IPersonService? _personService;
    private DataGridViewModel? _viewModel;

    [TestInitialize]
    public void Initialize()
    {
        _personService = Substitute.For<IPersonService>();
        _personService.GetPersons().Returns(
        [
            new Person { Id = 1, FirstName = "F", LastName = "L" },
        ]);
        _personService.GetDepartments().Returns(
        [
            new Department { Id = 1, Name = "Management" },
        ]);
        _viewModel = new DataGridViewModel(_personService);
    }

    [TestMethod]
    public void SetupPopulatesCollectionsTest()
    {
        Assert.AreEqual(1, _viewModel!.Persons.Count);
        Assert.AreEqual(1, _viewModel.Departments.Count);
        CollectionAssert.AreEqual(
            new[] { nameof(Person.FirstName), nameof(Person.LastName), nameof(Person.Email), nameof(Person.Department), nameof(Person.Birthday) },
            _viewModel.Columns.ToArray());
    }

    [TestMethod]
    public void SetupReusesDepartmentInstancesFromDepartmentCollectionTest()
    {
        var department = new Department { Id = 7, Name = "Sales" };
        var person = new Person { Id = 1, Department = new Department { Id = 7, Name = "Sales" } };
        var personService = Substitute.For<IPersonService>();
        personService.GetDepartments().Returns([department]);
        personService.GetPersons().Returns([person]);

        var viewModel = new DataGridViewModel(personService);

        Assert.AreSame(viewModel.Departments[0], viewModel.Persons[0].Department);
    }

    [TestMethod]
    public void IsItemSelectedReflectsSelectionTest()
    {
        Assert.IsFalse(_viewModel!.IsItemSelected);

        _viewModel.SelectedPerson = _viewModel.Persons[0];

        Assert.IsTrue(_viewModel.IsItemSelected);
    }

    [TestMethod]
    public void RemoveCommandRemovesSelectedPersonTest()
    {
        var person = _viewModel!.Persons[0];
        _viewModel.SelectedPerson = person;

        _viewModel.RemoveCommand.Execute(person);

        Assert.AreEqual(0, _viewModel.Persons.Count);
        Assert.IsNull(_viewModel.SelectedPerson);
        Assert.IsFalse(_viewModel.IsItemSelected);
    }
}
