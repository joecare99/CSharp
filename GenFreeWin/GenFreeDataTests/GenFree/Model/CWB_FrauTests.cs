using GenFree.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Data;
using GenFree.Interfaces.Data;
using BaseLib.Interfaces;
using System;

namespace GenFree.Model.Tests;

[TestClass()]
public class CWB_FrauTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private IRecordset _rs;
    private CWB_Frau TestClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize]
    public void Initialize()
    {
        _rs = Substitute.For<IRecordset>();
        TestClass = new CWB_Frau(() => _rs);
    }

    [TestMethod()]
    public void AddParentTest()
    {
        // Arrange
        var family = Substitute.For<IFamilyData>();
        family.Mann.Returns(1);
        family.Frau.Returns(2);
        // Act
        TestClass.AddParent(family);
        // Assert
        _rs.Received(2).AddNew();
        _ = _rs.Received(4).Fields;
        _rs.Fields[NB_Frau1Fields.Nr].Received(2).Value = 0;
        _rs.Received(2).Update();
        _rs.Fields[NB_Frau1Fields.LfNr].Received(1).Value = 2;
        _rs.Fields[NB_Frau1Fields.LfNr].Received(1).Value = 1;
    }

    [TestMethod()]
    public void AddRowTest()
    {
        // Arrange
        int iPerFamNr = 123;
        _rs.Fields[NB_Frau1Fields.Nr].Value = 1;
        _rs.Fields[NB_Frau1Fields.Nr].ClearReceivedCalls();
        _rs.ClearReceivedCalls();
        // Act
        TestClass.AddRow(iPerFamNr);
        // Assert
        _rs.Received(1).AddNew();
        _ = _rs.Received(2).Fields;
        _rs.Fields[NB_Frau1Fields.LfNr].Received(1).Value = iPerFamNr;
        _rs.Fields[NB_Frau1Fields.Nr].Received(1).Value = 0;
        _rs.Received(1).Update();
    }

    [TestMethod()]
    public void ClearNrTest()
    {
        // Arrange
        _rs.Fields[NB_Frau1Fields.Nr].Value = 1;
        _rs.EOF.Returns(false, true); // Simulate multiple records
        _rs.ClearReceivedCalls();
        // Act
        TestClass.ClearNr();
        // Assert
        _rs.Received(1).MoveFirst();
        _rs.Received(1).Seek(">", 0);
        _rs.Received(1).Edit();
        _rs.Received(1).Fields[NB_Frau1Fields.Nr].Value = 0;
        _rs.Received(1).Update();
        _rs.Received(1).MoveNext();
    }

    [TestMethod()]
    [DataRow(0, true, true)]   // Datensatz nicht gefunden, AddNew wird aufgerufen
    [DataRow(1, false, false)]  // Datensatz gefunden, Edit/Update wird aufgerufen
    public void CommitTest(int iPers, bool noMatch, bool expectAddNew)
    {
        // Arrange
        _rs.NoMatch.Returns(noMatch);
        (_rs.Fields[NB_Frau1Fields.Nr] as IHasValue).Value.Returns(0);
        _rs.ClearReceivedCalls();
        _rs.Fields[NB_Frau1Fields.Nr].ClearReceivedCalls();

        // Act
        TestClass.Commit(iPers);

        // Assert
        _rs.Received(1).Seek("=", iPers);
        _rs.Index = nameof(NB_Frau1Index.LfNr);
        if (expectAddNew)
        {
            _rs.Received(1).AddNew();
            _rs.Fields[NB_Frau1Fields.LfNr].Received(1).Value = iPers;
            _rs.Received(1).Update();
        }
        else
        {
            _rs.Received(1).Edit();
            _rs.Fields[NB_Frau1Fields.Nr].Received(1).Value = 1;
            _rs.Received(1).Update();
            _rs.DidNotReceive().AddNew();
        }
    }

    [TestMethod()]
    [DataRow(true)]
    [DataRow(false)]
    public void UpdateTest(bool xSeek)
    {
        // Arrange
        int iPerNr = 123;
        _rs.NoMatch.Returns(xSeek);
        _rs.Fields[NB_Frau1Fields.Nr].Value = 0;
        // Act
        bool result = TestClass.Update(iPerNr);
        // Assert
        Assert.AreEqual(!xSeek, result);
        _rs.Received(1).Seek("=", iPerNr);
        _rs.Index = nameof(NB_Frau1Index.LfNr);
        if (!xSeek)
        {
            _rs.Received(1).Edit();
            _rs.Fields[NB_Frau1Fields.Nr].Value = 1;
            _rs.Received(1).Update();
        }
        else
        {
            _rs.DidNotReceive().Edit();
            _rs.Fields[NB_Frau1Fields.Nr].Value = 0;
            _rs.DidNotReceive().Update();
        }
    }

    [TestMethod()]
    [DataRow(0, 2, 2)] // 2 Datensätze, beide mit Nr=0, beide werden gelöscht
    [DataRow(1, 2, 0)] // 2 Datensätze, einer mit Nr=0, einer mit Nr=1, nur einer wird gelöscht
    [DataRow(0, 0, 0)] // keine Datensätze, nichts wird gelöscht
    public void DeleteEmptyTest(int nrValue, int recordCount, int expectedDeleteCalls)
    {
        // Arrange
        var rs = _rs;
        var fields = _rs.Fields;
        var field = fields[NB_Frau1Fields.Nr];
        rs.RecordCount.Returns(recordCount);

        // Simuliere Datensätze mit Nr=nrValue
        int current = 0;
        rs.EOF.Returns(ci => current >= recordCount);
        (fields[NB_Frau1Fields.Nr] as IHasValue).Value.Returns(nrValue);
        rs.When(x => x.MoveFirst()).Do(_ => current = 0);
        rs.When(x => x.MoveNext()).Do(_ => current++);
        rs.When(x => x.Delete()).Do(_ => { });

        var testClass = new CWB_Frau(() => rs);

        // Act
        testClass.DeleteEmpty();

        // Assert
        rs.Received(1).MoveFirst();
        rs.Received(expectedDeleteCalls).Delete();
    }

    [TestMethod()]
    [DataRow(1, "Anna", "01.01.2000")]
    [DataRow(2, "Berta", "02.02.2002")]
    public void ForAllTest(int id, string expectedName, string expectedDate)
    {
        // Arrange
        var rs = _rs;
        var fields = rs.Fields;
        int recordCount = 1;
        int current = 0;
        rs.RecordCount.Returns(recordCount);
        rs.EOF.Returns(ci => current >= recordCount);
        (fields[NB_Frau1Fields.LfNr] as IHasValue).Value.Returns(id);
        rs.When(x => x.MoveFirst()).Do(_ => current = 0);
        rs.When(x => x.MoveNext()).Do(_ => current++);

        // Name/Datum-Funktion
        Func<int, (string, string)> getNameDate = i =>
            (i == id ? expectedName : string.Empty, i == id ? expectedDate : string.Empty);

        // Felder für Name und Datum
        var nameField = fields[NB_Frau1Fields.Name];

        // Act
        TestClass.ForAll(getNameDate);

        // Assert
        rs.Received(1).MoveFirst();
        rs.Received(1).Edit();
        nameField.Received(1).Value = expectedName+" "+expectedDate;
        rs.Received(1).Update();
    }
}