using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Config.Service;
using Config.UI;
using Config.UI.Avalonia.Views;
using Config.UI.ConfigService;
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
using Microsoft.Extensions.DependencyInjection;

namespace AA98_AvlnCodeStudio.UI.Views
{
    public partial class MainWindow : Window
    {
        private bool _closeConfirmed;
        private bool _closePromptActive;

        private sealed class ProjectSectionRegistry(params IConfigUiSection[] sections) : IConfigUiSectionRegistry
        {
            public IReadOnlyList<IConfigUiSection> Sections { get; } = sections;
        }

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            Closed += OnWindowClosed;
        }

        private void OnWindowClosed(object? sender, EventArgs e)
        {
            if (DataContext is ViewModels.MainWindowViewModel viewModel)
            {
                viewModel.EditorWorkspace.Dispose();
            }
        }

        private void OnEditorTabHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Control { DataContext: ViewModels.EditorTabViewModel tab }
                && DataContext is ViewModels.MainWindowViewModel viewModel)
            {
                viewModel.EditorWorkspace.Activate(tab);
                tab.ActivateByUser(designerSelection: tab.SelectedProperties is not null);
            }
        }

        private async void OnCloseEditorTabRequested(object? sender, RoutedEventArgs e)
        {
            if (sender is not Control { DataContext: ViewModels.EditorTabViewModel tab }
                || DataContext is not ViewModels.MainWindowViewModel viewModel)
            {
                return;
            }

            if (viewModel.EditorWorkspace.IsSaving(tab))
            {
                viewModel.Editor.StatusText = "Wait for the document save to complete before closing the tab.";
                return;
            }

            bool discard = !tab.IsProtected
                || await new DiscardWorkbenchChangesDialog(tab.Title).ShowDialog<bool>(this);
            if (discard)
            {
                if (!viewModel.EditorWorkspace.TryClose(tab, discardProtected: true))
                {
                    viewModel.Editor.StatusText = "Wait for the document save to complete before closing the tab.";
                }
            }
        }

        private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
        {
            if (_closeConfirmed || DataContext is not ViewModels.MainWindowViewModel viewModel)
            {
                return;
            }

            if (_closePromptActive)
            {
                e.Cancel = true;
                return;
            }

            if (viewModel.EditorWorkspace.HasInProgressSaves)
            {
                e.Cancel = true;
                viewModel.Editor.StatusText = "Wait for document saves to complete before closing the Workbench.";
                return;
            }

            string[] protectedTabs = viewModel.EditorWorkspace.Tabs
                .Where(static tab => tab.IsProtected)
                .Select(static tab => tab.Title)
                .ToArray();
            if (protectedTabs.Length == 0)
            {
                return;
            }

            e.Cancel = true;
            _closePromptActive = true;
            bool discard = await new DiscardWorkbenchChangesDialog(string.Join(Environment.NewLine, protectedTabs))
                .ShowDialog<bool>(this);
            _closePromptActive = false;
            if (discard)
            {
                _closeConfirmed = true;
                Close();
            }
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel)
            : this(viewModel, null)
        {
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IServiceProvider? hostServices)
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
            viewModel.OpenConfigurationLayerDialogAsync = async layer =>
            {
                if (hostServices is null)
                {
                    viewModel.Editor.StatusText = "Layer-specific settings are unavailable because the host services were not provided.";
                    return;
                }

                IConfigLayerStore? targetStore = hostServices.GetServices<IConfigLayerStore>()
                    .SingleOrDefault(store => store.Layer == layer);
                if (targetStore is null)
                {
                    viewModel.Editor.StatusText = layer == ConfigLayer.Group
                        ? "Group settings are not configured. Configure both AVLN_CONFIG_GROUP_ROOT and AVLN_CONFIG_GROUP_ID."
                        : "The system configuration store is not available.";
                    return;
                }

                ConfigLayerStoreAccessResult access = ConfigLayerStoreAccessEvaluator.Evaluate(targetStore);
                ConfigLayeredUiSectionRegistry registry = new(
                    hostServices.GetRequiredService<IConfigSectionRegistry>(),
                    hostServices.GetRequiredService<ConfigLayeredSettingsResolver>(),
                    targetStore,
                    layer,
                    optionsProvider: static (section, property) =>
                    {
                        if (!string.Equals(section.Name, "Workbench", StringComparison.Ordinal)
                            || !string.Equals(property.Name, nameof(Configuration.CodeStudioSettings.CredentialManager), StringComparison.Ordinal))
                        {
                            return null;
                        }

                        List<PropertyOption> options =
                        [
                            new(Configuration.CredentialManagerSelection.None, "None (session only)"),
                            new(Configuration.CredentialManagerSelection.EncryptedFileVault, "Encrypted local vault (passphrase)"),
                        ];
                        if (OperatingSystem.IsWindows())
                        {
                            options.Add(new(Configuration.CredentialManagerSelection.WindowsCredentialManager, "Windows Credential Manager"));
                        }
                        else if (OperatingSystem.IsLinux())
                        {
                            options.Add(new(Configuration.CredentialManagerSelection.LinuxSecretService, "Linux Secret Service"));
                        }

                        return options;
                    },
                    isReadOnly: !access.CanWrite,
                    storageLocation: targetStore is JsonConfigLayerStore jsonStore ? jsonStore.FilePath : null);
                global::Config.UI.Avalonia.ViewModels.ConfigUiViewModel configViewModel = new(
                    registry,
                    new PropertyEditorViewModel(),
                    layer == ConfigLayer.System ? "System Settings" : "Group Settings");
                Configuration.WorkbenchSettingsRuntimeActivator settingsActivator =
                    hostServices.GetRequiredService<Configuration.WorkbenchSettingsRuntimeActivator>();
                configViewModel.ValidateChangesAsync = settingsActivator.ValidateChangesAsync;
                configViewModel.SectionAppliedAsync = settingsActivator.ActivateAsync;

                string layerContext = layer == ConfigLayer.System
                    ? "System values are not treated as enforced policy until operating-system protection is independently verified."
                    : "The configured group ID is used for this layer; group membership is not verified by the workbench.";
                StackPanel hostContent = new() { Spacing = 6 };
                if (!access.CanWrite)
                {
                    hostContent.Children.Add(new TextBlock
                    {
                        Text = access.ReadOnlyReason,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    });
                }

                hostContent.Children.Add(new TextBlock
                {
                    Text = layerContext,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                });

                ConfigUiDialog layerDialog = new(configViewModel)
                {
                    Height = 620,
                    HostContent = hostContent,
                };
                await layerDialog.ShowDialog(this);
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
                        ApproveProjectSettingsChangeAsync(viewModel, workspace, proposal, cancellationToken));
                global::Config.UI.Avalonia.ViewModels.ConfigUiViewModel configViewModel = new(
                    new ProjectSectionRegistry(
                        new PropertyProviderConfigUiSection(
                            "Project",
                            project.Name,
                            provider,
                            $"Project settings from {project.ProjectPath}."),
                        new PropertyProviderConfigUiSection(
                            "LaunchProfiles",
                            "Launch profiles",
                            new ProjectLaunchProfilesProvider(project.ProjectPath),
                            "Read-only project profile inspection. The workbench debugger does not apply these profiles.",
                            sortOrder: 1)),
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

            CSharpWorkspace? workspace = viewModel.Workspace;
            ViewModels.EditorTabViewModel activeTab = viewModel.ActiveEditorTab;
            bool applied = await new WorkspaceFileChangeReviewDialog(
                proposal,
                viewModel.WorkspaceFileChangeService,
                () => ReferenceEquals(DataContext, viewModel)
                    && ReferenceEquals(viewModel.Workspace, workspace)
                    && ReferenceEquals(viewModel.ActiveEditorTab, activeTab)).ShowDialog(this);
            return applied;
        }

        private async Task<bool> ApproveProjectSettingsChangeAsync(
            ViewModels.MainWindowViewModel viewModel,
            CSharpWorkspace workspace,
            WorkspaceFileChangeProposal proposal,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!ReferenceEquals(DataContext, viewModel) || !ReferenceEquals(viewModel.Workspace, workspace))
            {
                throw new InvalidOperationException("The active workspace changed. Close project settings and reopen them for the current workspace.");
            }

            WorkspaceFileChangeReviewDialog dialog = WorkspaceFileChangeReviewDialog.CreateApprovalOnly(proposal);
            Task<bool> review = dialog.ShowDialog(this);
            using CancellationTokenRegistration registration = cancellationToken.Register(
                () => Dispatcher.UIThread.Post(() =>
                {
                    if (dialog.IsVisible)
                    {
                        dialog.Close(false);
                    }
                }));
            bool approved = await review;
            cancellationToken.ThrowIfCancellationRequested();
            if (!approved)
            {
                return false;
            }

            if (!ReferenceEquals(DataContext, viewModel) || !ReferenceEquals(viewModel.Workspace, workspace))
            {
                throw new InvalidOperationException("The active workspace changed during review. No project settings were applied.");
            }

            return true;
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