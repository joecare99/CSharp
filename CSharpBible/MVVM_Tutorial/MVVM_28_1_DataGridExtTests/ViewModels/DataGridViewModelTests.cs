﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_28_1_DataGridExt.ViewModels.Tests
{
    [TestClass]
    public class DataGridViewModelTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        DataGridViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testModel = new DataGridViewModel();
        }

        [TestMethod]
        public void SetupTest() {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(DataGridViewModel));
        }
    }
}