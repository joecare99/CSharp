using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_28_1_CTDataGridExt.Services;
using MVVM_28_1_CTDataGridExt.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace MVVM_28_1_CTDataGridExt.ViewModels.Tests;

[TestClass]
public class DataGridViewModelTests:BaseTestViewModel<DataGridViewModel>
{
    private IPersonService _testPS;

    [TestInitialize]
    public override void Init()
    {

        _testPS = Substitute.For<IPersonService>();
        _testPS.GetPersons().Returns(new[]  { new Person() {Id = 1, FirstName="F",LastName="L" } });
        _testPS.GetDepartments().Returns(new[] { new Department() { Id = 1 } });
        IoC.GetReqSrv=(t)=>t switch
        {
            _ when t==typeof(IPersonService)=>_testPS,
            _=>throw new System.NotImplementedException()
        };
        base.Init();
    }
    [TestMethod]
    public void SetupTest() {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(DataGridViewModel));
        Assert.AreEqual(1, testModel.Persons.Count);
        Assert.AreEqual(1, testModel.Departments.Count);
    }

    protected override Dictionary<string, object?> GetDefaultData() 
        => new() { 
            { nameof(DataGridViewModel.IsItemSelected),false},
        };

    [TestMethod]
    public void IsItemSelectedTest()
    {
        var bExp = GetDefaultData()[nameof(DataGridViewModel.IsItemSelected)];
        Assert.AreEqual(bExp, testModel.IsItemSelected);
    }

    [TestMethod]
    public void PersonsTest()
    {
        var aExp = _testPS.GetPersons().First();
        testModel.SelectedPerson = aExp;
        Assert.IsTrue(testModel.IsItemSelected);
    }

    [TestMethod]
    public void RemovePersonTest()
    {
        var aExp = _testPS.GetPersons().First();
        testModel.RemoveCommand.Execute(aExp);
        Assert.AreEqual(0,testModel.Persons.Count);
    }
}
