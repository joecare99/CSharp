using System;
using System.ComponentModel;
using ConsoleLib;
using ConsoleLib.CommonControls;
using BaseLib.Interfaces;
using ConsoleLib.Interfaces;
using GenFreeBrowser.Places.Interface; // for PlaceResult type

namespace PlaceAuthorityConsoleDemo;

/// <summary>
/// Console-based View that binds to ConsoleAppViewModel + SearchViewModel.
/// </summary>
public sealed class ConsoleSearchView : Application
{
    private readonly ConsoleAppViewModel _vm;
    private TextBox _queryBox = null!;
    private Label _statusLabel = null!;
    private ListBox _resultsList = null!;
    private Label _historyLabel = null!;

    public ConsoleSearchView(ConsoleAppViewModel vm, IConsole console, IExtendedConsole extendedConsole) : base(console, extendedConsole)
    {
        _vm = vm;
        console.Title = "PlaceAuthority Console Demo (MVVM)";
        console.Clear();
        BuildUi();
    }

    private void BuildUi()
    {
        var root = this;
        BackColor = ConsoleColor.DarkBlue;
        BoarderColor = ConsoleColor.Gray;
        Border = ConsoleFramework.singleBorder;

        new Label { 
            Parent = root, 
            Dimension = new System.Drawing.Rectangle(0, 0, 20, 1), 
            ForeColor = ConsoleColor.Gray, 
            Text = "Query:" 
        };

        _queryBox = new TextBox {
            Parent = root,
            Dimension = new System.Drawing.Rectangle(0, 1, Math.Min(40, Console.WindowWidth - 2), 1),
            MultiLine = false,
            BackColor = ConsoleColor.DarkBlue,
            ForeColor = ConsoleColor.White,
            Binding = (_vm.Search, nameof(_vm.Search.SearchText)),
        };

        _queryBox.Active = true;
        _historyLabel = new Label {
            Parent = root,
            Dimension = new System.Drawing.Rectangle(0, 2, Console.WindowWidth - 2, 1),
            ForeColor = ConsoleColor.DarkCyan,
            Binding = (_vm, nameof(_vm.HistoryBanner))
        };
        new Label { Parent = root, Dimension = new System.Drawing.Rectangle(0, 3, 20, 1), Text = "Results:", ForeColor = ConsoleColor.Gray };

        _resultsList = new ListBox {
            Parent = root,
            Dimension = new System.Drawing.Rectangle(0, 4, Console.WindowWidth - 2, Console.WindowHeight - 6),
            BackColor = ConsoleColor.Black,
            ForeColor = ConsoleColor.Gray,
            ItemsSource = _vm.Search.Results // one-way binding
            SelectedBinding = (_vm.Search, nameof(_vm.Search.SelectedResult)),
        };

        _statusLabel = new Label {
            Parent = root,
            Dimension = new System.Drawing.Rectangle(0, Console.WindowHeight - 1, Console.WindowWidth - 1, 1),
            BackColor = ConsoleColor.DarkGray,
            ForeColor = ConsoleColor.Black,
            Binding = (_vm, nameof(_vm.Status))
        };

        ((INotifyPropertyChanged)_vm.Search).PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(_vm.Search.SelectedResult) && _vm.Search.SelectedResult is not null)
            {
                var r = _vm.Search.SelectedResult;
                _statusLabel.Text = $"Sel: {r.Name} [{r.Source}]";
                _statusLabel.Invalidate();
                // Sync list selection if different
                var idx = _vm.Search.Results.IndexOf(r);
                if (idx >= 0 && _resultsList.SelectedIndex != idx)
                {
                    _resultsList.SelectedIndex = idx;
                }
            }
        };

        this.OnKeyPressed += async (_, e) => {
            switch ((ConsoleKey)e.usKeyCode)
            {
                case ConsoleKey.Enter:
                    if (_vm.DoSearchCommand.CanExecute(null))
                    {
                        await _vm.DoSearchCommand.ExecuteAsync(null);
                        EnsureSelectionAfterSearch();
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (_queryBox.Active)
                    {
                        _vm.HistoryPrevCommand.Execute(null);
                    }
                    else
                    {
                        MoveSelection(-1);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (_queryBox.Active)
                    {
                        _vm.HistoryNextCommand.Execute(null);
                    }
                    else
                    {
                        MoveSelection(1);
                    }
                    break;
                case ConsoleKey.Tab:
                    // Toggle focus between query and results
                    _queryBox.Active = !_queryBox.Active;
                    break;
                case ConsoleKey.Escape:
                    Stop();
                    break;
            }
        };
    }

    private void EnsureSelectionAfterSearch()
    {
        if (_vm.Search.Results.Count > 0)
        {
            _resultsList.SelectedIndex = 0;
            _vm.Search.SelectedResult = _vm.Search.Results[0];
            _queryBox.Active = false; // move focus to list automatically
        }
    }

    private void MoveSelection(int delta)
    {
        if (_vm.Search.Results.Count == 0) return;
        var newIndex = _resultsList.SelectedIndex;
        if (newIndex < 0) newIndex = 0;
        newIndex += delta;
        if (newIndex < 0) newIndex = 0;
        if (newIndex >= _vm.Search.Results.Count) newIndex = _vm.Search.Results.Count - 1;
        _resultsList.SelectedIndex = newIndex;
        _vm.Search.SelectedResult = _vm.Search.Results[newIndex];
    }
}
