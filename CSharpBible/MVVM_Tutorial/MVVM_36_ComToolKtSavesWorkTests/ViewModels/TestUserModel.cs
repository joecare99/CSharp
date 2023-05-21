// ***********************************************************************
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
using MVVM_36_ComToolKtSavesWork.Models;
using System;
using System.ComponentModel;

namespace MVVM_36_ComToolKtSavesWork.ViewModels.Tests
{
    public class TestUserModel : ICommunityToolkit2Model
    {
        private IGetResult _getResult;

        public TestUserModel(IGetResult getResult) {
            _getResult = getResult;
        }
        public DateTime Now => (DateTime)_getResult.Get(new object[] { })!;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}