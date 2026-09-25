using System;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class HostPageView : UserControl
{
    public HostPageView()
    {
        InitializeComponent();
    }

    public HostPageView(IHostPageViewModel viewModel)
    {
        InitializeComponent();
        global::Views.ViewBinding.Commit(this, viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
    }
}
