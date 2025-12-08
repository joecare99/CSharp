// ***********************************************************************
// Assembly         : ConsoleMouseApp
// Author           : AI Assistant (MVVM refactor)
// Created          : 09-26-2025
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace ConsoleMouseApp.ViewModels;

public partial class ConsoleMouseViewModel : ObservableObject, IConsoleMouseViewModel
{
    [ObservableProperty]
    public partial string MousePosition { get; set; } = string.Empty; // generates MousePosition property

    [ObservableProperty]
    public partial bool Panel3_Visible { get; set; } = false;

    [ObservableProperty]
    public partial int HorizontalScroll { get; set; } = 0;

    [ObservableProperty]
    public partial int VerticalScroll { get; set; } = 0;

    [ObservableProperty]
    public partial string SelectedName { get; set; } = string.Empty;

    public ObservableCollection<string> Names { get; } = new ObservableCollection<string>(new [] {
        "Alice","Bob","Charlie","Diana","Eve","Frank","Grace","Heidi","Ivan","Judy","Karl","Liam","Mallory","Niaj","Olivia","Peggy","Rupert","Sybil","Trent","Uma","Victor","Walter","Xavier","Yvonne","Zara"
    });

    public event EventHandler? RequestClose;

    // RelayCommand attributes generate *Command properties
    [RelayCommand]
    private void One()
    {
        MousePosition = "[1] " + MousePosition;
        Panel3_Visible = !Panel3_Visible;
    }

    [RelayCommand] private void Ok() => MousePosition = "OK:" + MousePosition;
    [RelayCommand] private void Cancel() => RequestClose?.Invoke(this, EventArgs.Empty);

    [RelayCommand] private void Open() => MousePosition = "Open:" + DateTime.Now.ToLongTimeString();
    [RelayCommand] private void Save() => MousePosition = "Save:" + DateTime.Now.ToLongTimeString();
    [RelayCommand] private void Exit() => RequestClose?.Invoke(this, EventArgs.Empty);
    [RelayCommand] private void About() => MousePosition = "About ConsoleMouseApp";
}
