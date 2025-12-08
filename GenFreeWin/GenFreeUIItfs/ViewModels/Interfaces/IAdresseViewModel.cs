using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface IAdresseViewModel : INotifyPropertyChanged
{
    string Title { get; set; }
    string Givenname { get; set; }
    string Surname { get; set; }
    string Street { get; set; }
    string Zip { get; set; }
    string Place { get; set; }
    string Phone { get; set; }
    string EMail { get; set; }
    string Special { get; set; }
    IRelayCommand SaveCommand { get; }
    IRelayCommand FormLoadCommand { get; }

    event EventHandler OnClose;
}