using AA28_DataGridExt.Model;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AA28_DataGridExt.Tests.Services;

[TestClass]
public class PersonServiceTests
{
    private Func<Type, object?>? _previousGetService;
    private Func<Type, object>? _previousGetRequiredService;
    private PersonService? _personService;

    [TestInitialize]
    public void Initialize()
    {
        _previousGetService = IoC.GetSrv;
        _previousGetRequiredService = IoC.GetReqSrv;
        IoC.GetReqSrv = type => type == typeof(IRandom) ? new CRandom() : throw new NotImplementedException();
        IoC.GetSrv = _ => null;
        _personService = new PersonService();
    }

    [TestCleanup]
    public void Cleanup()
    {
        IoC.GetSrv = _previousGetService!;
        IoC.GetReqSrv = _previousGetRequiredService!;
    }

    [TestMethod]
    public void GetPersonsReturnsExpectedCountTest()
    {
        var result = _personService!.GetPersons().ToArray();

        Assert.AreEqual(14, result.Length);
        Assert.IsTrue(result.All(person => person.Department is not null));
    }

    [TestMethod]
    public void GetDepartmentsReturnsExpectedCountTest()
    {
        var result = _personService!.GetDepartments();

        Assert.AreEqual(4, result.Length);
        Assert.IsTrue(result.All(department => !string.IsNullOrWhiteSpace(department.Name)));
        Assert.IsTrue(result.All(department => !string.IsNullOrWhiteSpace(department.Description)));
    }

    [TestMethod]
    public void GetPersonsReturnsPersonInstancesTest()
    {
        var result = _personService!.GetPersons();

        Assert.IsInstanceOfType(result, typeof(IEnumerable<Person>));
    }
}
