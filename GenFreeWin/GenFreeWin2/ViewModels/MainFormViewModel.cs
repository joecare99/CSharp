using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GenFree.Interfaces.UI;
using GenFreeWin2.ViewModels.Interfaces;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenFreeWin2.ViewModels;

public partial class MainFormViewModel: BaseViewModelCT, IMainFormViewModel, IRecipient<IShowFrmMsg>,IRecipient<IShowDlgMsg>, IDisposable
{
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private Control _view;

    public Action<Control> ShowDialog { get; set; }

    public MainFormViewModel(IMessenger messenger)
    {
        messenger.RegisterAll(this);
        _messenger = messenger;
    }

    public void Dispose()
    {
        _messenger.UnregisterAll(this);
    }

    public void Receive(IShowFrmMsg message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (message.View as Control == null)
            throw new ArgumentNullException(nameof(message.View));
        if (message.View != (object)View)
        {
            View = message.View as Control;
        }
    }

    public void Receive(IShowDlgMsg message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (message.Dialog as Control == null)
            throw new ArgumentNullException(nameof(message.Dialog));
        ShowDialog?.Invoke(message.Dialog as Control);
    }
}
