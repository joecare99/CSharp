using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Input;
using BaseLib.Helper;
using MVVM_36_ComToolKtSavesWork.Models;

namespace MVVM_36_ComToolKtSavesWork.ViewModels;

public partial class UserInfoViewModel : ObservableObject, IRecipient<ValueChangedMessage<User>>, IDisposable
{
    private IMessenger _messenger;
    
    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _showLogin = true;

    [ObservableProperty]
    private bool _showUser = false;

    public UserInfoViewModel():this(IoC.GetRequiredService<IMessenger>())
    {
    }
    public UserInfoViewModel(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register(this);
    }

    [RelayCommand]
    private void ShowLoginCtrl()
    {
        _messenger.Send(new ShowLoginMessage());    
    }
    public void Dispose()
    {
        _messenger.UnregisterAll(this);
        GC.SuppressFinalize(this); //??
    }

    public void Receive(ValueChangedMessage<User> message)
    {
        this.User = message.Value;
        ShowUser = true;
        ShowLogin = false;
    }
}
