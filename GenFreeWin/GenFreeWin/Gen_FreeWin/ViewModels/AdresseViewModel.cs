using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Gen_FreeWin.ViewModels;

public partial class AdresseViewModel:BaseViewModelCT, IAdresseViewModel
{
    [ObservableProperty]
    public partial string Title { get; set; }
    [ObservableProperty]
    public partial string Givenname { get; set; }
    [ObservableProperty]
    public partial string Surname { get; set; }
    [ObservableProperty]
    public partial string Street { get; set; }
    [ObservableProperty]
    public partial string Zip { get; set; }
    [ObservableProperty]
    public partial string Place { get; set; }
    [ObservableProperty]
    public partial string Phone { get; set; }
    [ObservableProperty]
    public partial string EMail { get; set; }
    [ObservableProperty]
    public partial string Special { get; set; }

    public event EventHandler? OnClose;

    public Action? DoHide { get; set; }
    private Adresse View => Adresse.Default;
    public IRelayCommand SaveCommand => throw new NotImplementedException();

    [RelayCommand]
    private void Button1()
    {
        if (_Modul1.Instance.Typ == DriveType.CDRom)
        {
            DoHide?.Invoke();
            return;
        }
        if (EMail.Trim() != "")
        {
            string pattern = "^([\\w-]+\\.)*?[\\w-]+@[\\w-]+\\.([\\w-]+\\.)*?[\\w]+$";
            if (!Regex.IsMatch(EMail, pattern))
            {
                _ = Interaction.MsgBox("Die EMail-Adresse ist nicht korrekt!");
                return;
            }
        }
        _Modul1.Instance.Persistence.WriteStringsProg("Adresse", [Title,Givenname,]);
        DoHide?.Invoke();
        Menue.Default.SetAdress(Givenname.Trim() + " " + Surname.Trim());
    }

    [RelayCommand]
    private void FormLoad()
    {
        View.BackColor = Menue.Default.BackColor;
        FileSystem.FileClose(99);
        if (_Modul1.Instance.Typ != DriveType.CDRom)
        {
            FileSystem.FileOpen(99, Path.Combine(_Modul1.Instance.GenFreeDir, "Adresse"), OpenMode.Append);
        }
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Path.Combine(_Modul1.Instance.GenFreeDir, "Adresse"), OpenMode.Input);
        if (!FileSystem.EOF(99))
        {
            Title = FileSystem.LineInput(99);
            Givenname = FileSystem.LineInput(99);
            if (!FileSystem.EOF(99))
            {
                Surname = FileSystem.LineInput(99);
            }
            if (!FileSystem.EOF(99))
            {
                Street = FileSystem.LineInput(99);
            }
            if (!FileSystem.EOF(99))
            {
                Zip = FileSystem.LineInput(99);
            }
            if (!FileSystem.EOF(99))
            {
                Place = FileSystem.LineInput(99);
            }
            if (!FileSystem.EOF(99))
            {
                Phone = FileSystem.LineInput(99);
            }
            if (!FileSystem.EOF(99))
            {
                EMail = FileSystem.LineInput(99);
            }
            if (!FileSystem.EOF(99))
            {
                Special = FileSystem.LineInput(99);
            }
        }
    }
    
}
