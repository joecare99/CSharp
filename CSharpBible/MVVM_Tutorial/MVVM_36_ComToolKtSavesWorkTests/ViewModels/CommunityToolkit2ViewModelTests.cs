﻿// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="CommunityToolkit2ViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MVVM_36_ComToolKtSavesWork.Models;
using System;
using MVVM.View.Extension;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_36_ComToolKtSavesWork.ViewModels.Tests
{
    /// <summary>
    /// Defines test class CommunityToolkit2ViewModelTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class CommunityToolkit2ViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test model
        /// </summary>
        /// <autogeneratedoc />
        CommunityToolkit2ViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        private IDebugLog _debugLog;
        private IGetResult _getResult;
        private Func<Type, object?> _gsold;
        private Func<Type, object> _grsold;

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
                    .AddSingleton<IUserRepository, TestUserRepository>()
                    .AddSingleton<ICommunityToolkit2Model, TestUserModel>()
                    .AddSingleton<IDebugLog, DebugLog>()
                    .AddSingleton<IGetResult, GetResult>()
                    .BuildServiceProvider();
            IoC.GetReqSrv = t => sp.GetRequiredService(t);
            IoC.GetSrv = t => sp.GetService(t);
            _debugLog = IoC.GetRequiredService<IDebugLog>();
            _getResult = IoC.GetRequiredService<IGetResult>();
            _getResult.Register("Login", GetLoginResult);
            testModel = new();
            testModel.PropertyChanged += OnVMPropertyChanged;
            if (testModel is INotifyPropertyChanging npchgn)
                npchgn.PropertyChanging += OnVMPropertyChanging;
            ClearLog();
        }

        [TestCleanup]
        public void Cleanup()
        {
            testModel.Dispose();
            IoC.GetReqSrv = _grsold;
            IoC.GetSrv = _gsold;
        }
        private object? GetLoginResult(object[] arg)
        {
            if (arg.Length == 2
                && arg[0] is string username
                && arg[1] is string password
                && username == "User1")
                return new User() { Username = "User", Id = 1 };
            else
                return null;
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(CommunityToolkit2ViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
            Assert.IsInstanceOfType(_debugLog, typeof(IDebugLog));
            Assert.IsInstanceOfType(_getResult, typeof(IGetResult));
        }
        [DataTestMethod]
        [DataRow("", "", new[] { ""})]
        public void LoginTest(string user,string password,string[] asExp)
        {
            
        }
    }
}
