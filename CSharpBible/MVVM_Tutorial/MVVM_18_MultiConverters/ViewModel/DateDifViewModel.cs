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
using MVVM.ViewModel;
using MVVM_18_MultiConverters.Model;
using System;

namespace MVVM_18_MultiConverters.ViewModel
{
    /// <summary>
    /// Class DateDifViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class DateDifViewModel : BaseViewModel
    {
        public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
        /// <summary>
        /// The start date
        /// </summary>
        private DateTime _startDate = GetNow().AddDays(-30);
        /// <summary>
        /// The end date
        /// </summary>
        private DateTime _endDate = GetNow();
        /// <summary>
        /// The format
        /// </summary>
        private DateDifFormat _format = DateDifFormat.Days;
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get => _startDate; set => SetProperty(ref _startDate, value); }
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate { get => _endDate; set => SetProperty(ref _endDate, value); }
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public DateDifFormat Format { get => _format; set => SetProperty(ref _format, value); }

        /// <summary>
        /// Gets the date dif.
        /// </summary>
        /// <value>The date dif.</value>
        public TimeSpan DateDif { get => _endDate - _startDate; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateDifViewModel"/> class.
        /// </summary>
        public DateDifViewModel()
        {
            AddPropertyDependency(nameof(DateDif), nameof(StartDate));
            AddPropertyDependency(nameof(DateDif), nameof(EndDate));
            AddPropertyDependency(nameof(DateDif), nameof(Format));
        }

    }
}
