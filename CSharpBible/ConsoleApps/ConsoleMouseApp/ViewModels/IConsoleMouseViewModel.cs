// ***********************************************************************
// Assembly         : ConsoleMouseApp
// Author           : AI Assistant (MVVM refactor)
// Created          : 09-26-2025
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ConsoleMouseApp.ViewModels;

public interface IConsoleMouseViewModel: INotifyPropertyChanged
{
    string MousePosition { get; set; }
    IRelayCommand OneCommand { get; }
    IRelayCommand OkCommand { get; }
    IRelayCommand CancelCommand { get; }
    IRelayCommand OpenCommand { get; }
    IRelayCommand SaveCommand { get; }
    IRelayCommand ExitCommand { get; }
    IRelayCommand AboutCommand { get; }
    bool Panel3_Visible { get; set; }
    int HorizontalScroll { get; set; }
    int VerticalScroll { get; set; }
    string SelectedName { get; set; }
    ObservableCollection<string> Names { get; }

    event EventHandler? RequestClose;
}