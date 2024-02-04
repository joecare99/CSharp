﻿// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="UserInfoViewModelTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM.ViewModel.Tests;
using MVVM_36_ComToolKtSavesWork.Models;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_36_ComToolKtSavesWork.ViewModels.Tests
{
    /// <summary>
    /// Defines test class UserInfoViewModelTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class UserInfoViewModelTests : BaseTestViewModel<UserInfoViewModel>
    {

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private IDebugLog _debugLog;
        private IGetResult _getResult;
        private IMessenger _msg;
        private Func<Type, object?> _gsold;
        private Func<Type, object> _grsold;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public override void Init()
        {
            _gsold = IoC.GetSrv;
            _grsold = IoC.GetReqSrv;
            var sp = new ServiceCollection()
                    //          .AddSingleton<IUserRepository, TestUserRepository>()
                    .AddSingleton<IDebugLog, DebugLog>()
                    .AddSingleton(Substitute.For<IMessenger>())
                    .AddSingleton<IGetResult, GetResult>()
                    .BuildServiceProvider();
            IoC.GetReqSrv = t => sp.GetRequiredService(t);
            IoC.GetSrv = t => sp.GetService(t);
            _debugLog = IoC.GetRequiredService<IDebugLog>();
            _getResult = IoC.GetRequiredService<IGetResult>();
            _msg = IoC.GetRequiredService<IMessenger>();
            _getResult.Register("Login", GetLoginResult);
            base.Init();
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
            Assert.IsInstanceOfType(testModel, typeof(UserInfoViewModel));
            Assert.IsInstanceOfType(testModel, typeof(ObservableObject));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
            Assert.IsInstanceOfType(_debugLog, typeof(IDebugLog));
            Assert.IsInstanceOfType(_getResult, typeof(IGetResult));
        }

        [DataTestMethod]
        [DataRow("", "", new[] { "" })]
        public void LoginTest(string user, string password, string[] asExp)
        {

        }

        protected override Dictionary<string, object?> GetDefaultData() =>  new(){
            { nameof(UserInfoViewModel.ShowLogin),true },
            { nameof(UserInfoViewModel.ShowUser),false },
            { nameof(UserInfoViewModel.User),null },
        };
            
    }
}
