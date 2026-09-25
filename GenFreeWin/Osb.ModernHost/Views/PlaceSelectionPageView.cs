using System;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class PlaceSelectionPageView : UserControl
{
    public PlaceSelectionPageView()
    {
        InitializeComponent();
    }

    public PlaceSelectionPageView(PlaceSelectionViewModel viewModel)
    {
        InitializeComponent();
        global::Views.ViewBinding.Commit(this, viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
    }
}
