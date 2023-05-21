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
using System;

namespace MVVM_36_ComToolKtSavesWork.ViewModels.Tests
{
    public class DebugLog : IDebugLog
    {
        private string _debugLog="";

        string IDebugLog.DebugLog => _debugLog;

        public void ClearLog()
        {
            _debugLog="";
        }

        public void DoLog(string message)
        {
            _debugLog+=$"{message}{Environment.NewLine}";
        }
    }
}