// ***********************************************************************
// Assembly         : MVVM_6_Converters
// Author           : Mir
// Created          : 07-01-2022
//
// Last Modified By : Mir
// Last Modified On : 07-01-2022
// ***********************************************************************
// <copyright file="CurrencyViewViewModel.cs" company="MVVM_6_Converters">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;

namespace MVVM_6_Converters.ViewModel
{
    /// <summary>
    /// Class CurrencyViewViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class CurrencyViewViewModel : BaseViewModel
    {
        /// <summary>
        /// The value
        /// </summary>
        private decimal _value;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public decimal Value { get => _value; set => SetProperty(ref _value, value); }

        public DelegateCommand EnterKey { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyViewViewModel"/> class.
        /// </summary>
        public CurrencyViewViewModel()
        {
            Value = 10;
            EnterKey = new DelegateCommand(OnEnterKey);
        }

        private void OnEnterKey(object? obj)
        {
            ;
        }
    }
}
