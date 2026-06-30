using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GenFree.ViewModels.Interfaces;
using System;

namespace GenFreeAvln.Views;

public partial class Adresse : Window
{
    private readonly IAdresseViewModel _adresseViewModel;

    public Adresse(IAdresseViewModel viewModel)
    {
        _adresseViewModel = viewModel;
        DataContext = _adresseViewModel;
        _adresseViewModel.OnClose += ViewModel_OnClose;

        InitializeComponent();
        Opened += Adresse_Opened;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ViewModel_OnClose(object? sender, EventArgs e)
    {
        Hide();
    }

    private void Adresse_Opened(object? sender, EventArgs e)
    {
        _adresseViewModel.FormLoadCommand.Execute(e);
    }
}
