using System;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class PublicationSettingsPageView : UserControl
{
    public PublicationSettingsPageView()
    {
        InitializeComponent();
    }

    public PublicationSettingsPageView(PublicationSettingsViewModel viewModel)
    {
        InitializeComponent();
        global::Views.ViewBinding.Commit(this, viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
    }
}
