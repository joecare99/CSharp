using System.Windows;
using TraceAnalysis.Workbench.Wpf.ViewModels;
using TraceAnalysis.Widgets.Wpf.ViewModels;

namespace TraceAnalysis.Workbench.Wpf;

/// <summary>
/// Interaction logic for the main workbench window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of <see cref="MainWindow"/>.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TraceSourceSummaryControl_GotFocus(object sender, RoutedEventArgs e)
    {
        SetContext(WorkbenchContextKind.TraceSource);
    }

    private void ChannelBrowserControl_GotFocus(object sender, RoutedEventArgs e)
    {
        SetContext(WorkbenchContextKind.ChannelBrowser);
    }

    private void ProcessingStepListControl_GotFocus(object sender, RoutedEventArgs e)
    {
        SetContext(WorkbenchContextKind.ProcessingSteps);
    }

    private void ProcessingEditorControl_GotFocus(object sender, RoutedEventArgs e)
    {
        SetContext(WorkbenchContextKind.CurrentStep);
    }

    private void PreviewGroupBox_GotFocus(object sender, RoutedEventArgs e)
    {
        SetContext(WorkbenchContextKind.Preview);
    }

    private void TraceChartControl_GotFocus(object sender, RoutedEventArgs e)
    {
        SetContext(WorkbenchContextKind.Chart);
    }

    private void DiagnosticsPanelControl_GotFocus(object sender, RoutedEventArgs e)
    {
        SetContext(WorkbenchContextKind.Diagnostics);
    }

    private void SetContext(WorkbenchContextKind contextKind)
    {
        if (DataContext is MainWorkbenchViewModel viewModel)
            viewModel.ActiveContext = contextKind;
    }
}
