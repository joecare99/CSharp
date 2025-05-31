using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace GenFreeWpf.ViewModels;

public partial class MenueViewModel : ObservableObject
{
    [RelayCommand]
    private void OpenStammdaten()
    {
        // Navigation oder Logik für Stammdaten
    }

    [RelayCommand]
    private void OpenEingaben()
    {
        // Navigation oder Logik für Eingaben
    }

    [RelayCommand]
    private void OpenAuswertungen()
    {
        // Navigation oder Logik für Auswertungen
    }

    [RelayCommand]
    private void OpenExtras()
    {
        // Navigation oder Logik für Extras
    }

    [RelayCommand]
    private void Beenden()
    {
        Application.Current.Shutdown();
    }
}
