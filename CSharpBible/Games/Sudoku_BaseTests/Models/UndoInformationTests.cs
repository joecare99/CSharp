using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku_Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseLib.Helper.TestHelper;

namespace Sudoku_Base.Models.Tests;

[TestClass()]
public class UndoInformationTests
{
    [DataTestMethod()]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, null)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, true)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, new[] {3,5,7 })]

    public void UndoInformationTest(int[] aiPos, int? value, bool isPredefined, int[] pValues,object ov)
    {
        // Arrange
        var field = new SudokuField(new System.Drawing.Point(aiPos[0], aiPos[1]), value, isPredefined, pValues);
        // Act
        var testModel = new UndoInformation(field, [(ov, null)]);
        // Assert
        Assert.IsNotNull(testModel);
        Assert.IsTrue(testModel.Field.TryGetTarget(out var f2));
        AssertAreEqual(field, f2, []);
        Assert.AreEqual(1, testModel.list.Count);
        Assert.AreEqual((ov, null), testModel.list[0]);
    }

    [TestMethod()]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, 5, null, true)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, null, 5, false)]
    [DataRow(new int[] { 1, 2 }, null, true, new int[] { 1, 2, 3, 4, 5 }, 5, null, false)]
    [DataRow(new int[] { 1, 2 }, null, true, new int[] { 1, 2, 3, 4, 5 }, null, 5, true)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, true, false, true)]
    [DataRow(new int[] { 1, 2 }, 5, false, new int[] { 1, 2, 3, 4, 5 }, true, false, false)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, new[] { 5 }, new[] { 3, 7 }, true)]
    public void RedoTest(int[] aiPos, int? value, bool isPredefined, int[] pValues, object ov, object nv, bool xExp)
    {
        // Arrange
        var field = new SudokuField(new System.Drawing.Point(aiPos[0], aiPos[1]), value, isPredefined, pValues);
        var testModel = new UndoInformation(field, [(ov, null)]);
        testModel.TryUpdateNewValue(nv);
        // Act
        testModel.Redo();
        // Assert
        switch (nv, xExp)
        {
            case (int i, true):
            case (null, true):
                Assert.AreEqual(nv, field.Value);
                break;
            case (bool b, true):
                Assert.AreEqual(b, field.IsPredefined);
                break;
            case (IEnumerable<int> li, true):
                Assert.IsTrue(field.PossibleValues.SequenceEqual(li));
                break;
            case (int i, false):
            case (null, false):
                Assert.AreEqual(value, field.Value);
                break;
            case (bool b, false):
                Assert.AreEqual(isPredefined, field.IsPredefined);
                break;
            case (IEnumerable<int> li, false):
                Assert.IsTrue(field.PossibleValues.SequenceEqual(pValues));
                break;
            default:
                Assert.Fail();
                break;
        }
    }

    [TestMethod()]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, 5, null,false)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, null, 5,true)]
    [DataRow(new int[] { 1, 2 }, null, true, new int[] { 1, 2, 3, 4, 5 }, 5, null,true)]
    [DataRow(new int[] { 1, 2 }, null, true, new int[] { 1, 2, 3, 4, 5 }, null, 5,false)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, true, false,false)]
    [DataRow(new int[] { 1, 2 }, 5, false, new int[] { 1, 2, 3, 4, 5 }, true, false,true)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, new[] { 5 }, new[] { 3, 7 },true)]
    public void UndoTest(int[] aiPos, int? value, bool isPredefined, int[] pValues, object ov, object nv,bool xExp)
    {
        // Arrange
        var field = new SudokuField(new System.Drawing.Point(aiPos[0], aiPos[1]), value, isPredefined, pValues);
        var testModel = new UndoInformation(field, [(ov, null)]);
        testModel.TryUpdateNewValue(nv);
        // Act
        testModel.Undo();
        // Assert
        switch (ov,xExp)
        {
            case (int i,true):
            case (null,true):
                Assert.AreEqual(ov, field.Value);
                break;
            case (bool b,true):
                Assert.AreEqual(b, field.IsPredefined);
                break;
            case (IEnumerable<int> li,true):
                Assert.IsTrue(field.PossibleValues.SequenceEqual(li));
                break;
            case (int i, false):
            case (null, false):
                Assert.AreEqual(value, field.Value);
                break;
            case (bool b, false):
                Assert.AreEqual(isPredefined, field.IsPredefined);
                break;
            case (IEnumerable<int> li, false):
                Assert.IsTrue(field.PossibleValues.SequenceEqual(pValues));
                break;
            default:
                Assert.Fail();
                break;
        }
    }

    [DataTestMethod()]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, 5,null)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, null,5)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, true,false)]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, new[] { 5 },new[] { 3, 7 })]
    [DataRow(new int[] { 1, 2 }, 5, true, new int[] { 1, 2, 3, 4, 5 }, null ,"Hello")]

    public void TryUpdateNewValueTest(int[] aiPos, int? value, bool isPredefined, int[] pValues, object ov,object nv)
    {
        // Arrange
        var field = new SudokuField(new System.Drawing.Point(aiPos[0], aiPos[1]), value, isPredefined, pValues);
        var testModel = new UndoInformation(field, [(ov, null)]);
        // Act
        testModel.TryUpdateNewValue(nv);
        // Assert
        Assert.AreEqual(1, testModel.list.Count);
        Assert.AreEqual((ov, nv is string?null:nv), testModel.list[0]);
    }
}