using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpBible.AboutEx.ViewModels.Interfaces;
using System;

namespace CSharpBible.AboutEx.ViewModels;

public partial class FrmAboutExMainViewModel : ObservableObject, IFrmAboutExMainViewModel
{
    [ObservableProperty]
    private string _title  = "AboutEx";

    [ObservableProperty]
    private string _version = "1.0.0.0";
    
    [ObservableProperty]
    private string _description = "AboutEx is a simple application that shows the version of the application.";

    [ObservableProperty]
    private string _author = "Joe Care";
    
    [ObservableProperty]
    private string _company = "JC-Soft";
    
    [ObservableProperty]
    private string _copyright = "© by JC-Soft 2025";
 
    [ObservableProperty]
    private string _product = "AboutEx";

    public Action<string[]> ShowAboutFrm1 { get; set; }
    public Action<string[]> ShowAboutFrm2 { get; set; }

    [RelayCommand]
    private void ShowAbout1() 
        => ShowAboutFrm1?.Invoke([_title, _version, _description, _author, _company, _copyright, _product]);

    [RelayCommand]
    private void ShowAbout2()
        => ShowAboutFrm2?.Invoke([_title, _version, _description, _author, _company, _copyright, _product]);
}
