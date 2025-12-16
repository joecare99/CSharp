// Pseudocode-Plan:
// - Setup: Verwende MSTest + NSubstitute.
// - Test TileColor:
//   - Mit Parent + Model: Model.TileColor so konfigurieren, dass es aus (x,y) -> y*10 + x berechnet.
//   - ColData mit ColId und Parent erstellen.
//   - Erwartete Werte berechnen (y = ColId-1, Werte: y*10 + 0..3).
//   - Prüfen: Länge = 4, Werte korrekt, und TileColor-Aufrufe 4x mit korrekten Parametern.
//   - DataRow für mehrere ColIds.
// - Test TileColor ohne Parent:
//   - Parent = null, ColId variieren.
//   - Prüfen: Array {0,0,0,0}.
// - Test MoveLeftCommand/MoveRightCommand:
//   - Parent-Mocks für MoveLeftCommand/MoveRightCommand bereitstellen.
//   - Prüfen: Weitergabe identisch; Ohne Parent: null.
// - Test This:
//   - Prüfen: ReferenceEquals(This, instance) == true.
// - Test Length:
//   - Prüfen: 4.
// - Test OnPropertyChanged (explizites Interface):
//   - Auf PropertyChanged subscriben, ((IColData)sut).OnPropertyChanged("TileColor") aufrufen.
//   - Prüfen: Eventname == "TileColor".

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM_22_CTWpfCap.ViewModels;
using MVVM_22_CTWpfCap.ViewModels.Interfaces;
using MVVM_22_CTWpfCap.Model;

namespace MVVM_22_CTWpfCap.ViewModels.Tests;

[TestClass]
public class ColDataTests
{
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(4)]
    public void TileColor_Returns_ModelValues(int colId)
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

        var sut = new ColData { ColId = colId, Parent = vm };

        // Act

        // Assert
        Assert.AreEqual(4, sut.Length);
        var y = colId - 1;
        Assert.AreEqual(y * 10 + 0, sut[0]);
        Assert.AreEqual( y * 10 + 1, sut[1]);
        Assert.AreEqual(y * 10 + 2, sut[2]);
        Assert.AreEqual(y * 10 + 3, sut[3]);

        model.Received(1).TileColor(0, y);
        model.Received(1).TileColor(1, y);
        model.Received(1).TileColor(2, y);
        model.Received(1).TileColor(3, y);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(5)]
    public void TileColor_WithNullParent_ReturnsZeros(int colId)
    {
        // Arrange
        var sut = new ColData { ColId = colId, Parent = null };

        // Act

        // Assert
        Assert.AreEqual(0, sut[0]);
        Assert.AreEqual(0, sut[1]);
        Assert.AreEqual(0, sut[2]);
    }

    [TestMethod]
    public void MoveUpCommand_Returns_ParentCommand()
    {
        // Arrange
        var vm = Substitute.For<IWpfCapViewModel>();
        var cmd = Substitute.For<IRelayCommand<object>>();
        vm.MoveUpCommand.Returns(cmd);

        var sut = new ColData { ColId = 1, Parent = vm };

        // Act
        var result = sut.MoveUpCommand;

        // Assert
        Assert.AreSame(cmd, result);
    }

    [TestMethod]
    public void MoveDownCommand_Returns_ParentCommand()
    {
        // Arrange
        var vm = Substitute.For<IWpfCapViewModel>();
        var cmd = Substitute.For<IRelayCommand<object>>();
        vm.MoveDownCommand.Returns(cmd);

        var sut = new ColData { ColId = 1, Parent = vm };

        // Act
        var result = sut.MoveDownCommand;

        // Assert
        Assert.AreSame(cmd, result);
    }

    [TestMethod]
    public void MoveUpCommand_WithNullParent_ReturnsNull()
    {
        // Arrange
        var sut = new ColData { ColId = 1, Parent = null };

        // Act
        var result = sut.MoveUpCommand;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void MoveDownCommand_WithNullParent_ReturnsNull()
    {
        // Arrange
        var sut = new ColData { ColId = 1, Parent = null };

        // Act
        var result = sut.MoveDownCommand;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void This_Returns_Self()
    {
        // Arrange
        var sut = new ColData { ColId = 1, Parent = null };

        // Act
        var self = sut.This;

        // Assert
        Assert.AreSame(sut, self);
    }

    [TestMethod]
    public void Length_Is_4()
    {
        // Arrange
        var sut = new ColData { ColId = 1, Parent = null };

        // Act
        var len = sut.Length;

        // Assert
        Assert.AreEqual(4, len);
    }

}