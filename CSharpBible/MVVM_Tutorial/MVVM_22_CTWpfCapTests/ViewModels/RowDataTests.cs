// Pseudocode-Plan:
// - Setup: Verwende MSTest + NSubstitute.
// - Test TileColor:
//   - Mit Parent + Model: Model.TileColor so konfigurieren, dass es aus (x,y) -> y*10 + x berechnet.
//   - RowData mit RowId und Parent erstellen.
//   - Erwartete Werte berechnen (y = RowId-1, Werte: y*10 + 0..3).
//   - Prüfen: Länge = 4, Werte korrekt, und TileColor-Aufrufe 4x mit korrekten Parametern.
//   - DataRow für mehrere RowIds.
// - Test TileColor ohne Parent:
//   - Parent = null, RowId variieren.
//   - Prüfen: Array {0,0,0,0}.
// - Test MoveLeftCommand/MoveRightCommand:
//   - Parent-Mocks für MoveLeftCommand/MoveRightCommand bereitstellen.
//   - Prüfen: Weitergabe identisch; Ohne Parent: null.
// - Test This:
//   - Prüfen: ReferenceEquals(This, instance) == true.
// - Test Length:
//   - Prüfen: 4.
// - Test OnPropertyChanged (explizites Interface):
//   - Auf PropertyChanged subscriben, ((IRowData)sut).OnPropertyChanged("TileColor") aufrufen.
//   - Prüfen: Eventname == "TileColor".

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using CommunityToolkit.Mvvm.Input;
using MVVM_22_CTWpfCap.ViewModels.Interfaces;
using MVVM_22_CTWpfCap.Model;

namespace MVVM_22_CTWpfCap.ViewModels.Tests;

[TestClass]
public class RowDataTests
{
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(4)]
    public void TileColor_Returns_ModelValues(int rowId)
    {
        // Arrange
        var model = Substitute.For<IWpfCapModel>();
        model.TileColor(Arg.Any<int>(), Arg.Any<int>()).Returns(ci =>
        {
            var x = (int)ci[0];
            var y = (int)ci[1];
            return y * 10 + x;
        });

        var vm = Substitute.For<IWpfCapViewModel>();
        vm.Model.Returns(model);

        var sut = new RowData { RowId = rowId, Parent = vm };

        // Act
        var result = sut.TileColor;

        // Assert
        Assert.AreEqual(4, result.Length);
        var y = rowId - 1;
        CollectionAssert.AreEqual(new[] { y * 10 + 0, y * 10 + 1, y * 10 + 2, y * 10 + 3 }, result);

        model.Received(1).TileColor(0, y);
        model.Received(1).TileColor(1, y);
        model.Received(1).TileColor(2, y);
        model.Received(1).TileColor(3, y);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(5)]
    public void TileColor_WithNullParent_ReturnsZeros(int rowId)
    {
        // Arrange
        var sut = new RowData { RowId = rowId, Parent = null };

        // Act
        var result = sut.TileColor;

        // Assert
        CollectionAssert.AreEqual(new[] { 0, 0, 0, 0 }, result);
    }

    [TestMethod]
    public void MoveLeftCommand_Returns_ParentCommand()
    {
        // Arrange
        var vm = Substitute.For<IWpfCapViewModel>();
        var cmd = Substitute.For<IRelayCommand<object>>();
        vm.MoveLeftCommand.Returns(cmd);

        var sut = new RowData { RowId = 1, Parent = vm };

        // Act
        var result = sut.MoveLeftCommand;

        // Assert
        Assert.AreSame(cmd, result);
    }

    [TestMethod]
    public void MoveRightCommand_Returns_ParentCommand()
    {
        // Arrange
        var vm = Substitute.For<IWpfCapViewModel>();
        var cmd = Substitute.For<IRelayCommand<object>>();
        vm.MoveRightCommand.Returns(cmd);

        var sut = new RowData { RowId = 1, Parent = vm };

        // Act
        var result = sut.MoveRightCommand;

        // Assert
        Assert.AreSame(cmd, result);
    }

    [TestMethod]
    public void MoveLeftCommand_WithNullParent_ReturnsNull()
    {
        // Arrange
        var sut = new RowData { RowId = 1, Parent = null };

        // Act
        var result = sut.MoveLeftCommand;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void MoveRightCommand_WithNullParent_ReturnsNull()
    {
        // Arrange
        var sut = new RowData { RowId = 1, Parent = null };

        // Act
        var result = sut.MoveRightCommand;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void This_Returns_Self()
    {
        // Arrange
        var sut = new RowData { RowId = 1, Parent = null };

        // Act
        var self = sut.This;

        // Assert
        Assert.AreSame(sut, self);
    }

    [TestMethod]
    public void Length_Is_4()
    {
        // Arrange
        var sut = new RowData { RowId = 1, Parent = null };

        // Act
        var len = sut.Length;

        // Assert
        Assert.AreEqual(4, len);
    }

    [TestMethod]
    public void OnPropertyChanged_Raises_PropertyChanged_Event()
    {
        // Arrange
        var sut = new RowData { RowId = 1, Parent = null };
        string? raisedName = null;
        sut.PropertyChanged += (s, e) => raisedName = e.PropertyName;

        // Act
        ((IRowData)sut).OnPropertyChanged("TileColor");

        // Assert
        Assert.AreEqual("TileColor", raisedName);
    }
}
