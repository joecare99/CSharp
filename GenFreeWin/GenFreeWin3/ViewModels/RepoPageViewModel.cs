using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.ViewModels.Interfaces;

namespace GenFreeWpf.ViewModels;

public partial class RepoPageViewModel : BaseViewModelCT, IRepoViewModel
{
    [ObservableProperty]
    private string repoName_Text;

    [ObservableProperty]
    private string textBox2_Text;

    [ObservableProperty]
    private string textBox3_Text;

    [ObservableProperty]
    private string textBox4_Text;

    [ObservableProperty]
    private string textBox5_Text;

    [ObservableProperty]
    private string textBox6_Text;

    [ObservableProperty]
    private string richTextBox1_Text;

    [ObservableProperty]
    private string richTextBox2_Text;

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
