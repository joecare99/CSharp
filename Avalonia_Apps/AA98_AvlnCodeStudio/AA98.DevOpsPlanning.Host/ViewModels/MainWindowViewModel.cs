using AA98.DevOpsPlanning.Host.Commands;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using PlanningExplorerViewModel = AA98_AvlnCodeStudio.Planning.UI.ViewModels.PlanningExplorerViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.ViewModels;

/// <summary>
/// Provides the root view model for the DevOps planning micro host.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly PlanningExplorerViewModel _planningExplorerViewModel;
    private readonly IReadOnlyList<IDiagnosticConsumer> _diagnosticConsumers;
    private readonly IDevOpsPlanningCommandContext _commandContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="planningExplorerViewModel">The reusable planning explorer view model.</param>
    /// <param name="commandContributions">The registered workbench command contributions.</param>
    /// <param name="services">The host service provider.</param>
    public MainWindowViewModel(
        PlanningExplorerViewModel planningExplorerViewModel,
        DiagnosticCollectionViewModel diagnosticCollectionViewModel,
        IEnumerable<IDiagnosticConsumer> diagnosticConsumers,
        IEnumerable<IWorkbenchCommandContribution> commandContributions,
        IServiceProvider services)
    {
        _planningExplorerViewModel = planningExplorerViewModel ?? throw new ArgumentNullException(nameof(planningExplorerViewModel));
        DiagnosticCollection = diagnosticCollectionViewModel ?? throw new ArgumentNullException(nameof(diagnosticCollectionViewModel));
        _diagnosticConsumers = (diagnosticConsumers ?? throw new ArgumentNullException(nameof(diagnosticConsumers))).ToList();

        _commandContext = new DevOpsPlanningCommandContext(
            services,
            () => PathSelection,
            () => PathSelectionIsPlanningProject,
            (pathSelection, pathSelectionIsPlanningProject) =>
            {
                PathSelection = pathSelection;
                PathSelectionIsPlanningProject = pathSelectionIsPlanningProject;
            },
            LoadAsync,
            statusText => StatusText = statusText);

        string detectedRepositoryRootPath = ResolveRepositoryRootPath();
        PathSelection = detectedRepositoryRootPath;

        LoadFileMenuCommands(commandContributions ?? []);

        StatusText = "Loading planning items...";
        RefreshCommand.Execute(null);
    }

    /// <summary>
    /// Gets the reusable planning explorer composition model.
    /// </summary>
    public PlanningExplorerViewModel PlanningExplorer => _planningExplorerViewModel;

    /// <summary>
    /// Gets the file-menu command entries discovered from command contributions.
    /// </summary>
    public ObservableCollection<FileMenuCommandViewModel> FileMenuCommands { get; } = [];

    /// <summary>
    /// Gets the command for opening a solution root path.
    /// </summary>
    public IRelayCommand? OpenSolutionRootCommand { get; private set; }

    /// <summary>
    /// Gets the command for opening a DevOps planning project path.
    /// </summary>
    public IRelayCommand? OpenPlanningProjectCommand { get; private set; }

    /// <summary>
    /// Gets the command for creating a new DevOps planning project.
    /// </summary>
    public IRelayCommand? NewPlanningProjectCommand { get; private set; }

    /// <summary>
    /// Gets the diagnostics UI composition model.
    /// </summary>
    public DiagnosticCollectionViewModel DiagnosticCollection { get; }

    [ObservableProperty]
    private string _repositoryRootPath = string.Empty;

    [ObservableProperty]
    private string _planningRootPath = string.Empty;

    [ObservableProperty]
    private string _pathSelection = string.Empty;

    [ObservableProperty]
    private bool _pathSelectionIsPlanningProject;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [RelayCommand]
    private async Task ApplyPathSelectionAsync()
    {
        await LoadAsync(CancellationToken.None);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync(CancellationToken.None);
    }

    private void LoadFileMenuCommands(IEnumerable<IWorkbenchCommandContribution> contributions)
    {
        FileMenuCommands.Clear();
        OpenSolutionRootCommand = null;
        OpenPlanningProjectCommand = null;
        NewPlanningProjectCommand = null;

        List<IWorkbenchCommandContribution> fileContributions = contributions
            .Where(static contribution => contribution.Descriptor.Placements.Any(static placement =>
                placement.Surface == WorkbenchCommandSurface.Menu &&
                placement.PathSegments.Count > 0 &&
                string.Equals(placement.PathSegments[0], "Datei", StringComparison.OrdinalIgnoreCase)))
            .OrderBy(static contribution => contribution.Descriptor.Placements
                .Where(static placement => placement.Surface == WorkbenchCommandSurface.Menu)
                .Select(static placement => placement.Order)
                .DefaultIfEmpty(0)
                .Min())
            .ToList();

        foreach (IWorkbenchCommandContribution contribution in fileContributions)
        {
            IRelayCommand relayCommand = new RelayCommand(() => _ = ExecuteFileContributionAsync(contribution));

            switch (contribution.Descriptor.CommandId)
            {
                case "Planning.File.OpenSolutionRoot":
                    OpenSolutionRootCommand = relayCommand;
                    break;
                case "Planning.File.OpenPlanningProject":
                    OpenPlanningProjectCommand = relayCommand;
                    break;
                case "Planning.File.NewPlanningProject":
                    NewPlanningProjectCommand = relayCommand;
                    break;
            }

            FileMenuCommands.Add(new FileMenuCommandViewModel(contribution.Descriptor.DisplayTitle, relayCommand));
        }

        OnPropertyChanged(nameof(OpenSolutionRootCommand));
        OnPropertyChanged(nameof(OpenPlanningProjectCommand));
        OnPropertyChanged(nameof(NewPlanningProjectCommand));
    }

    private async Task ExecuteFileContributionAsync(IWorkbenchCommandContribution contribution)
    {
        if (!contribution.CanExecute(_commandContext))
        {
            return;
        }

        await contribution.ExecuteAsync(_commandContext, CancellationToken.None);
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        PlanningReadRequest request = CreateReadRequest();
        await _planningExplorerViewModel.LoadAsync(request, cancellationToken);

        foreach (IDiagnosticConsumer diagnosticConsumer in _diagnosticConsumers)
        {
            await diagnosticConsumer.ConsumeAsync(_planningExplorerViewModel.Diagnostics, cancellationToken);
        }

        RepositoryRootPath = _planningExplorerViewModel.RepositoryRootPath;
        PlanningRootPath = _planningExplorerViewModel.PlanningRootPath;
        StatusText = _planningExplorerViewModel.StatusText;
    }

    private PlanningReadRequest CreateReadRequest()
    {
        string normalizedPathSelection = string.IsNullOrWhiteSpace(PathSelection)
            ? string.Empty
            : PathSelection.Trim().Trim('"', '\'');

        string selectedPath = string.IsNullOrWhiteSpace(normalizedPathSelection)
            ? ResolveRepositoryRootPath()
            : Path.GetFullPath(normalizedPathSelection);

        bool isPlanningProjectSelection = PathSelectionIsPlanningProject;
        if (File.Exists(selectedPath))
        {
            string extension = Path.GetExtension(selectedPath);
            if (string.Equals(extension, ".shproj", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(extension, ".projitems", StringComparison.OrdinalIgnoreCase))
            {
                selectedPath = Path.GetDirectoryName(selectedPath) ?? selectedPath;
                isPlanningProjectSelection = true;
            }
        }

        if (!isPlanningProjectSelection)
        {
            return new PlanningReadRequest
            {
                RepositoryRootPath = selectedPath,
                PlanningRootPath = "DevOps",
            };
        }

        DirectoryInfo? parent = Directory.GetParent(selectedPath);
        if (parent is null)
        {
            return new PlanningReadRequest
            {
                RepositoryRootPath = selectedPath,
                PlanningRootPath = ".",
            };
        }

        return new PlanningReadRequest
        {
            RepositoryRootPath = parent.FullName,
            PlanningRootPath = Path.GetRelativePath(parent.FullName, selectedPath),
        };
    }

    private static string ResolveRepositoryRootPath()
    {
        DirectoryInfo? current = new(Path.GetFullPath(Directory.GetCurrentDirectory()));

        while (current is not null)
        {
            string devOpsPath = Path.Combine(current.FullName, "DevOps");
            if (Directory.Exists(devOpsPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return Path.GetFullPath(Directory.GetCurrentDirectory());
    }
}
