using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Model;
using NSubstitute;
using BaseLib.Interfaces;
using BaseLib.Helper;
using System.Linq;

namespace GenFree.Data.Tests;

[TestClass]
public class CRepositoryTests
{
    private IRecordset _mockRecordset;
    private CRepository _repository;

    [TestInitialize]
    public void Setup()
    {
        _mockRecordset = Substitute.For<IRecordset>();
        var fields = Substitute.For<IFieldsCollection>();
        var field = Substitute.For<IField>();
        (field as IHasValue).Value.Returns(42);
        fields[RepoFields.Nr].Returns(field);
        _mockRecordset.Fields.Returns(fields);
        _repository = new CRepository(() => _mockRecordset);
    }

    [TestMethod]
    public void _keyIndex_CorrectIndex()
    {
        var property = typeof(CRepository).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Where(p=>p.Name == "_keyIndex" && p.PropertyType == typeof(RepoIndex)).FirstOrDefault();
        var id = property.GetValue(_repository);
        Assert.IsInstanceOfType(id,typeof(Enum));
        Assert.AreEqual(RepoIndex.Nr, id.AsEnum<RepoIndex>());
    }

    [TestMethod]
    public void _db_Table_ReturnsRecordSet()
    {
        var property = typeof(CRepository).GetProperties( System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Where(p => p.Name == "_db_Table" && p.PropertyType == typeof(IRecordset))
            .FirstOrDefault();
        var data = property.GetValue(_repository);
        Assert.IsInstanceOfType(data, typeof(IRecordset));
        Assert.AreEqual(_mockRecordset, data);
    }
    [TestMethod]
    public void GetID_ReturnsCorrectID()
    {
        var method = typeof(CRepository).GetMethod("GetID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        int id = (int)method.Invoke(_repository, new object[] { _mockRecordset });
        Assert.AreEqual(42, id);
    }

    [TestMethod]
    public void GetData_ReturnsCRepoData()
    {
        var method = typeof(CRepository).GetMethod("GetData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var data = method.Invoke(_repository, new object[] { _mockRecordset, false });
        Assert.IsInstanceOfType(data, typeof(CRepoData));
    }

    [TestMethod]
    public void GetIndex1Field_ReturnsNr()
    {
        var result = _repository.GetIndex1Field(RepoIndex.Nr);
        Assert.AreEqual(RepoFields.Nr, result);
    }
}
