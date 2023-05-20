using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_36_ComToolKtSavesWork.Models;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MVVM_36_ComToolKtSavesWork.ViewModels
{
    public partial class UserInfoViewModel : ObservableObject, IRecipient<ValueChangedMessage<User>>, IDisposable
    {
        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private bool _showLogin = true;

        [ObservableProperty]
        private bool _showUser = false;

        public UserInfoViewModel() //: this(Ioc.Default.GetService<IUserRepository>())
        {
            WeakReferenceMessenger.Default.Register(this);
        }

        [RelayCommand()]
        private void _ShowLogin()
        {
            WeakReferenceMessenger.Default.Send(new ShowLoginMessage());    
        }
        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        public void Receive(ValueChangedMessage<User> message)
        {
            this.User = message.Value;
            ShowUser = true;
            ShowLogin = false;
        }
    }
}
