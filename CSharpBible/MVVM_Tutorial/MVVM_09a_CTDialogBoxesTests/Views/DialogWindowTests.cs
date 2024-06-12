﻿// ***********************************************************************
// Assembly         : MVVM_00a_CTTemplate_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="DialogWindowTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using MVVM_09a_CTDialogBoxes.ViewModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_09a_CTDialogBoxes.Views.Tests
{
    /// <summary>
    /// Defines test class DialogWindowTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class DialogWindowTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test view
        /// </summary>
        /// <autogeneratedoc />
        DialogWindow testView;
        DialogWindowViewModel vm;
        private MessageBoxResult mbResult;
        private bool? xResult;
        private string sNewName = "NewName";
        private string sNewEmail = "NewEmail";
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            var t = new Thread(() => { testView = new(); vm = (DialogWindowViewModel)testView.DataContext; testView.Show(); });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            vm.PropertyChanged += OnVMPropertyChanged;
            if (vm is INotifyPropertyChanging npchgn)
                npchgn.PropertyChanging += OnVMPropertyChanging;
        }

        /// <summary>
        /// Defines the test method DialogWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void DialogWindowTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(DialogWindow));    
            Assert.IsNotNull(vm);
            Assert.IsInstanceOfType(vm, typeof(DialogWindowViewModel));
        }

        [DataTestMethod]
        public void DoCancelTest()
        {
            var t = new Thread(() => { testView = new(); vm = (DialogWindowViewModel)testView.DataContext; testView.Show(); vm.CancelCommand.Execute(null); });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsFalse(testView.IsVisible);
        }


        [TestMethod]
        public void DoOKTest()
        {
            bool? xRes = null;
            bool xVisible = false;
            var t = new Thread(() =>
            {
                testView = new();
                vm = (DialogWindowViewModel)testView.DataContext;
                ExecOK();
                xRes = testView.ShowDialog();
                // xResult = testView.ShowDialog();
                xVisible = testView.IsVisible;

                async void ExecOK()
                {
                    await System.Threading.Tasks.Task.Delay(50);
                    testView.Dispatcher.Invoke(() => vm.OKCommand.Execute(null));
                }

            });
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsFalse(xVisible);
            Assert.IsTrue(xRes.HasValue);
            Assert.IsTrue(xRes.Value);
        }

    }
}
