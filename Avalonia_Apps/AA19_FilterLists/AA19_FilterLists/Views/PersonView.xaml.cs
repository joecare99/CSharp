// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 12-24-2021
// ***********************************************************************
// <copyright file="PersonView.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Avalonia.Controls;
using Avalonia;
using Avalonia.VisualTree;
using AA19_FilterLists.ViewModels;
using MessageBox.Avalonia;

namespace AA19_FilterLists.Views;

public partial class PersonView : UserControl
{
    public PersonView()
    {
        InitializeComponent();
        if (DataContext is PersonViewViewModel vm)
        {
            vm.MissingData += (_, __) => ShowError();
        }
        this.AttachedToVisualTree += (_, __) =>
        {
            if (DataContext is PersonViewViewModel vm2)
                vm2.MissingData += (_, __) => ShowError();
        };
    }

    private async void ShowError()
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is Window win)
        {
            await MessageBoxManager.GetMessageBoxStandardWindow("Hinweis", "Bitte einen Vornamen oder Nachnamen eingeben.").ShowDialog(win);
        }
    }
}
