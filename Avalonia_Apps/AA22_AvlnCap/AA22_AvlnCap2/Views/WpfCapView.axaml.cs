// ***********************************************************************
// Assembly         : AA22_AvlnCap
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="WpfCapView.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA22_AvlnCap2.ViewModels.Interfaces;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Views.Extension;

namespace AA22_AvlnCap2.Views;

/// <summary>
/// Interaktionslogik für WpfCapView.axaml
/// </summary>
public partial class WpfCapView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WpfCapView"/> class.
    /// </summary>
    public WpfCapView()
    {
        InitializeComponent();
        DataContext = IoC.GetRequiredService<IWpfCapViewModel>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
