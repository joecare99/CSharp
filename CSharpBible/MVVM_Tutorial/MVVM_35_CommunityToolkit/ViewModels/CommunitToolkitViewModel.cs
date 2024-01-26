// ***********************************************************************
// Assembly         : MVVM_35_CommunityToolkit
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Timers;

namespace MVVM_35_CommunityToolkit.ViewModels
{
    public partial class CommunitToolkitViewModel : ObservableObject
    {
        #region Properties
        private readonly Timer _timer;
        public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
        public DateTime Now { get => GetNow(); }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DoClickMeCommand))]
        private string _prop1="";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DoClickMeCommand))]
        private string _prop2 = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DoClickMeCommand))]
        private string _prop3 = "";

        [ObservableProperty]
        private string _prop4 = "";
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public CommunitToolkitViewModel()
        {
            _timer = new Timer() { Interval = 250 };
            _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
            _timer.Start();
        }

        private bool _CanDoPressMe 
            => !string.IsNullOrEmpty(_prop1) 
            && !string.IsNullOrEmpty(_prop2) 
            && !string.IsNullOrEmpty(_prop3);

        [RelayCommand(CanExecute = nameof(_CanDoPressMe))]
        private void DoClickMe()
        {
            Prop4 = $"Do({_prop1},{_prop2},{_prop3})";
        }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Finalizes an instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        ~CommunitToolkitViewModel()
        {
            _timer.Stop();
            return;
        }
#endif
        #endregion
    }
}
