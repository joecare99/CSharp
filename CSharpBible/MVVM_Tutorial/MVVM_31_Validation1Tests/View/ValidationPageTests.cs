using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_31_Validation1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVVM_31_Validation1.View.Tests
{
    [TestClass()]
    public class ValidationPageTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        ValidationPage testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize()]
        public void Init()
        {
            Thread thread = new(() =>
            {
                testView = new();
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
        }

        [TestMethod()]
        public void ValidationPageTest()
        {
            Assert.IsNotNull(testView);
        }
    }
}