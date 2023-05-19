// ***********************************************************************
// Assembly         : MVVM_00a_CTTemplate
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using System.Timers;

namespace MVVM_00a_CTTemplate.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class TemplateViewModel : BaseViewModel
    {
        #region Properties
        private readonly Timer _timer;
        public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
        public DateTime Now { get => GetNow(); }
        #endregion 
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public TemplateViewModel()
        {
            _timer = new Timer() { Interval = 250};
            _timer.Elapsed += (s, e) => RaisePropertyChanged(nameof(Now));
            _timer.Start();
        }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Finalizes an instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        ~TemplateViewModel()
        {
            _timer.Stop();
            return;
        }
#endif
        #endregion
    }
}
