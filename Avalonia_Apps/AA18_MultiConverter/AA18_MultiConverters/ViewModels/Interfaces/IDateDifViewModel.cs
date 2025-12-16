// ***********************************************************************
// Assembly         : MVVM_18_MultiConverters
// Author           : Mir
// Created          : 07-05-2022
//
// Last Modified By : Mir
// Last Modified On : 07-05-2022
// ***********************************************************************
// <copyright file="DateDifViewModel.cs" company="MVVM_18_MultiConverters">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA18_MultiConverter.Model;

namespace AA18_MultiConverter.ViewModels.Interfaces;

public interface IDateDifViewModel
{
    DateTimeOffset StartDate { get; set; }
    DateTimeOffset EndDate { get; set; }
    DateDifFormat Format { get; set; }
    TimeSpan DateDif { get; }
    DateDifFormat[] DateFormats { get; }
}