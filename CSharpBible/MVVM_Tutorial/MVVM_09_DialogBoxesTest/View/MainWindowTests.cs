﻿// ***********************************************************************
// Assembly         : MVVM_00a_CTTemplate_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="MainWindowTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using MVVM_09_DialogBoxes.ViewModels;
using System.ComponentModel;
using System.Threading;
using System.Windows;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_09_DialogBoxes.Views.Tests
{
    /// <summary>
    /// Defines test class MainWindowTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class MainWindowTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test view
        /// </summary>
        /// <autogeneratedoc />
        MainWindow testView;
        MainWindowViewModel vm;
        private MessageBoxResult mbResult;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            var t = new Thread(() => { 
                testView = new();
                vm = (MainWindowViewModel)testView.DataContext; 
//                testView.Window_Loaded(this,null!); 
            });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            vm.PropertyChanged += OnVMPropertyChanged;
            if (vm is INotifyPropertyChanging npchgn)
                npchgn.PropertyChanging += OnVMPropertyChanging;
        }

        /// <summary>
        /// Defines the test method MainWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void MainWindowTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(MainWindow));    
            Assert.IsNotNull(vm);
            Assert.IsInstanceOfType(vm, typeof(MainWindowViewModel));
        }
    }
}
