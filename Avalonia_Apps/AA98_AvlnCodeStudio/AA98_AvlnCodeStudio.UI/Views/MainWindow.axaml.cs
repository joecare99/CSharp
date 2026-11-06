using Avalonia.Controls;
using Config.UI;
using Config.UI.Avalonia.Views;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Property.Editor;
using Property.Editor.Avalonia;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Views
{
    public partial class MainWindow : Window
    {
        private sealed class SingleSectionRegistry(IConfigUiSection section) : IConfigUiSectionRegistry
        {
            public IReadOnlyList<IConfigUiSection> Sections { get; } = [section];
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
            viewModel.ApproveAIChatToolExecutionAsync = ApproveAIChatToolExecutionAsync;
            viewModel.ApproveSvnUpdateAsync = ApproveSvnUpdateAsync;
            viewModel.ReviewWorkspaceFileChangesAsync = ReviewWorkspaceFileChangesAsync;
            viewModel.OpenConfigurationDialogAsync = async () =>
            {
                if (viewModel.ConfigUi is not null)
                {
                    await new ConfigUiDialog(viewModel.ConfigUi).ShowDialog(this);
                }
            };
            viewModel.OpenProjectConfigurationDialogAsync = async () =>
            {
                CSharpWorkspace? workspace = viewModel.Workspace;
                if (workspace is null || workspace.Projects.Count == 0)
                {
                    viewModel.Editor.StatusText = "Open a C# workspace before editing project settings.";
                    return;
                }

                CSharpWorkspaceProject? project = workspace.Projects.Count == 1
                    ? workspace.Projects[0]
                    : await new ProjectSettingsTargetDialog(workspace.Projects).ShowDialog<CSharpWorkspaceProject?>(this);
                if (project is null)
                {
                    return;
                }

                ProjectAssemblyNameProvider provider = new(
                    workspace.RootPath,
                    project.ProjectPath,
                    viewModel.WorkspaceFileChangeService,
                    (proposal, cancellationToken) =>
                        viewModel.ReviewWorkspaceFileChangesAsync?.Invoke(proposal, cancellationToken)
                        ?? Task.FromResult(false));
                Config.UI.Avalonia.ViewModels.ConfigUiViewModel configViewModel = new(
                    new SingleSectionRegistry(new PropertyProviderConfigUiSection(
                        "Project",
                        project.Name,
                        provider,
                        $"Project settings from {project.ProjectPath}.")),
                    new PropertyEditorViewModel(),
                    $"Project Settings - {project.Name}");
                await new ConfigUiDialog(configViewModel).ShowDialog(this);
            };
        }

        private async Task<bool> ReviewWorkspaceFileChangesAsync(
            WorkspaceFileChangeProposal proposal,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (DataContext is not ViewModels.MainWindowViewModel viewModel)
            {
                throw new InvalidOperationException("The workbench view model is not available for change review.");
            }

            bool applied = await new WorkspaceFileChangeReviewDialog(
                proposal,
                viewModel.WorkspaceFileChangeService).ShowDialog(this);
            return applied;
        }

        private async Task<bool> ApproveAIChatToolExecutionAsync(
            AA98_AvlnCodeStudio.Base.AI.Models.AIChatToolDescriptor descriptor,
            JsonElement arguments,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            bool approved = await new AIChatToolApprovalDialog(descriptor, arguments).ShowDialog(this);
            cancellationToken.ThrowIfCancellationRequested();
            return approved;
        }

        private async Task<bool> ApproveSvnUpdateAsync(
            string workingCopyPath,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            bool approved = await new SvnUpdateConfirmationDialog(workingCopyPath).ShowDialog(this);
            cancellationToken.ThrowIfCancellationRequested();
            return approved;
        }
    }
}