using System.Linq;
using TraceAnalysis.Workbench.Wpf.ViewModels;

namespace TraceAnalysis.Workbench.Wpf.Services;

/// <summary>
/// Creates a simple keyboard-oriented menu baseline for the workbench shell.
/// </summary>
public sealed class WorkbenchMenuService : IWorkbenchMenuService
{
    /// <inheritdoc/>
    public MainMenuViewModel CreateMenu(MainWorkbenchViewModel shellViewModel)
    {
        var menu = new MainMenuViewModel();

        var fileMenu = new MenuItemViewModel("_File");
        fileMenu.Items.Add(new MenuItemViewModel("_Load Trace", shellViewModel.LoadTraceCommand, MenuCommandKind.General, MenuContextScope.Global));
        fileMenu.Items.Add(new MenuItemViewModel("_New Configuration", shellViewModel.ProcessingEditor.NewCommand, MenuCommandKind.General, MenuContextScope.Global));
        fileMenu.Items.Add(new MenuItemViewModel("_Open Configuration", shellViewModel.ProcessingEditor.OpenCommand, MenuCommandKind.General, MenuContextScope.Global));
        fileMenu.Items.Add(new MenuItemViewModel("_Save Configuration", shellViewModel.ProcessingEditor.SaveCommand, MenuCommandKind.General, MenuContextScope.Global));
        fileMenu.Items.Add(new MenuItemViewModel("Save Configuration _As", shellViewModel.ProcessingEditor.SaveAsCommand, MenuCommandKind.General, MenuContextScope.Global));

        var traceMenu = new MenuItemViewModel("_Trace");
        traceMenu.Items.Add(new MenuItemViewModel("Trace _Summary", commandKind: MenuCommandKind.Conditional, contextScope: MenuContextScope.TraceSource));
        traceMenu.Items.Add(new MenuItemViewModel("Channel _Browser", commandKind: MenuCommandKind.Conditional, contextScope: MenuContextScope.ChannelBrowser));
        traceMenu.Items.Add(new MenuItemViewModel("_Chart", commandKind: MenuCommandKind.Conditional, contextScope: MenuContextScope.Chart));

        var viewMenu = new MenuItemViewModel("_View");
        viewMenu.Items.Add(new MenuItemViewModel("_Preview", commandKind: MenuCommandKind.Conditional, contextScope: MenuContextScope.Preview));
        viewMenu.Items.Add(new MenuItemViewModel("_Diagnostics", commandKind: MenuCommandKind.Conditional, contextScope: MenuContextScope.Diagnostics));

        var processingMenu = new MenuItemViewModel("_Processing");
        processingMenu.Items.Add(new MenuItemViewModel("_Processing Steps", commandKind: MenuCommandKind.Conditional, contextScope: MenuContextScope.ProcessingSteps));
        processingMenu.Items.Add(new MenuItemViewModel("Current _Step", commandKind: MenuCommandKind.Conditional, contextScope: MenuContextScope.CurrentStep));
        processingMenu.Items.Add(new MenuItemViewModel("_Add Step", shellViewModel.ProcessingEditor.AddStepCommand, MenuCommandKind.Conditional, MenuContextScope.ProcessingSteps));
        processingMenu.Items.Add(new MenuItemViewModel("_Remove Step", shellViewModel.ProcessingEditor.RemoveStepCommand, MenuCommandKind.Conditional, MenuContextScope.ProcessingSteps));

        var helpMenu = new MenuItemViewModel("_Help");
        helpMenu.Items.Add(new MenuItemViewModel("_About", commandKind: MenuCommandKind.General, contextScope: MenuContextScope.Global));

        menu.Items.Add(fileMenu);
        menu.Items.Add(traceMenu);
        menu.Items.Add(viewMenu);
        menu.Items.Add(processingMenu);
        menu.Items.Add(helpMenu);

        ApplyContext(menu, WorkbenchContextKind.None);
        return menu;
    }

    /// <inheritdoc/>
    public void ApplyContext(MainMenuViewModel menu, WorkbenchContextKind contextKind)
    {
        foreach (var topLevelItem in menu.Items)
        {
            foreach (var childItem in topLevelItem.Items)
            {
                if (childItem.CommandKind == MenuCommandKind.General)
                {
                    childItem.IsEnabled = true;
                    continue;
                }

                childItem.IsEnabled = MatchesContext(childItem.ContextScope, contextKind);
            }
        }
    }

    private static bool MatchesContext(MenuContextScope scope, WorkbenchContextKind contextKind)
    {
        return scope switch
        {
            MenuContextScope.Global => true,
            MenuContextScope.TraceSource => contextKind == WorkbenchContextKind.TraceSource,
            MenuContextScope.ChannelBrowser => contextKind == WorkbenchContextKind.ChannelBrowser,
            MenuContextScope.ProcessingSteps => contextKind == WorkbenchContextKind.ProcessingSteps,
            MenuContextScope.CurrentStep => contextKind == WorkbenchContextKind.CurrentStep,
            MenuContextScope.Preview => contextKind == WorkbenchContextKind.Preview,
            MenuContextScope.Chart => contextKind == WorkbenchContextKind.Chart,
            MenuContextScope.Diagnostics => contextKind == WorkbenchContextKind.Diagnostics,
            _ => contextKind == WorkbenchContextKind.None
        };
    }
}
