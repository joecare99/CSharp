using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.UI;
using NSubstitute;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class ViewHelperTests
    {
     #pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        IApplUserTexts iText;
     #pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        [TestInitialize]
        public void Init()
        {
            iText = Substitute.For<IApplUserTexts>();
            iText[Arg.Any<string>()].Returns(callInfo => callInfo.Arg<string>());
        }

        [DataTestMethod]
        [DataRow("Speichern", "Save", true)]
        [DataRow("Löschen", "Delete", false)]
        [DataRow("Bearbeiten", "Edit", true)]
        public void SetCommandBtnTest(string buttonText, string commandName, bool expectedEnabled)
        {
            // Arrange
            var button = new System.Windows.Forms.Button
            {
                Text = buttonText
            };

            // Act
            ViewHelper.SetCommandBtn(button, expectedEnabled, commandName, iText);

            // Assert
            Assert.AreEqual(commandName, button.Text);
            Assert.AreEqual(expectedEnabled, button.Visible);
        }
    }
}