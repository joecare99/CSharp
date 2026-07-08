using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services;
using Gen_FreeWin.ViewModels;
using Gen_FreeWin.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GenFreeWinTests;

/// <summary>
/// Unit-Tests für FehlerliViewModelFactory (MVVM-Binding Setup).
/// Testet ViewModel-Initialisierung und WinForms-BindingSource-Konfiguration.
/// </summary>
[TestClass]
public class FehlerliBindingFactoryTests
{
    private IFehlerlistenService _mockService;

    [TestInitialize]
    public void Setup()
    {
        _mockService = Substitute.For<IFehlerlistenService>();
    }

    /// <summary>
    /// TEST 1: CreateAndBindViewModel() wirft ArgumentNullException bei null-View.
    /// </summary>
    [TestMethod]
    public void CreateAndBindViewModel_WithNullView_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        try
        {
            FehlerliViewModelFactory.CreateAndBindViewModel(null);
            Assert.Fail("Sollte ArgumentNullException werfen");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    /// <summary>
    /// TEST 2: CreateAndBindViewModel() gibt gültig initialisiertes ViewModel zurück.
    /// </summary>
    [TestMethod]
    public void CreateAndBindViewModel_WithValidView_ReturnsInitializedViewModel()
    {
        // Arrange
        var mockView = Substitute.For<Fehlerli>();
        mockView.List1 = new ListBox { Name = "List1" };
        mockView.List2 = new ListBox { Name = "List2" };

        try
        {
            // Act
            var viewModel = FehlerliViewModelFactory.CreateAndBindViewModel(mockView);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.PersonenOhneElternList);
            Assert.IsNotNull(viewModel.OerterErrorsList);
            Assert.AreEqual("DisplayText", viewModel.SortFieldName);
        }
        finally
        {
            // Cleanup
            mockView.List1?.Dispose();
            mockView.List2?.Dispose();
        }
    }

    /// <summary>
    /// TEST 3: SetupDataBindings() konfiguriert BindingSource korrekt für ListBoxes.
    /// </summary>
    [TestMethod]
    public void SetupDataBindings_ConfiguresListBoxDataSource_WithBindingSource()
    {
        // Arrange
        var mockView = Substitute.For<Fehlerli>();
        var list1 = new ListBox { Name = "List1" };
        var list2 = new ListBox { Name = "List2" };
        mockView.List1 = list1;
        mockView.List2 = list2;

        var mockService = Substitute.For<IFehlerlistenService>();
        var viewModel = new FehlerliViewModel(mockService);

        try
        {
            // Act
            FehlerliViewModelFactory.SetupDataBindings(mockView, viewModel);

            // Assert
            // List1 sollte ein DataSource haben (BindingSource)
            Assert.IsNotNull(list1.DataSource);

            // List2 sollte ein DataSource haben (BindingSource)
            Assert.IsNotNull(list2.DataSource);

            // DisplayMember sollte auf "DisplayText" gesetzt sein
            // Hinweis: ListBox.DisplayMember ist nur direkt einsehbar wenn über DataSource
            Assert.AreEqual("DisplayText", list1.DisplayMember);
            Assert.AreEqual("DisplayText", list2.DisplayMember);

            // ValueMember sollte auf "Id" gesetzt sein
            Assert.AreEqual("Id", list1.ValueMember);
            Assert.AreEqual("Id", list2.ValueMember);
        }
        finally
        {
            // Cleanup
            list1?.Dispose();
            list2?.Dispose();
        }
    }

    /// <summary>
    /// TEST 4: SetupDataBindings() mit Null-Argumenten crasht nicht.
    /// </summary>
    [TestMethod]
    public void SetupDataBindings_WithNullArguments_DoesNotThrow()
    {
        // Arrange & Act
        try
        {
            FehlerliViewModelFactory.SetupDataBindings(null, null);
            // Sollte graceful exit sein
        }
        catch (Exception ex)
        {
            Assert.Fail($"SetupDataBindings sollte null-Arguments graceful handhaben, erhielt: {ex.Message}");
        }
    }

    /// <summary>
    /// TEST 5: BindingSource synchronisiert ObservableCollection ↔ ListBox.
    /// </summary>
    [TestMethod]
    public void BindingSource_SynchronizesObservableCollection_WithListBox()
    {
        // Arrange
        var collection = new ObservableCollection<ErrorListItem>
        {
            new ErrorListItem { Id = 1, DisplayText = "Item 1", AdditionalData = new() },
            new ErrorListItem { Id = 2, DisplayText = "Item 2", AdditionalData = new() }
        };

        var bindingSource = new BindingSource { DataSource = collection };
        var listBox = new ListBox
        {
            DataSource = bindingSource,
            DisplayMember = "DisplayText",
            ValueMember = "Id"
        };

        try
        {
            // Assert - Items in ListBox sollten von Collection stammen
            Assert.AreEqual(2, bindingSource.Count);

            // Item hinzufügen
            collection.Add(new ErrorListItem { Id = 3, DisplayText = "Item 3", AdditionalData = new() });

            // BindingSource sollte aktualisiert sein
            Assert.AreEqual(3, bindingSource.Count, "BindingSource sollte neue Collection-Items erkennen");
        }
        finally
        {
            listBox?.Dispose();
        }
    }
}
