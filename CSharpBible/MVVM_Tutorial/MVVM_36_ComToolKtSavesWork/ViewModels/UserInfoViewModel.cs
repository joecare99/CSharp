using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using MVVM_36_ComToolKtSavesWork.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Input;

namespace MVVM_36_ComToolKtSavesWork.ViewModels
{
    public partial class UserInfoViewModel : ObservableObject, IRecipient<ValueChangedMessage<User>>, IDisposable
    {
        [ObservableProperty]
        private User? _user;

        [ObservableProperty]
        private bool _showLogin = true;

        [ObservableProperty]
        private bool _showUser = false;

        public UserInfoViewModel()
        {
            WeakReferenceMessenger.Default.Register(this);
        }

        [RelayCommand]
        private void ShowLoginCtrl()
        {
            WeakReferenceMessenger.Default.Send(new ShowLoginMessage());    
        }
        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            GC.SuppressFinalize(this);
        }

        public void Receive(ValueChangedMessage<User> message)
        {
            this.User = message.Value;
            ShowUser = true;
            ShowLogin = false;
        }
    }
}
