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
using CommunityToolkit.Mvvm.ComponentModel;
using AA18_MultiConverter.Model;
using System;
using AA18_MultiConverter.ViewModels.Interfaces;

namespace AA18_MultiConverter.ViewModels;

public partial class DateDifViewModel : ObservableObject, IDateDifViewModel
{
    public static Func<DateTimeOffset> GetNow { get; set; } = () => DateTimeOffset.Now;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateDif))]
    private DateTimeOffset startDate = GetNow().AddDays(-30);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateDif))]
    private DateTimeOffset endDate = GetNow();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateDif))]
    private DateDifFormat format = DateDifFormat.Days;

    public TimeSpan DateDif => EndDate - StartDate;

    public DateDifFormat[] DateFormats { get; } = Enum.GetValues<DateDifFormat>();

}
