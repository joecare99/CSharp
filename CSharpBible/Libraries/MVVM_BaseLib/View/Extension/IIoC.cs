// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 05-20-2023
//
// Last Modified By : Mir
// Last Modified On : 09-26-2023
// ***********************************************************************
// <copyright file="IoC.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MVVM.View.Extension;

public interface IIoC
{
    Type Type { get; set; }

    object? ProvideValue(IServiceProvider serviceProvider);
}