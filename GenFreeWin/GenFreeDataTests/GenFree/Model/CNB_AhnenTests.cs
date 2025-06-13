using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaseLib.Interfaces;
using GenFree.GenFree.Model;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;


namespace GenFree.Model.Tests;

[TestClass]
public class CNB_AhnenTests
{
    private IRecordset _recordset;
    private Func<IRecordset> _recordsetFactory;
    private CNB_Ahnen _sut;

    [TestInitialize]
    public void SetUp()
    {
        _recordset = Substitute.For<IRecordset>();
        _recordsetFactory = () => _recordset;
        _sut = new CNB_Ahnen(_recordsetFactory);
    }

    [TestMethod]
    [DataRow(1, true)]
    [DataRow(2, false)]
    public void PersonExists_ReturnsExpected(int persNr, bool exists)
    {
        _recordset.NoMatch.Returns(!exists);
     
        var result = _sut.PersonExists(persNr);

        Assert.AreEqual(exists,result);
        _recordset.Received().Seek("=", persNr);
        _recordset.Received().Index = nameof(NB_AhnenIndex.PerNR);
    }

    [TestMethod]
    [DataRow(1, 2, 3, 4, 5, 6, true)]
    [DataRow(1, 0, 0, 0, 0, 0, false)]
    public void ReadData_ReturnsExpected(
        int lfdNr, int persNrVal, int famNrVal, int genVal, int kek2Val, int kek1Val, bool exists)
    {
        _recordset.NoMatch.Returns(!exists);
        (_recordset.Fields[NB_AhnenFields.Ahn1] as IHasValue).Value.Returns(kek1Val);
        (_recordset.Fields[NB_AhnenFields.Ahn2] as IHasValue).Value.Returns(kek2Val);
        (_recordset.Fields[NB_AhnenFields.Ahn2] as IHasValue).Value.Returns(kek2Val);
        (_recordset.Fields[NB_AhnenFields.Ahn2] as IHasValue).Value.Returns(kek2Val);
        (_recordset.Fields[NB_AhnenFields.Ahn2] as IHasValue).Value.Returns(kek2Val);

        var result = _sut.ReadData(lfdNr, out int persNr, out int famNr, out int gen, out int kek2, out int kek1);

        Assert.AreEqual(exists, result);
        if (exists)
        {
            Assert.AreEqual(persNrVal, persNr);
            Assert.AreEqual(famNrVal, famNr);
            Assert.AreEqual(genVal, gen);
            Assert.AreEqual(kek1Val, kek1);
            Assert.AreEqual(kek2Val, kek2);
        }
        else
        {
            Assert.AreEqual(0, persNr);
            Assert.AreEqual(0, famNr);
            Assert.AreEqual(0, gen);
            Assert.AreEqual(0, kek1);
            Assert.AreEqual(0, kek2);
        }
    }

    [TestMethod]
    [DataRow(1, 2, 3, true)]
    [DataRow(1, 0, 0, false)]
    public void CReadData_ReturnsExpected(int num6, int genVal, int persVal, bool exists)
    {

        var result = _sut.CReadData(num6, out var value);

        Assert.AreEqual(exists,result);
        if (exists)
        {
            Assert.IsNotNull(value);
            Assert.AreEqual(genVal,value.Value.iGen);
            Assert.AreEqual(persVal, value.Value.iPers);
        }
        else
        {
            Assert.IsNull(value);
        }
    }

    [TestMethod]
    public void AddRow_SetsAllFieldsAndUpdates()
    {
        var fields = Substitute.For<IFieldsCollection>();
        _recordset.Fields.Returns(fields);

        fields[NB_AhnenFields.PerNr].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Gene].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Ahn1].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Ahn2].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Ahn3].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Weiter].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Ehe].Returns(Substitute.For<IField>());

        _sut.AddRow(1, 2, 3, 4, 5, 6);

        _recordset.Received().AddNew();
        fields[NB_AhnenFields.PerNr].Received().Value = 1;
        fields[NB_AhnenFields.Gene].Received().Value = 2;
        fields[NB_AhnenFields.Ahn1].Received().Value = 4;
        fields[NB_AhnenFields.Ahn2].Received().Value = 5;
        fields[NB_AhnenFields.Ahn3].Received().Value = 0;
        fields[NB_AhnenFields.Weiter].Received().Value = 3;
        fields[NB_AhnenFields.Ehe].Received().Value = 6;
        _recordset.Received().Update();
    }

    [TestMethod]
    public void EditRaw_SetsFieldsAndUpdates()
    {
        var fields = Substitute.For<IFieldsCollection>();
        _recordset.Fields.Returns(fields);

        fields[NB_AhnenFields.Gene].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Ahn1].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Ahn2].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Ehe].Returns(Substitute.For<IField>());
        fields[NB_AhnenFields.Name].Returns(Substitute.For<IField>());

        _sut.EditRaw(1, 2, 3, 4, "Test");

        _recordset.Received().Edit();
        fields[NB_AhnenFields.Gene].Received().Value = 1;
        fields[NB_AhnenFields.Ahn1].Received().Value = 2;
        fields[NB_AhnenFields.Ahn2].Received().Value = 3;
        fields[NB_AhnenFields.Ehe].Received().Value = 4;
        fields[NB_AhnenFields.Name].Received().Value = "Test";
        _recordset.Received().Update();
    }

    [TestMethod]
    public void SetWeiterRaw_SetsWeiterAndUpdates()
    {
        var fields = Substitute.For<IFieldsCollection>();
        _recordset.Fields.Returns(fields);
        fields[NB_AhnenFields.Weiter].Returns(Substitute.For<IField>());

        _sut.SetWeiterRaw();

        _recordset.Received().Edit();
        fields[NB_AhnenFields.Weiter].Received().Value = 1;
        _recordset.Received().Update();
    }

    [TestMethod]
    [DataRow(false, 0, true, false, false, DisplayName = "Commit_AddRow_WhenPersonNotExists")]
    [DataRow(true, 1, true, false, true, DisplayName ="Commit_SetWeiterAndAddRow_WhenAhn1NotZero")]
    [DataRow(true, 0, false, true, false, DisplayName ="Commit_EditRaw_WhenAhn1Zero")]
    public void Commit_BehavesCorrectly(bool personExists, int ahn1, bool expectAdd, bool expectEdit, bool expectSetWeiter)
    {
        _sut = Substitute.ForPartsOf<CNB_Ahnen>(_recordsetFactory);
        _sut.PersonExists(1).Returns(personExists);
        (_recordset.Fields[NB_AhnenFields.Ahn1] as IHasValue).Value.Returns(ahn1);

        _sut.When(x => x.AddRow(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())).DoNotCallBase();
        _sut.When(x => x.EditRaw(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>())).DoNotCallBase();
        _sut.When(x => x.SetWeiterRaw()).DoNotCallBase();

        _sut.Commit(1, 2, 3, 4, 5, "Name");

        if (expectAdd)
            _sut.Received().AddRow(1, 3, Arg.Any<int>(), 4, 5, 2);
        else
            _sut.DidNotReceive().AddRow(1, 3, Arg.Any<int>(), 4, 5, 2);

        if (expectEdit)
            _sut.Received().EditRaw(3, 4, 5, 2, "Name");
        else
            _sut.DidNotReceive().EditRaw(3, 4, 5, 2, "Name");

        if (expectSetWeiter)
            _sut.Received().SetWeiterRaw();
        else
            _sut.DidNotReceive().SetWeiterRaw();
    }

}


public static class NB_AhnenFields
{
    public const string PerNr = "PerNr";
    public const string Gene = "Gene";
    public const string Ahn1 = "Ahn1";
    public const string Ahn2 = "Ahn2";
    public const string Ahn3 = "Ahn3";
    public const string Weiter = "Weiter";
    public const string Ehe = "Ehe";
    public const string Name = "Name";
}

public static class NB_AhnenIndex
{
    public const string LfNr = "LfNr";
    public const string PerNR = "PerNR";
}
