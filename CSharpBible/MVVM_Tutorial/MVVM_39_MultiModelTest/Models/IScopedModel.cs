// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTest
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="ISystemModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;

namespace MVVM_39_MultiModelTest.Models;

public interface IScopedModel : INotifyPropertyChanged,INotifyPropertyChanging
{
    string Name { get; }
    string Description { get; }
    ISystemModel? parent { get; set; }
    Guid Id { get; }
    IServiceScope? Scope { get; set; }

    int ICommonValue { get; set; }
}