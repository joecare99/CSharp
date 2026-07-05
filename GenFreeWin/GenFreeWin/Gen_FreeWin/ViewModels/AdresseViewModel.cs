using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
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
        IList<string> lines = [];
        _Modul1.Instance.Persistence.ReadStringsProg("Adresse", lines, 9);
        if (lines.Count > 1)
        {
            Title = lines[0];
            Givenname = lines[1];
            if (lines.Count > 2)
            {
                Surname = lines[2];
            }
            if (lines.Count > 3)
            {
                Street = lines[3];
            }
            if (lines.Count > 4)
            {
                Zip = lines[4];
            }
            if (lines.Count > 5)
            {
                Place = lines[5];
            }
            if (lines.Count > 6)
            {
                Phone = lines[6];
            }
            if (lines.Count > 7)
            {
                EMail = lines[7];
            }
            if (lines.Count > 8)
            {
                Special = lines[8];
            }
        }
    }
    
}
