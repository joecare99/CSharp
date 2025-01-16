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
using MVVM.ViewModel.Tests;
using MVVM_36_ComToolKtSavesWork.Models;

namespace MVVM_36_ComToolKtSavesWork.ViewModels.Tests;

public class TestUserRepository : IUserRepository
{
    private IDebugLog _debLog;
    private IGetResult _getResult;

    public TestUserRepository(IDebugLog log,IGetResult getResult)
    {
        _debLog = log;
        _getResult = getResult;
    }
    public User? Login(string username, string password)
    {
        _debLog.DoLog($"Login({username},{password})");
        return (User?)_getResult.Get(new object[] {username,password});
    }
}