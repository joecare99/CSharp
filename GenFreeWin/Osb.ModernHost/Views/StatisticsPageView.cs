using System;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class StatisticsPageView : UserControl
{
    public StatisticsPageView()
    {
        InitializeComponent();
    }

    public StatisticsPageView(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        global::Views.ViewBinding.Commit(this, viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
    }
}
