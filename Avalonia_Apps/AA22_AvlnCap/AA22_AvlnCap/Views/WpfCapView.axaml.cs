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
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA22_AvlnCap.Views;

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
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
