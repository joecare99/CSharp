﻿// ***********************************************************************
// Assembly         : MVVM_37_TreeView_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="TemplateViewTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using MVVM.ViewModel.Tests;
using MVVM_37_TreeView.Services;
using MVVM_37_TreeView.ViewModels;
using System;
using System.Threading;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_37_TreeView.Views.Tests
{
    /// <summary>
    /// Defines test class TemplateViewTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class BooksTreeViewTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test view
        /// </summary>
        /// <autogeneratedoc />
        BooksTreeView testView;
        private IDebugLog _debugLog;
        private IGetResult _getResult;
        private Func<Type, object?> _gsold;
        private Func<Type, object> _grsold;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            _gsold = IoC.GetSrv;
            _grsold = IoC.GetReqSrv;
            var sp = new ServiceCollection()
                    .AddSingleton<IDebugLog, DebugLog>()
                    .AddSingleton<IGetResult, GetResult>()
                    .AddTransient<IBooksService, BooksService>()
                    .AddTransient<BooksTreeViewModel>()
                    .BuildServiceProvider();
            IoC.GetReqSrv = t => sp.GetRequiredService(t);
            IoC.GetSrv = t => sp.GetService(t);
            _debugLog = IoC.GetRequiredService<IDebugLog>();
            _getResult = IoC.GetRequiredService<IGetResult>();
            var t = new Thread(() => testView = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (testView is IDisposable id)
            id.Dispose();
            IoC.GetReqSrv = _grsold;
            IoC.GetSrv = _gsold;
        }

        /// <summary>
        /// Defines the test method MainWindowTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void BooksTreeViewTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsInstanceOfType(testView, typeof(BooksTreeView));    
        }
    }
}
