using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using GenFree.Helper;

namespace GenFreeWpf.ViewModels;

public partial class RepoPageViewModel : BaseViewModelCT, IRepoViewModel
{
    [ObservableProperty]
    public partial string RepoName_Text { get; set; }

    [ObservableProperty]
    public partial string RepoStreet_Text { get; set; }

    [ObservableProperty]
    public partial string RepoPlace_Text { get; set; }

    [ObservableProperty]
    public partial string RepoPLZ_Text { get; set; }

    [ObservableProperty]
    public partial string RepoPhone_Text { get; set; }

    [ObservableProperty]
    public partial string RepoMail_Text { get; set; }

    [ObservableProperty]
    public partial string RichTextBox1_Text { get; set; }

    [ObservableProperty]
    public partial string RichTextBox2_Text { get; set; }

    [ObservableProperty]
    private Action doClose;

    public bool BtnDeleteVisible => true;

    public int SourceCount => 0;

    public float FontSize => 12.0f;

    public object HintFarb => default!;

    public IRelayCommand FormLoadCommand { get; }
    public IRelayCommand LinkClickCommand { get; }
    public IRelayCommand SaveCommand { get; }
    public IRelayCommand Save2Command { get; }
    public IRelayCommand CloseCommand { get; }
    public IRelayCommand NewEntryCommand { get; }
    public IRelayCommand DeleteCommand { get; }
    public IRelayCommand Sources_DblCommand { get; }
    public IRelayCommand List2DblCommand { get; }

    public System.Collections.ObjectModel.ObservableCollection<IListItem<int>> Sources_Items => throw new NotImplementedException();

    public System.Collections.ObjectModel.ObservableCollection<IListItem<int>> Repolist_Items => throw new NotImplementedException();

    public IListItem<int> Repolist_SelectedItem => throw new NotImplementedException();

    public IListItem<int> Sources_SelectedItem => throw new NotImplementedException();

    public Action<string> DoStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public RepoPageViewModel()
    {
        FormLoadCommand = new RelayCommand(OnFormLoad);
        LinkClickCommand = new RelayCommand(OnLinkClick);
        SaveCommand = new RelayCommand(OnSave);
        Save2Command = new RelayCommand(OnSave2);
        CloseCommand = new RelayCommand(OnClose);
        NewEntryCommand = new RelayCommand(OnNewEntry);
        DeleteCommand = new RelayCommand(OnDelete);
        Sources_DblCommand = new RelayCommand(OnList1Dbl);
        List2DblCommand = new RelayCommand(OnList2Dbl);
    }

    private void OnFormLoad() { }
    private void OnLinkClick() { }
    private void OnSave() { }
    private void OnSave2() { }
    private void OnClose() => DoClose?.Invoke();
    private void OnNewEntry() { }
    private void OnDelete() { }
    private void OnList1Dbl() { }
    private void OnList2Dbl() { }
}
