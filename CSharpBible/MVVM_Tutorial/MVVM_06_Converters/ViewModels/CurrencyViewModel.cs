// ***********************************************************************
// Assembly         : MVVM_06_Converters
// Author           : Mir
// Created          : 07-01-2022
//
// Last Modified By : Mir
// Last Modified On : 07-01-2022
// ***********************************************************************
// <copyright file="CurrencyViewModel.cs" company="MVVM_06_Converters">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;

namespace MVVM_06_Converters.ViewModels
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
        public decimal Value { get => _value; set => SetProperty(ref _value, value); }

        public DelegateCommand EnterKey { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyViewModel"/> class.
        /// </summary>
        public CurrencyViewModel()
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
