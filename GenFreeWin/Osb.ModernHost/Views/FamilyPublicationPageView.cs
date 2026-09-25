using System;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class FamilyPublicationPageView : UserControl
{
    public FamilyPublicationPageView()
    {
        InitializeComponent();
    }

    public FamilyPublicationPageView(FamilyPublicationViewModel viewModel)
    {
        InitializeComponent();
        global::Views.ViewBinding.Commit(this, viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
    }
}
