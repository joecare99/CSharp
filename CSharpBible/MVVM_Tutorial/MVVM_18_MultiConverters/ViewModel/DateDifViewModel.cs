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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_18_MultiConverters.ViewModel
{
    /// <summary>
    /// Class DateDifViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class DateDifViewModel : BaseViewModel
    {
        /// <summary>
        /// The start date
        /// </summary>
        private DateTime _startDate = DateTime.Now.AddDays(-30);
        /// <summary>
        /// The end date
        /// </summary>
        private DateTime _endDate = DateTime.Now;
        /// <summary>
        /// The format
        /// </summary>
        private DateDifFormat _format = DateDifFormat.Days;
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get => _startDate; set { if (_startDate == value) return; _startDate = value; RaisePropertyChanged(nameof(StartDate), nameof(DateDif)); } }
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate { get => _endDate; set { if (_endDate == value) return; _endDate = value; RaisePropertyChanged(nameof(EndDate), nameof(DateDif)); } }
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public DateDifFormat Format { get => _format   ; set { if (_format == value) return; _format = value; RaisePropertyChanged(nameof(Format), nameof(DateDif)); } }

        /// <summary>
        /// Gets the date dif.
        /// </summary>
        /// <value>The date dif.</value>
        public TimeSpan DateDif { get => _endDate - _startDate; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateDifViewModel"/> class.
        /// </summary>
        public DateDifViewModel(){
            }

    }
}
