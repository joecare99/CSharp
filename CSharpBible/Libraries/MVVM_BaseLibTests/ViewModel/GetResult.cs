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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MVVM.ViewModel.Tests
{
    public class GetResult : IGetResult
    {
        private readonly Dictionary<string,Func<object[],object?>> _Dic=new();

        public int Count => _Dic.Count;
        public object? Get(object[] objects, [CallerMemberName] string proc = "")
        {
            if (_Dic.TryGetValue(proc??"", out var f))
                return f(objects);
            else
                return null;
        }

        public void Register(string proc, Func<object[], object?> fResultFct)
        {
            if (_Dic.ContainsKey(proc))
                _Dic[proc] = fResultFct;
            else
                _Dic.Add(proc, fResultFct);
        }

    }
}