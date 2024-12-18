// ***********************************************************************
// Assembly         : MVVM_6_Converters_3
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="CurrencyViewViewModel.cs" company="MVVM_6_Converters_3">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;

namespace MVVM_06_Converters_3.ViewModels
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
            { if (_value == value) return; _value = value; RaisePropertyChanged(nameof(Value), nameof(ValueIsNotZero) ); } 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyViewViewModel"/> class.
        /// </summary>
        public CurrencyViewViewModel()
        {
            Value = 0;
        }

        /// <summary>
        /// Gets a value indicating whether [value is not zero].
        /// </summary>
        /// <value><c>true</c> if [value is not zero]; otherwise, <c>false</c>.</value>
        public bool ValueIsNotZero => _value != 0;
    }
}
