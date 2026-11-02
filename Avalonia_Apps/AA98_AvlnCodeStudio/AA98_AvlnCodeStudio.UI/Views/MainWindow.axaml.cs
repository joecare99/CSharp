using Avalonia.Controls;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Workspaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Views
{
    public partial class MainWindow : Window
    {
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