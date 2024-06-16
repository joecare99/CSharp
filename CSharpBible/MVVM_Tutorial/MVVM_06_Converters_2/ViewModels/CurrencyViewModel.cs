// ***********************************************************************
// Assembly         : MVVM_6_Converters_2
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="CurrencyViewModel.cs" company="MVVM_6_Converters_2">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;

namespace MVVM_06_Converters_2.ViewModels
{
    /// <summary>
    /// Class CurrencyViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class CurrencyViewModel : BaseViewModel
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
        /// Initializes a new instance of the <see cref="CurrencyViewModel"/> class.
        /// </summary>
        public CurrencyViewModel()
        {
            Value = 0;
        }

    }
}
