// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork
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
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_36_ComToolKtSavesWork.Models;
using System;
using System.ComponentModel;

namespace MVVM_36_ComToolKtSavesWork.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class CommunityToolkit2ViewModel : BaseViewModelCT, 
        IRecipient<ShowLoginMessage>,
        IRecipient<ValueChangedMessage<User>>,
        IDisposable
    {
        #region Properties
        [ObservableProperty]
        private bool _showLogin = false;
        public static Func<ICommunityToolkit2Model> GetModel { get; set; } = () => new CommunityToolkit2Model();

        private readonly ICommunityToolkit2Model _model;

        public DateTime Now => _model?.Now ?? DateTime.MinValue;
        #endregion
  
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public CommunityToolkit2ViewModel():this(IoC.GetRequiredService<ICommunityToolkit2Model>())
        {
        }

        public CommunityToolkit2ViewModel(ICommunityToolkit2Model model)
        {
            _model = model;
            if (model != null)
                _model.PropertyChanged += OnMPropertyChanged;
            WeakReferenceMessenger.Default.Register<ShowLoginMessage>(this);
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<User>>(this);
        }
        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            GC.SuppressFinalize(this);
        }

        private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName); 
        }

        void IRecipient<ShowLoginMessage>.Receive(ShowLoginMessage message)
        {
            ShowLogin = true;
        }

        public void Receive(ValueChangedMessage<User> message)
        {
            ShowLogin = false;
        }

        #endregion
    }
}
