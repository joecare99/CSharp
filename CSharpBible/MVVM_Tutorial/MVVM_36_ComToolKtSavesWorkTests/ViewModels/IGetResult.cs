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
using System.Runtime.CompilerServices;

namespace MVVM_36_ComToolKtSavesWork.ViewModels.Tests
{
    public interface IGetResult
    {
        object? Get( object[] objects, [CallerMemberName] string proc="");
        void Register(string proc, Func<object[],object?> fesultFct);
    }
}