// ***********************************************************************
// Assembly         : MVVM_6_Converters_2
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="CurrencyViewViewModel.cs" company="MVVM_6_Converters_2">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;

namespace MVVM_6_Converters_2.ViewModel
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
        public decimal Value { get => _value;
            set
            { if (_value == value) return; _value = value; RaisePropertyChanged(); } 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyViewViewModel"/> class.
        /// </summary>
        public CurrencyViewViewModel()
        {
            Value = 0;
        }

    }
}
