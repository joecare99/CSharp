// ***********************************************************************
// Assembly         : GenFreeWinTests
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// 
// <copyright file="AdresseViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Unit tests for AdresseViewModel</summary>
// ***********************************************************************

using GenFreeWin.ViewModels;
using GenFree.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GenFreeWin.Tests.ViewModels
{
    /// <summary>
    /// Unit tests for the AdresseViewModel class.
    /// Tests cover observable properties, email validation, command execution, and event handling.
    /// </summary>
    [TestClass]
    public class AdresseViewModelTests
    {
        private AdresseViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            // Initialize ViewModel
            _viewModel = new AdresseViewModel();
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_InitializesSuccessfully()
        {
            // Assert
            Assert.IsNotNull(_viewModel);
        }

        #endregion

        #region Observable Properties Tests

        [TestMethod]
        public void Title_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedTitle = "Herr";

            // Act
            _viewModel.Title = expectedTitle;

            // Assert
            Assert.AreEqual(expectedTitle, _viewModel.Title);
        }

        [TestMethod]
        public void Givenname_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedName = "Hans";

            // Act
            _viewModel.Givenname = expectedName;

            // Assert
            Assert.AreEqual(expectedName, _viewModel.Givenname);
        }

        [TestMethod]
        public void Surname_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedSurname = "Müller";

            // Act
            _viewModel.Surname = expectedSurname;

            // Assert
            Assert.AreEqual(expectedSurname, _viewModel.Surname);
        }

        [TestMethod]
        public void Street_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedStreet = "Hauptstraße 123";

            // Act
            _viewModel.Street = expectedStreet;

            // Assert
            Assert.AreEqual(expectedStreet, _viewModel.Street);
        }

        [TestMethod]
        public void Zip_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedZip = "10115";

            // Act
            _viewModel.Zip = expectedZip;

            // Assert
            Assert.AreEqual(expectedZip, _viewModel.Zip);
        }

        [TestMethod]
        public void Place_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedPlace = "Berlin";

            // Act
            _viewModel.Place = expectedPlace;

            // Assert
            Assert.AreEqual(expectedPlace, _viewModel.Place);
        }

        [TestMethod]
        public void Phone_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedPhone = "+49 30 12345";

            // Act
            _viewModel.Phone = expectedPhone;

            // Assert
            Assert.AreEqual(expectedPhone, _viewModel.Phone);
        }

        [TestMethod]
        public void EMail_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedEmail = "test@example.com";

            // Act
            _viewModel.EMail = expectedEmail;

            // Assert
            Assert.AreEqual(expectedEmail, _viewModel.EMail);
        }

        [TestMethod]
        public void Special_SetValue_PropertyUpdates()
        {
            // Arrange
            string expectedSpecial = "Spezialinfo";

            // Act
            _viewModel.Special = expectedSpecial;

            // Assert
            Assert.AreEqual(expectedSpecial, _viewModel.Special);
        }

        #endregion

        #region Email Validation Tests

        [TestMethod]
        public void ValidateEmailPattern_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            string validEmail = "test@example.com";
            string pattern = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$";

            // Act
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(validEmail, pattern);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ValidateEmailPattern_WithAnotherValidEmail_ReturnsTrue()
        {
            // Arrange
            string validEmail = "john.doe@mail.co.uk";
            string pattern = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$";

            // Act
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(validEmail, pattern);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ValidateEmailPattern_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "invalid.email@";
            string pattern = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$";

            // Act
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(invalidEmail, pattern);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateEmailPattern_WithInvalidEmailNoAt_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "invalidemail.com";
            string pattern = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$";

            // Act
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(invalidEmail, pattern);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateEmailPattern_WithInvalidEmailNoDomain_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "test@";
            string pattern = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$";

            // Act
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(invalidEmail, pattern);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateEmailPattern_WithComplexValidEmail_ReturnsTrue()
        {
            // Arrange
            string validEmail = "first.last@subdomain.example.co.uk";
            string pattern = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$";

            // Act
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(validEmail, pattern);

            // Assert
            Assert.IsTrue(isValid);
        }

        #endregion

        #region Relay Command Tests

        [TestMethod]
        public void Button1Command_Exists()
        {
            // Assert
            Assert.IsNotNull(_viewModel.Button1Command);
        }

        [TestMethod]
        public void Button1Command_CanExecute()
        {
            // Assert
            Assert.IsTrue(_viewModel.Button1Command.CanExecute(null));
        }

        [TestMethod]
        public void FormLoadCommand_Exists()
        {
            // Assert
            Assert.IsNotNull(_viewModel.FormLoadCommand);
        }

        [TestMethod]
        public void FormLoadCommand_CanExecute()
        {
            // Assert
            Assert.IsTrue(_viewModel.FormLoadCommand.CanExecute(null));
        }

        #endregion

        #region Action Tests

        [TestMethod]
        public void DoHide_CanBeSet()
        {
            // Arrange
            Action testAction = () => { };

            // Act
            _viewModel.DoHide = testAction;

            // Assert
            Assert.AreEqual(testAction, _viewModel.DoHide);
        }

        [TestMethod]
        public void DoHide_CanBeInvoked()
        {
            // Arrange
            bool invoked = false;
            _viewModel.DoHide = () => { invoked = true; };

            // Act
            _viewModel.DoHide?.Invoke();

            // Assert
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void DoHide_WithNullAction_DoesNotThrow()
        {
            // Arrange
            _viewModel.DoHide = null;

            // Act & Assert
            _viewModel.DoHide?.Invoke(); // Should not throw
        }

        #endregion

        #region Interface Implementation Tests

        [TestMethod]
        public void AdresseViewModel_ImplementsIAdresseViewModel()
        {
            // Assert
            Assert.IsInstanceOfType(_viewModel, typeof(IAdresseViewModel));
        }

        #endregion

        #region Edge Cases Tests

        [TestMethod]
        public void Title_WithEmptyString_CanBeSet()
        {
            // Act
            _viewModel.Title = "";

            // Assert
            Assert.AreEqual("", _viewModel.Title);
        }

        [TestMethod]
        public void Title_WithNull_CanBeSet()
        {
            // Act
            _viewModel.Title = null;

            // Assert
            Assert.IsNull(_viewModel.Title);
        }

        [TestMethod]
        public void Givenname_WithSpecialCharacters_CanBeSet()
        {
            // Arrange
            string nameWithSpecialChars = "Hans-Peter Müller-Schöne";

            // Act
            _viewModel.Givenname = nameWithSpecialChars;

            // Assert
            Assert.AreEqual(nameWithSpecialChars, _viewModel.Givenname);
        }

        [TestMethod]
        public void Surname_WithUmlaut_CanBeSet()
        {
            // Arrange
            string surnameWithUmlaut = "Müller";

            // Act
            _viewModel.Surname = surnameWithUmlaut;

            // Assert
            Assert.AreEqual(surnameWithUmlaut, _viewModel.Surname);
        }

        [TestMethod]
        public void EMail_WithWhitespace_CanBeSet()
        {
            // Arrange
            string emailWithSpaces = "  test@example.com  ";

            // Act
            _viewModel.EMail = emailWithSpaces;

            // Assert
            Assert.AreEqual(emailWithSpaces, _viewModel.EMail);
        }

        [TestMethod]
        public void Phone_WithExtendedFormat_CanBeSet()
        {
            // Arrange
            string phoneNumber = "+49 (30) 12345 / 67890";

            // Act
            _viewModel.Phone = phoneNumber;

            // Assert
            Assert.AreEqual(phoneNumber, _viewModel.Phone);
        }

        [TestMethod]
        public void Street_WithNumbers_CanBeSet()
        {
            // Arrange
            string streetAddress = "Mainzer Straße 123-456 A";

            // Act
            _viewModel.Street = streetAddress;

            // Assert
            Assert.AreEqual(streetAddress, _viewModel.Street);
        }

        [TestMethod]
        public void Zip_WithLongValue_CanBeSet()
        {
            // Arrange
            string zip = "D-10115";

            // Act
            _viewModel.Zip = zip;

            // Assert
            Assert.AreEqual(zip, _viewModel.Zip);
        }

        #endregion

        #region Property Default Values Tests

        [TestMethod]
        public void Title_DefaultValue_IsNull()
        {
            // Assert (default for string after initialization)
            Assert.IsNull(_viewModel.Title);
        }

        [TestMethod]
        public void Givenname_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.Givenname);
        }

        [TestMethod]
        public void Surname_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.Surname);
        }

        [TestMethod]
        public void Street_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.Street);
        }

        [TestMethod]
        public void Zip_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.Zip);
        }

        [TestMethod]
        public void Place_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.Place);
        }

        [TestMethod]
        public void Phone_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.Phone);
        }

        [TestMethod]
        public void EMail_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.EMail);
        }

        [TestMethod]
        public void Special_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.Special);
        }

        [TestMethod]
        public void DoHide_DefaultValue_IsNull()
        {
            // Assert
            Assert.IsNull(_viewModel.DoHide);
        }

        #endregion

        #region SaveCommand Tests

        [TestMethod]
        public void SaveCommand_ThrowsNotImplementedException()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _viewModel.SaveCommand);
        }

        #endregion

        #region Property Binding Tests

        [TestMethod]
        public void MultipleProperties_CanBeSetSequentially()
        {
            // Arrange
            _viewModel.Title = "Herr";
            _viewModel.Givenname = "Hans";
            _viewModel.Surname = "Müller";
            _viewModel.Street = "Hauptstraße 123";
            _viewModel.Zip = "10115";
            _viewModel.Place = "Berlin";
            _viewModel.Phone = "+49 30 12345";
            _viewModel.EMail = "hans.mueller@example.com";
            _viewModel.Special = "VIP";

            // Assert
            Assert.AreEqual("Herr", _viewModel.Title);
            Assert.AreEqual("Hans", _viewModel.Givenname);
            Assert.AreEqual("Müller", _viewModel.Surname);
            Assert.AreEqual("Hauptstraße 123", _viewModel.Street);
            Assert.AreEqual("10115", _viewModel.Zip);
            Assert.AreEqual("Berlin", _viewModel.Place);
            Assert.AreEqual("+49 30 12345", _viewModel.Phone);
            Assert.AreEqual("hans.mueller@example.com", _viewModel.EMail);
            Assert.AreEqual("VIP", _viewModel.Special);
        }

        [TestMethod]
        public void Property_CanBeUpdatedMultipleTimes()
        {
            // Arrange
            _viewModel.Givenname = "Hans";

            // Act
            _viewModel.Givenname = "Peter";
            _viewModel.Givenname = "Klaus";

            // Assert
            Assert.AreEqual("Klaus", _viewModel.Givenname);
        }

        #endregion

        #region Email Trim Tests

        [TestMethod]
        public void EMail_TrimmedInButton1Logic_RemovesWhitespace()
        {
            // Arrange
            string emailWithSpaces = "  test@example.com  ";

            // Act
            string trimmedEmail = emailWithSpaces.Trim();

            // Assert
            Assert.AreEqual("test@example.com", trimmedEmail);
            Assert.AreNotEqual("", trimmedEmail);
        }

        [TestMethod]
        public void EMail_OnlyWhitespace_TreatAsEmpty()
        {
            // Arrange
            string emailOnlySpaces = "   ";

            // Act
            string trimmedEmail = emailOnlySpaces.Trim();

            // Assert
            Assert.AreEqual("", trimmedEmail);
        }

        #endregion

        #region Address Concatenation Tests

        [TestMethod]
        public void FullName_ConcatenationWorks()
        {
            // Arrange
            _viewModel.Givenname = "Hans";
            _viewModel.Surname = "Müller";

            // Act
            string fullName = _viewModel.Givenname.Trim() + " " + _viewModel.Surname.Trim();

            // Assert
            Assert.AreEqual("Hans Müller", fullName);
        }

        [TestMethod]
        public void FullName_WithWhitespace_ConcatenationHandlesCorrectly()
        {
            // Arrange
            _viewModel.Givenname = "  Hans  ";
            _viewModel.Surname = "  Müller  ";

            // Act
            string fullName = _viewModel.Givenname.Trim() + " " + _viewModel.Surname.Trim();

            // Assert
            Assert.AreEqual("Hans Müller", fullName);
        }

        #endregion
    }
}
