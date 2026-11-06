using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Config.UI;
using Config.UI.Avalonia.Views;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Property.Editor;
using Property.Editor.Avalonia;
using Property.Editor.Avalonia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    using Configuration.AIProviderDraftViewModel? draft = viewModel.CreateAIProviderDraft();
                    ConfigUiDialog dialog = new(viewModel.ConfigUi);
                    var originalValidator = viewModel.ConfigUi.ValidateChangesAsync;
                    if (draft is not null)
                    {
                        dialog.Height = 620;
                        dialog.HostContent = new AIProviderDraftView
                        {
                            DataContext = draft,
                            OpenCredentialsAsync = async () =>
                                await new AICredentialsDialog { DataContext = viewModel }.ShowDialog(dialog),
                        };
                        draft.ProfileEndpointNavigationRequested += (provider, revision) =>
                        {
                            var focused = dialog.FocusManager?.GetFocusedElement();
                            Dispatcher.UIThread.Post(() =>
                            {
                                if (!dialog.IsVisible || !dialog.IsActive || !draft.IsProfileNavigationCurrent(provider, revision)
                                    || !ReferenceEquals(focused, dialog.FocusManager?.GetFocusedElement()))
                                {
                                    return;
                                }

                                string propertyName = Configuration.CodeStudioSettings.GetProfileEndpointPropertyName(provider);
                                PropertyEditorItemViewModel? item = viewModel.ConfigUi.PropertyEditor.Items
                                    .FirstOrDefault(candidate => candidate.IsVisible && candidate.PropertyItem.Name == propertyName);
                                PropertyEditorPanel? panel = dialog.GetVisualDescendants().OfType<PropertyEditorPanel>().FirstOrDefault();
                                Grid? row = panel?.GetVisualDescendants().OfType<Grid>()
                                    .FirstOrDefault(candidate => ReferenceEquals(candidate.DataContext, item));
                                if (item is null || row is null)
                                {
                                    return;
                                }

                                row.BringIntoView();
                                if (item.IsEditable)
                                {
                                    row.GetVisualDescendants().OfType<TextBox>()
                                        .FirstOrDefault(candidate => candidate.IsVisible && candidate.IsEnabled)?.Focus();
                                }
                            }, DispatcherPriority.Background);
                        };
                        viewModel.ConfigUi.ValidateChangesAsync = async sections =>
                        {
                            string? changed = await viewModel.ConfirmAIProviderTemplatesAsync(draft, sections);
                            return changed ?? (originalValidator is null ? null : await originalValidator(sections));
                        };
                    }
                    try
                    {
                        await dialog.ShowDialog(this);
                    }
                    finally
                    {
                        viewModel.ConfigUi.ValidateChangesAsync = originalValidator;
                    }
                }
            };
            viewModel.OpenAICredentialsDialogAsync = async () =>
                await new AICredentialsDialog { DataContext = viewModel }.ShowDialog(this);
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
                global::Config.UI.Avalonia.ViewModels.ConfigUiViewModel configViewModel = new(
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
            return await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DataContext is not ViewModels.MainWindowViewModel viewModel)
                {
                    throw new InvalidOperationException("The workbench view model is not available for AI tool approval.");
                }

                AIChatToolApprovalDialog dialog = new(descriptor, arguments,
                    () => viewModel.SendAIChatMessageCancelCommand.Execute(null));
                Task<bool> approval = dialog.ShowDialog(this);
                using CancellationTokenRegistration registration = cancellationToken.Register(
                    () => Dispatcher.UIThread.Post(() =>
                    {
                        if (dialog.IsVisible)
                        {
                            dialog.Close(false);
                        }
                    }));
                bool approved = await approval;
                cancellationToken.ThrowIfCancellationRequested();
                return approved;
            });
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