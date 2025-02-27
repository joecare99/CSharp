﻿// ***********************************************************************
// Assembly         : Calc32WPFTests
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="MainWindowViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using Calc32.Models.Interfaces;
using Calc32WPF.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.ComponentModel;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace Calc32WPF.ViewModel.Tests
{
    /// <summary>
    /// Defines test class MainWindowViewModelTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class MainWindowViewModelTests
    {
        /// <summary>
        /// The property changed count
        /// </summary>
        /// <autogeneratedoc />
        private int _PropChangedCount;
        /// <summary>
        /// The property changed
        /// </summary>
        /// <autogeneratedoc />
        private string _PropChanged = "";
        /// <summary>
        /// The model view
        /// </summary>
        /// <autogeneratedoc />
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private MainWindowViewModel _ModelView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Tests the initialize.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void TestInitialize()
        {
            IServiceProvider Sp;
            Ioc.Default.ConfigureServices(Sp= Substitute.For<IServiceProvider>());
            Sp.GetService(typeof(ICalculatorClass)).Returns(Substitute.For<ICalculatorClass>());
            ClearResults();
            _ModelView = new MainWindowViewModel();
            _ModelView.PropertyChanged += MainWindow_VM_PropChanged;
        }

        private void ClearResults()
        {
            _PropChangedCount = 0;
            _PropChanged = "";
        }

        /// <summary>
        /// Handles the PropChanged event of the MainWindow_VM control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        private void MainWindow_VM_PropChanged(object? sender, PropertyChangedEventArgs e)
        {
            _PropChangedCount++;
            _PropChanged += $"{e.PropertyName}:{sender?.GetType()?.GetProperty(e.PropertyName!)?.GetValue(sender)}\r\n";
        }

        /// <summary>
        /// Defines the test method MainWindowViewModelTestSetup.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void MainWindowViewModelTestSetup()
        {
            Assert.IsNotNull(_ModelView);
            Assert.AreEqual(0, _PropChangedCount);
        }

        /// <summary>
        /// Mains the window vm number button.
        /// </summary>
        /// <param name="sButtons">The s buttons.</param>
        /// <param name="iPCCount">The i pc count.</param>
        /// <param name="sAkk">The s akk.</param>
        /// <autogeneratedoc />
        [DataTestMethod()]
        [DataRow("0", 0, 0, "")]
        [DataRow("1", 3, 1, "Accumulator:1\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("2", 3, 2, "Accumulator:2\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("3", 3, 3, "Accumulator:3\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("4", 3, 4, "Accumulator:4\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("5", 3, 5, "Accumulator:5\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("6", 3, 6, "Accumulator:6\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("7", 3, 7, "Accumulator:7\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("8", 3, 8, "Accumulator:8\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("9", 3, 9, "Accumulator:9\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("00", 0, 0, "")]
        [DataRow("01", 3, 1, "Accumulator:1\r\nMemory:0\r\nOperationText:\r\n")]
        [DataRow("10", 6, 10, "Accumulator:1\r\nMemory:0\r\nOperationText:\r\nAkkumulator:10\r\nMemory:0\r\nOperationText:\r\n")]
        public void MainWindow_VM_NumButton(string sButtons, int iPCCount, int sAkk, string sExpPC)
        {
            foreach (var button in sButtons)
            {
                if (button >= '0' && button <= '9')
                    _ModelView.NumberCommand.Execute($"{button}");

            }
            Assert.AreEqual(iPCCount, _PropChangedCount);
            Assert.AreEqual(sAkk, _ModelView.Accumulator);
            Assert.AreEqual(sExpPC, _PropChanged);

        }

        [DataTestMethod()]
        [DataRow("0",100, 0, 100, "", new string[] { "" })]
        [DataRow("0", 0, 0, 0, "", new string[] { "" })]
        [DataRow("0", -1, 0, -1, "", new string[] { "" })]
        [DataRow("0", int.MaxValue, 0, int.MaxValue, "", new string[] { "" })]
        [DataRow("0", int.MinValue, 0, int.MinValue, "", new string[] { "" })]
        [DataRow("1", 100, 3, 100, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("1", 0, 3, 0, "=", new string[] { "Accumulator:0\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("1", -1, 3, -1, "=", new string[] { "Accumulator:-1\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("1", int.MaxValue, 3, int.MaxValue, "=", new string[] { "Accumulator:2147483647\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("1", int.MinValue, 3, int.MinValue, "=", new string[] { "Accumulator:-2147483648\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("2", 0, 3, 0, "+", new string[] { "Accumulator:0\r\nMemory:0\r\nOperationText:+\r\n" })]
        [DataRow("2", -1, 6, -1, "+", new string[] { "Accumulator:-1\r\nMemory:0\r\nOperationText:+\r\nAkkumulator:-1\r\nMemory:-1\r\nOperationText:+\r\n" })]
        [DataRow("2", 100, 6, 100, "+", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:+\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:+\r\n" })]
        [DataRow("21", 0, 6, 0, "=", new string[] { "Accumulator:0\r\nMemory:0\r\nOperationText:+\r\nAkkumulator:0\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("21", -1, 15, -2, "=", new string[] { "Accumulator:-1\r\nMemory:0\r\nOperationText:+\r\nAkkumulator:-1\r\nMemory:-1\r\nOperationText:+\r\nAkkumulator:-2\r\nMemory:-1\r\nOperationText:+\r\nAkkumulator:-2\r\nMemory:-1\r\nOperationText:=\r\nAkkumulator:-2\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("21", 100, 15, 200, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:+\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:+\r\nAkkumulator:200\r\nMemory:100\r\nOperationText:+\r\nAkkumulator:200\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:200\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("22", 0, 3, 0, "+", new string[] { "Accumulator:0\r\nMemory:0\r\nOperationText:+\r\n" })]
        [DataRow("22", -1, 12, -2, "+", new string[] { "Accumulator:-1\r\nMemory:0\r\nOperationText:+\r\nAkkumulator:-1\r\nMemory:-1\r\nOperationText:+\r\nAkkumulator:-2\r\nMemory:-1\r\nOperationText:+\r\nAkkumulator:-2\r\nMemory:-2\r\nOperationText:+\r\n" })]
        [DataRow("22", 100, 12, 200, "+", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:+\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:+\r\nAkkumulator:200\r\nMemory:100\r\nOperationText:+\r\nAkkumulator:200\r\nMemory:200\r\nOperationText:+\r\n" })]
        [DataRow("3", 100, 6, 100, "-", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:-\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:-\r\n" })]
        [DataRow("31", 100, 15, 0, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:-\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:-\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:-\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:0\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("311", 100, 15, 0, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:-\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:-\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:-\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:0\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("33", 100, 12, 0, "-", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:-\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:-\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:-\r\nAkkumulator:0\r\nMemory:0\r\nOperationText:-\r\n" })]
        [DataRow("4", 100, 6, 100, "*", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:*\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:*\r\n" })]
        [DataRow("41", 100, 15, 10000, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:*\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:*\r\nAkkumulator:10000\r\nMemory:100\r\nOperationText:*\r\nAkkumulator:10000\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:10000\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("44", 100, 12, 10000, "*", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:*\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:*\r\nAkkumulator:10000\r\nMemory:100\r\nOperationText:*\r\nAkkumulator:10000\r\nMemory:10000\r\nOperationText:*\r\n" })]
        [DataRow("5", 100, 6, 100, "/", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:/\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:/\r\n" })]
        [DataRow("51", 100, 15, 1, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:/\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:/\r\nAkkumulator:1\r\nMemory:100\r\nOperationText:/\r\nAkkumulator:1\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:1\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("55", 100, 12, 1, "/", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:/\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:/\r\nAkkumulator:1\r\nMemory:100\r\nOperationText:/\r\nAkkumulator:1\r\nMemory:1\r\nOperationText:/\r\n" })]
        [DataRow("6", 100, 6, 100, "&", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:&\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:&\r\n" })]
        [DataRow("61", 100, 12, 100, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:&\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:&\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:100\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("66", 100, 6, 100, "&", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:&\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:&\r\n" })]
        [DataRow("7", 100, 6, 100, "|", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:|\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:|\r\n" })]
        [DataRow("71", 100, 12, 100, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:|\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:|\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:100\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("77", 100, 6, 100, "|", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:|\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:|\r\n" })]
        [DataRow("8", 100, 6, 100, "x", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:x\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:x\r\n" })]
        [DataRow("81", 100, 15, 0, "=", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:x\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:x\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:x\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:=\r\nAkkumulator:0\r\nMemory:0\r\nOperationText:=\r\n" })]
        [DataRow("88", 100, 12, 0, "x", new string[] { "Accumulator:100\r\nMemory:0\r\nOperationText:x\r\nAkkumulator:100\r\nMemory:100\r\nOperationText:x\r\nAkkumulator:0\r\nMemory:100\r\nOperationText:x\r\nAkkumulator:0\r\nMemory:0\r\nOperationText:x\r\n" })]
        [DataRow("9", 100, 3, -101, "", new string[] { "Accumulator:-101\r\nMemory:0\r\nOperationText:\r\n" })]
        [DataRow("99", 100, 6, 100, "", new string[] { "Accumulator:-101\r\nMemory:0\r\nOperationText:\r\nAkkumulator:100\r\nMemory:0\r\nOperationText:\r\n" })]
        public void MainWindow_VM_OpButton(string sButtons, int iAcc,  int iPCCount, int iExpAkk,string sExpOp , string[] asExpPC)
        {
            _ModelView.Accumulator = iAcc;
            ClearResults();    
            foreach (var button in sButtons)
            {
                if (button >= '0' && button <= '9')
                    _ModelView.OperationCommand.Execute($"-{button}");

            }
            Assert.AreEqual(iPCCount, _PropChangedCount,$"Test: {sButtons}.PCCount");
            Assert.AreEqual(iExpAkk, _ModelView.Accumulator, $"Test: {sButtons}.Acc");
            Assert.AreEqual(sExpOp, _ModelView.OperationText, $"Test: {sButtons}.Op");
            Assert.AreEqual(asExpPC[0], _PropChanged, $"Test: {sButtons}.PropChanges");
        }

        [DataTestMethod]
        [DataRow("",false,0,0,new string[] {"" })]
        [DataRow("1", true, 3, 0, new string[] { "Accumulator:0\r\nMemory:0\r\nOperationText:\r\n" })]
        [DataRow("10", true, 3, 1, new string[] { "Accumulator:1\r\nMemory:0\r\nOperationText:\r\n" })]
        public void MainWindow_VM_BackButtonTest(string sButtons,bool xEnab, int iPCCount, int sAkk, string[] asExpPC)
        {
            foreach (var button in sButtons)
            {
                if (button >= '0' && button <= '9')
                    _ModelView.NumberCommand.Execute($"{button}");
            }
            ClearResults();
            Assert.AreEqual(xEnab,_ModelView.BackSpaceCommand.CanExecute(null));
            if (xEnab)
                _ModelView.BackSpaceCommand.Execute(null);
            Assert.AreEqual(iPCCount, _PropChangedCount);
            Assert.AreEqual(sAkk, _ModelView.Accumulator);
            Assert.AreEqual(asExpPC[0], _PropChanged);

        }
    }
}