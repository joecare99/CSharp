using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.UI.Views;

public partial class ProjectSettingsTargetDialog : Window
{
    public ProjectSettingsTargetDialog()
        : this(Array.Empty<CSharpWorkspaceProject>())
    {
    }

    public ProjectSettingsTargetDialog(IReadOnlyList<CSharpWorkspaceProject> projects)
    {
        ArgumentNullException.ThrowIfNull(projects);
        InitializeComponent();
        Projects = projects;
        DataContext = this;
    }

    public IReadOnlyList<CSharpWorkspaceProject> Projects { get; }

    public CSharpWorkspaceProject? SelectedProject { get; set; }

    private void OnOpenClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (SelectedProject is not null)
        {
            Close(SelectedProject);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs eventArgs) => Close(null);
}
