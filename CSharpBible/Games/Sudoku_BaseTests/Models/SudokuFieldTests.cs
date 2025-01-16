using System.Collections.Generic;
using System.Linq;
using MVVM.ViewModel;
using System.Drawing;
using System.Diagnostics;
using static BaseLib.Helper.TestHelper;
using Sudoku_Base.Models.Interfaces;

namespace Sudoku_Base.Models.Tests;

[TestClass()]
public class SudokuFieldTests : BaseTestViewModel<SudokuField>
{
    public static IEnumerable<object[]> SudokuFieldTestData => [
        [new byte[] { 0x03, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00 },
        new SudokuField(new Point(3, 5), null, false, [1, 3, 7])],
        [new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 },
            new SudokuField(new Point(0, 0), 5, true, [])],
        [new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 },
                new SudokuField(new Point(0, 0), 5, true, [0, 1, 2])],
        [new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x06, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00 },
                    new SudokuField(new Point(0, 0), null, false, [1, 2, 5, 7, 8, 9])]
    ];

    [TestMethod()]
    public void SetUpTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(SudokuField));
        Assert.AreEqual(null, testModel.Value);
        Assert.AreEqual(false, testModel.IsPredefined);
        Assert.AreEqual(0, testModel.PossibleValues.Count);
    }

    [DataTestMethod()]
    [DataRow(new int[] { 1, 2, 3, 4, 5 }, 5)]
    [DataRow(new int[] { 1 }, 1)]
    [DataRow(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 9)]
    [DataRow(new int[] { 1, 1, 3, 3, 5, 5 }, 3)]

    public void AddPossibleValueTest(int[] aiAct, int iCnt)
    {
        foreach (var iAct in aiAct)
        {
            testModel.AddPossibleValue(iAct);
            Assert.IsTrue(testModel.PossibleValues.Contains(iAct));
        }
        Assert.AreEqual(iCnt, testModel.PossibleValues.Count);
    }

    [DataTestMethod()]
    [DataRow(new int[] { 1, 2, 3, 4, 5 }, 1)]
    [DataRow(new int[] { 1, 2, 3, 4, 5 }, 3)]
    [DataRow(new int[] { 1, 2, 3, 4, 5 }, 5)]
    [DataRow(new int[] { 1, 2, 3, 4, 5 }, 0)]
    [DataRow(new int[] { 1 }, 1)]
    [DataRow(new int[] { 1 }, 0)]
    [DataRow(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2)]
    [DataRow(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 4)]
    [DataRow(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 6)]
    [DataRow(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 10)]
    [DataRow(new int[] { 1, 1, 3, 3, 5, 5 }, 3)]
    [DataRow(new int[] { 1, 1, 3, 3, 5, 5 }, 1)]
    [DataRow(new int[] { 1, 1, 3, 3, 5, 5 }, 2)]
    public void RemovePossibleValueTest(int[] aiAct, int iAct)
    {
        // Arrange
        foreach (var iAct2 in aiAct)
            testModel.AddPossibleValue(iAct2);
        // Act
        testModel.RemovePossibleValue(iAct);
        // Assert
        Assert.IsFalse(testModel.PossibleValues.Contains(iAct));
    }

    [TestMethod()]
    [DynamicData(nameof(SudokuFieldTestData))]
    public void ReadFromStreamTest(byte[] bytes, SudokuField field)
    {
        // Arrange
        var stream = new System.IO.MemoryStream(bytes);
        // Act
        testModel.ReadFromStream(stream);
        // Assert
        AssertAreEqual(field, testModel, []);
    }

    [DataTestMethod()]
    [DynamicData(nameof(SudokuFieldTestData))]
    public void WriteToStreamTest(byte[] bytes, SudokuField field)
    {
        // Arrange
        var stream = new System.IO.MemoryStream();
        // Act
        field.WriteToStream(stream);
        // Assert
        Debug.WriteLine(string.Join(",", stream.ToArray().Select(b => $"0x{b:X2}")));
        CollectionAssert.AreEqual(bytes, stream.ToArray());
    }

    [DataTestMethod()]
    [DataRow(new[] { 3, 5 }, null, false, new[] { 1, 3, 7 })]
    [DataRow(new[] { 0, 0 }, 5, true, new int[] { })]
    [DataRow(new[] { 0, 0 }, 5, true, new int[] { 0, 1, 2 })]
    [DataRow(new[] { 0, 0 }, null, false, new int[] { 1, 2, 5, 7, 8, 9 })]

    public void SudokuFieldTest(int[] aiPos, int? value, bool isPredefined, int[] pValues)
    {
        // Act
        var field = new SudokuField(new Point(aiPos[0], aiPos[1]), value, isPredefined, pValues);
        // Assert
        Assert.AreEqual(new Point(aiPos[0], aiPos[1]), field.Position);
        Assert.AreEqual(value, field.Value);
        Assert.AreEqual(isPredefined, field.IsPredefined);
        CollectionAssert.AreEqual(pValues, field.PossibleValues);
        CollectionAssert.AreEqual(pValues.ToList(), (field as ISudokuField).PossibleValues.ToList());
    }

    [TestMethod()]
    [DataRow(new[] { 3, 5 }, null, false, new[] { 1, 3, 7 })]
    [DataRow(new[] { 0, 0 }, 5, true, new int[] { })]
    [DataRow(new[] { 0, 0 }, 5, true, new int[] { 0, 1, 2 })]
    [DataRow(new[] { 0, 0 }, null, false, new int[] { 1, 2, 5, 7, 8, 9 })]
    public void ClearTest(int[] aiPos, int? value, bool isPredefined, int[] pValues)
    {
        // Arrange
        var field = new SudokuField(new Point(aiPos[0], aiPos[1]), value, isPredefined, pValues);
        // Act
        field.Clear();
        // Assert
        Assert.AreEqual(null,field.Value);
        Assert.AreEqual(false, field.IsPredefined);
        Assert.AreEqual(0, field.PossibleValues.Count);
    }
}