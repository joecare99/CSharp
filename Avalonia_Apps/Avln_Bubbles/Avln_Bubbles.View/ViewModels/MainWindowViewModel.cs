using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln_Bubbles.View.Services;

namespace Avln_Bubbles.View.ViewModels;

/// <summary>
/// Coordinates the main Bubbles shell and current game session.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IGameSessionFactory _gameSessionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="gameSessionFactory">Creates board sessions.</param>
    /// <param name="hostFeatures">Describes the active host capabilities.</param>
    public MainWindowViewModel(IGameSessionFactory gameSessionFactory, IHostFeatureDescriptor hostFeatures)
    {
        _gameSessionFactory = gameSessionFactory;
        HostFeatures = hostFeatures;
        Title = "Bubbles on Avalonia";
        StatusText = "Start a new game to begin.";
        StartNewGame();
    }

    /// <summary>
    /// Gets the host feature descriptor.
    /// </summary>
    public IHostFeatureDescriptor HostFeatures { get; }

    /// <summary>
    /// Gets or sets the application title.
    /// </summary>
    [ObservableProperty]
    private string _title;

    /// <summary>
    /// Gets or sets the current status text.
    /// </summary>
    [ObservableProperty]
    private string _statusText;

    /// <summary>
    /// Gets or sets the current board.
    /// </summary>
    [ObservableProperty]
    private BubblesBoardViewModel? _board;

    /// <summary>
    /// Gets or sets the column count.
    /// </summary>
    [ObservableProperty]
    private int _columnCount = 11;

    /// <summary>
    /// Gets or sets the row count.
    /// </summary>
    [ObservableProperty]
    private int _rowCount = 11;

    /// <summary>
    /// Gets a value indicating whether desktop windowing is available.
    /// </summary>
    public bool IsDesktopHost => HostFeatures.SupportsDesktopWindowing;

    /// <summary>
    /// Gets a value indicating whether browser hosting is active.
    /// </summary>
    public bool IsBrowserHost => HostFeatures.SupportsBrowserHosting;

    /// <summary>
    /// Gets the total score of the current board.
    /// </summary>
    public int Score => Board?.Score ?? 0;

    /// <summary>
    /// Gets the score of the currently highlighted union.
    /// </summary>
    public int UnionScore => Board?.UnionScore ?? 0;

    /// <summary>
    /// Starts a new game.
    /// </summary>
    [RelayCommand]
    private void StartNewGame()
    {
        if (Board is not null)
        {
            Board.PropertyChanged -= OnBoardPropertyChanged;
            Board.GameOverReached -= OnBoardGameOverReached;
        }

        var board = _gameSessionFactory.CreateBoard(ColumnCount, RowCount);
        board.FillTable();
        board.PropertyChanged += OnBoardPropertyChanged;
        board.GameOverReached += OnBoardGameOverReached;
        Board = board;
        StatusText = $"Running on {HostFeatures.HostLabel}. Remote-ready composition prepared.";
        RaiseBoardDependentProperties();
    }

    private void OnBoardPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(BubblesBoardViewModel.Score) or nameof(BubblesBoardViewModel.UnionScore) or nameof(BubblesBoardViewModel.GameOver))
        {
            RaiseBoardDependentProperties();
        }
    }

    private void OnBoardGameOverReached()
    {
        StatusText = $"Game over on {HostFeatures.HostLabel}. Start a new round or connect a future remote host.";
        RaiseBoardDependentProperties();
    }

    private void RaiseBoardDependentProperties()
    {
        OnPropertyChanged(nameof(Score));
        OnPropertyChanged(nameof(UnionScore));
        OnPropertyChanged(nameof(IsDesktopHost));
        OnPropertyChanged(nameof(IsBrowserHost));
    }
}
