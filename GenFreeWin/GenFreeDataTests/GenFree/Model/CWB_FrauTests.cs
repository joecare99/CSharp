using GenFree.GenFree.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Interfaces.DB;
using NSubstitute;
using GenFree.Data;
using GenFree.Interfaces.Data;

namespace GenFree.GenFree.Model.Tests;

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

}