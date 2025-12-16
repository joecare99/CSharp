// ***********************************************************************
// Assembly         : MVVM_21_Buttons
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="ButtonsViewViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;

namespace AA21_Buttons.ViewModels;

/// <summary>
/// ViewModel für die Buttons-View mit Flip-Logik.
/// </summary>
public partial class ButtonsViewViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PlayCommand))]
    private object? lastPara;

    [ObservableProperty]
    private bool[] flip = new bool[10];

    public ButtonsViewViewModel()
    {
        flip[1] = true;
    }

    /// <summary>
    /// Kann die Play-Aktion ausgeführt werden?
    /// </summary>
    private bool CanPlay(object? parameter) => true;// LastPara == null || LastPara != parameter;

    /// <summary>
    /// Play-Befehl: Wechselt den Zustand des angeklickten Buttons und benachbarter Buttons.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanPlay))]
    private void Play(object? parameter)
    {
        if (parameter == null) return;

        LastPara = parameter;

        if (parameter is string s && int.TryParse(s, out int ix))
        {
            Flip[ix] = !Flip[ix];
            
            // Beeinflussungsmuster: ±1 (selbe Reihe), ±3 (über/unter)
            foreach (var ixo in new[] { -1, 1, -3, 3 })
            {
                if (ix + ixo <= 9 && ix + ixo > 0)
                {
                    // Nur horizontal benachbarte oder vertikal benachbarte in selber Reihe
                    if (Math.Abs(ixo) > 1 || (ix - 1) / 3 == (ix + ixo - 1) / 3)
                    {
                        Flip[ix + ixo] = !Flip[ix + ixo];
                    }
                }
            }

            OnPropertyChanged(nameof(Flip));
        }
    }

    /// <summary>
    /// Reset-Befehl: Setzt alle Buttons zurück (nur Button 1 ist an).
    /// </summary>
    [RelayCommand]
    private void Reset(object? parameter)
    {
        foreach (var ix in new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })
        {
            Flip[ix] = false;
        }

        Flip[1] = true;
        LastPara = "0";
        OnPropertyChanged(nameof(Flip));
    }
}
