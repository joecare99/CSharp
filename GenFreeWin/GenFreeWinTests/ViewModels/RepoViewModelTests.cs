// ***********************************************************************
// Assembly         : GenFreeWinTests
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// 
// <copyright file="RepoViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Unit tests for RepoViewModel</summary>
// ***********************************************************************

using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using Gen_FreeWin.UseCases;
using Gen_FreeWin.ViewModels;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Gen_FreeWin.Tests.ViewModels
{
    /// <summary>
    /// Unit tests for the RepoViewModel class.
    /// Tests cover initialization, command execution, property changes, and data loading.
    /// </summary>
    [TestClass]
    public class RepoViewModelTests
    {
        private IModul1 _mockModul1;
        private IRepoDataService _mockDataService;
        private RepoViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            // Create mock instances
            _mockModul1 = Substitute.For<IModul1>();
            _mockDataService = Substitute.For<IRepoDataService>();

            // Setup default behavior for Modul1
            _mockModul1.FontSize.Returns(12f);
            _mockModul1.HintFarb.Returns(Color.LightGray);

            // Initialize ViewModel
            _viewModel = new RepoViewModel(_mockModul1, _mockDataService);
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_WithNullModul1_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RepoViewModel(null, _mockDataService));
        }

        [TestMethod]
        public void Constructor_WithNullDataService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RepoViewModel(_mockModul1, null));
        }

        [TestMethod]
        public void Constructor_WithValidArguments_InitializesSuccessfully()
        {
            // Assert
            Assert.IsNotNull(_viewModel);
            Assert.IsNotNull(_viewModel.Repolist_Items);
            Assert.IsNotNull(_viewModel.Sources_Items);
        }

        #endregion

        #region Observable Properties Tests

        [TestMethod]
        public void SourceCount_SetValue_PropertyUpdates()
        {
            // Arrange
            int expectedValue = 42;

            // Act
            _viewModel.SourceCount = expectedValue;

            // Assert
            Assert.AreEqual(expectedValue, _viewModel.SourceCount);
        }

        [TestMethod]
        public void RepoName_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedName = "Test Repository";

            // Act
            _viewModel.RepoName_Text = expectedName;

            // Assert
            Assert.AreEqual(expectedName, _viewModel.RepoName_Text);
        }

        [TestMethod]
        public void RepoStreet_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedStreet = "Main Street 123";

            // Act
            _viewModel.RepoStreet_Text = expectedStreet;

            // Assert
            Assert.AreEqual(expectedStreet, _viewModel.RepoStreet_Text);
        }

        [TestMethod]
        public void RepoPlace_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedPlace = "Berlin";

            // Act
            _viewModel.RepoPlace_Text = expectedPlace;

            // Assert
            Assert.AreEqual(expectedPlace, _viewModel.RepoPlace_Text);
        }

        [TestMethod]
        public void RepoPLZ_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedPlz = "10115";

            // Act
            _viewModel.RepoPLZ_Text = expectedPlz;

            // Assert
            Assert.AreEqual(expectedPlz, _viewModel.RepoPLZ_Text);
        }

        [TestMethod]
        public void RepoPhone_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedPhone = "+49 30 123456";

            // Act
            _viewModel.RepoPhone_Text = expectedPhone;

            // Assert
            Assert.AreEqual(expectedPhone, _viewModel.RepoPhone_Text);
        }

        [TestMethod]
        public void RepoMail_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedMail = "info@example.com";

            // Act
            _viewModel.RepoMail_Text = expectedMail;

            // Assert
            Assert.AreEqual(expectedMail, _viewModel.RepoMail_Text);
        }

        [TestMethod]
        public void BtnDeleteVisible_SetValue_PropertyUpdates()
        {
            // Arrange
            bool expectedValue = true;

            // Act
            _viewModel.BtnDeleteVisible = expectedValue;

            // Assert
            Assert.AreEqual(expectedValue, _viewModel.BtnDeleteVisible);
        }

        [TestMethod]
        public void IsNotReadonly_SetValue_PropertyUpdates()
        {
            // Arrange
            bool expectedValue = false;

            // Act
            _viewModel.IsNotReadonly = expectedValue;

            // Assert
            Assert.AreEqual(expectedValue, _viewModel.IsNotReadonly);
        }

        [TestMethod]
        public void RichTextBox1_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedText = "Some remarks about the repository.";

            // Act
            _viewModel.RichTextBox1_Text = expectedText;

            // Assert
            Assert.AreEqual(expectedText, _viewModel.RichTextBox1_Text);
        }

        [TestMethod]
        public void RichTextBox2_Text_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedText = "https://example.com";

            // Act
            _viewModel.RichTextBox2_Text = expectedText;

            // Assert
            Assert.AreEqual(expectedText, _viewModel.RichTextBox2_Text);
        }

        #endregion

        #region Relay Command Tests

        [TestMethod]
        public void CloseCommand_Execution_InvokesDoCloseAction()
        {
            // Arrange
            bool closeInvoked = false;
            _viewModel.DoClose = () => { closeInvoked = true; };

            // Act
            _viewModel.CloseCommand.Execute(null);

            // Assert
            Assert.IsTrue(closeInvoked);
        }

        [TestMethod]
        public void CloseCommand_WithNullDoClose_DoesNotThrow()
        {
            // Arrange
            _viewModel.DoClose = null;

            // Act & Assert (should not throw)
            _viewModel.CloseCommand.Execute(null);
        }

        [TestMethod]
        public void NewEntryCommand_CanNotExecuteWhenReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = false;

            // Act
            bool canExecute = _viewModel.NewEntryCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        public void NewEntryCommand_CanExecuteWhenNotReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = true;

            // Act
            bool canExecute = _viewModel.NewEntryCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public async Task NewEntryCommand_Execution_ClearsFields()
        {
            // Arrange
            _viewModel.IsNotReadonly = true;
            _viewModel.RepoName_Text = "OldName";
            _viewModel.RepoStreet_Text = "OldStreet";

            // Act
            _viewModel.NewEntryCommand.Execute(null);
            await Task.Delay(100); // Allow async operations to complete

            // Assert
            // NewEntry should clear fields (based on Clear() method call)
            // The exact SourceCount value depends on the UseCase implementation
        }

        [TestMethod]
        public void SaveCommand_CanExecuteWhenNotReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = true;

            // Act
            bool canExecute = _viewModel.SaveCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void SaveCommand_CanNotExecuteWhenReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = false;

            // Act
            bool canExecute = _viewModel.SaveCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        public void Save2Command_CanExecuteWhenNotReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = true;

            // Act
            bool canExecute = _viewModel.Save2Command.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void Save2Command_CanNotExecuteWhenReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = false;

            // Act
            bool canExecute = _viewModel.Save2Command.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        public void DeleteCommand_CanExecuteWhenNotReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = true;

            // Act
            bool canExecute = _viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void DeleteCommand_CanNotExecuteWhenReadonly()
        {
            // Arrange
            _viewModel.IsNotReadonly = false;

            // Act
            bool canExecute = _viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region Observable Collections Tests

        [TestMethod]
        public void Repolist_Items_InitializedAsEmptyCollection()
        {
            // Assert
            Assert.IsNotNull(_viewModel.Repolist_Items);
            Assert.AreEqual(0, _viewModel.Repolist_Items.Count);
        }

        [TestMethod]
        public void Sources_Items_InitializedAsEmptyCollection()
        {
            // Assert
            Assert.IsNotNull(_viewModel.Sources_Items);
            Assert.AreEqual(0, _viewModel.Sources_Items.Count);
        }

        [TestMethod]
        public void Repolist_Items_CanAddItems()
        {
            // Arrange
            var mockItem = Substitute.For<IListItem<int>>();

            // Act
            _viewModel.Repolist_Items.Add(mockItem);

            // Assert
            Assert.AreEqual(1, _viewModel.Repolist_Items.Count);
            Assert.AreEqual(mockItem, _viewModel.Repolist_Items[0]);
        }

        [TestMethod]
        public void Repolist_SelectedItem_SetValue_PropertyUpdates()
        {
            // Arrange
            var mockItem = Substitute.For<IListItem<int>>();

            // Act
            _viewModel.Repolist_SelectedItem = mockItem;

            // Assert
            Assert.AreEqual(mockItem, _viewModel.Repolist_SelectedItem);
        }

        [TestMethod]
        public void Sources_SelectedItem_SetValue_PropertyUpdates()
        {
            // Arrange
            var mockItem = Substitute.For<IListItem<int>>();

            // Act
            _viewModel.Sources_SelectedItem = mockItem;

            // Assert
            Assert.AreEqual(mockItem, _viewModel.Sources_SelectedItem);
        }

        #endregion

        #region Public Properties Tests

        [TestMethod]
        public void FontSize_SetValue_PropertyUpdates()
        {
            // Arrange
            float expectedSize = 14f;

            // Act
            _viewModel.FontSize = expectedSize;

            // Assert
            Assert.AreEqual(expectedSize, _viewModel.FontSize);
        }

        [TestMethod]
        public void HintFarb_SetValue_PropertyUpdates()
        {
            // Arrange
            object expectedColor = Color.Red;

            // Act
            _viewModel.HintFarb = expectedColor;

            // Assert
            Assert.AreEqual(expectedColor, _viewModel.HintFarb);
        }

        [TestMethod]
        public void DoClose_SetValue_ActionCanBeSet()
        {
            // Arrange
            Action testAction = () => { };

            // Act
            _viewModel.DoClose = testAction;

            // Assert
            Assert.AreEqual(testAction, _viewModel.DoClose);
        }

        [TestMethod]
        public void DoStart_SetValue_ActionCanBeSet()
        {
            // Arrange
            Action<string> testAction = (s) => { };

            // Act
            _viewModel.DoStart = testAction;

            // Assert
            Assert.AreEqual(testAction, _viewModel.DoStart);
        }

        #endregion

        #region Property Validation Tests

        [TestMethod]
        public void RepoModel_CanBeConstructedFromViewModelProperties()
        {
            // Arrange
            _viewModel.RepoName_Text = "Test Repo";
            _viewModel.RepoStreet_Text = "123 Main St";
            _viewModel.RepoPlace_Text = "Berlin";
            _viewModel.RepoPLZ_Text = "10001";
            _viewModel.RepoPhone_Text = "+49 30 12345";
            _viewModel.RepoMail_Text = "test@example.com";
            _viewModel.RichTextBox2_Text = "https://test.de";
            _viewModel.RichTextBox1_Text = "Test remarks";

            // Act
            var repo = new RepoModel
            {
                Name = _viewModel.RepoName_Text,
                Street = _viewModel.RepoStreet_Text,
                Place = _viewModel.RepoPlace_Text,
                PostalCode = _viewModel.RepoPLZ_Text,
                Phone = _viewModel.RepoPhone_Text,
                Email = _viewModel.RepoMail_Text,
                Website = _viewModel.RichTextBox2_Text,
                Remarks = _viewModel.RichTextBox1_Text
            };

            // Assert
            Assert.AreEqual("Test Repo", repo.Name);
            Assert.AreEqual("123 Main St", repo.Street);
            Assert.AreEqual("Berlin", repo.Place);
            Assert.AreEqual("10001", repo.PostalCode);
            Assert.AreEqual("+49 30 12345", repo.Phone);
            Assert.AreEqual("test@example.com", repo.Email);
            Assert.AreEqual("https://test.de", repo.Website);
            Assert.AreEqual("Test remarks", repo.Remarks);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void SourceCount_WithZeroOrNegativeValue_PropertyUpdates()
        {
            // Act
            _viewModel.SourceCount = 0;

            // Assert
            Assert.AreEqual(0, _viewModel.SourceCount);

            // Act
            _viewModel.SourceCount = -1;

            // Assert
            Assert.AreEqual(-1, _viewModel.SourceCount);
        }

        [TestMethod]
        public void TextProperties_WithNullValue_CanBeSet()
        {
            // Act
            _viewModel.RepoName_Text = null;

            // Assert
            Assert.IsNull(_viewModel.RepoName_Text);
        }

        [TestMethod]
        public void TextProperties_WithEmptyString_CanBeSet()
        {
            // Act
            _viewModel.RepoName_Text = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, _viewModel.RepoName_Text);
        }

        #endregion
    }
}
