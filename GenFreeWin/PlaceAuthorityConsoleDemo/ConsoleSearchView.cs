using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ConsoleLib;
using ConsoleLib.CommonControls;
using CommunityToolkit.Mvvm.Input;
using BaseLib.Interfaces;
using ConsoleLib.Interfaces;
using System.Threading.Channels;

namespace PlaceAuthorityConsoleDemo;

/// <summary>
/// Console-based View that binds to ConsoleAppViewModel + SearchViewModel.
/// </summary>
public sealed class ConsoleSearchView: Application
{
    private readonly ConsoleAppViewModel _vm;
    private TextBox _queryBox = null!;
    private Label _statusLabel = null!;
    private TextBox _resultsBox = null!;
    private Label _historyLabel = null!;

    public ConsoleSearchView(ConsoleAppViewModel vm,IConsole console,IExtendedConsole extendedConsole):base(console,extendedConsole)
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
            Text = "Query:" };
        
        _queryBox = new TextBox { 
            Parent = root, 
            Dimension = new System.Drawing.Rectangle(0, 1, Math.Min(20, Console.WindowWidth - 2), 1), 
            MultiLine = false, 
            BackColor = ConsoleColor.DarkBlue, 
            ForeColor = ConsoleColor.White,
            Binding = (_vm.Search, nameof(_vm.Search.SearchText)),           
        };
    
        this.OnKeyPressed += async (_, e) =>
        {
            if (e.usKeyCode == (ushort)ConsoleKey.Enter)
            {
                if (_vm.DoSearchCommand.CanExecute(null))
                    await _vm.DoSearchCommand.ExecuteAsync(null);
                _queryBox.SetText(string.Empty);
            }
            else if (e.usKeyCode == (ushort)ConsoleKey.UpArrow)
            {
                _vm.HistoryPrevCommand.Execute(null);
            }
            else if (e.usKeyCode == (ushort)ConsoleKey.DownArrow)
            {
                _vm.HistoryNextCommand.Execute(null);
            }
            else if (e.usKeyCode == (ushort)ConsoleKey.Escape)
            {
                Stop();                 
            }
        };
        
        _queryBox.Active = true;
        _historyLabel = new Label { Parent = root, Position = new System.Drawing.Point(0, 2), size = new System.Drawing.Size(Console.WindowWidth - 1, 1), ForeColor = ConsoleColor.DarkCyan };
        new Label { Parent = root, Position = new System.Drawing.Point(0, 3), size = new System.Drawing.Size(20, 1), Text = "Results:" };
        _resultsBox = new TextBox { Parent = root, Position = new System.Drawing.Point(0, 4), size = new System.Drawing.Size(Console.WindowWidth - 2, Console.WindowHeight - 6), MultiLine = true, BackColor = ConsoleColor.Black, ForeColor = ConsoleColor.Gray };
        _statusLabel = new Label { Parent = root, Position = new System.Drawing.Point(0, Console.WindowHeight - 1), size = new System.Drawing.Size(Console.WindowWidth - 1, 1), BackColor = ConsoleColor.DarkGray, ForeColor = ConsoleColor.Black };

        // Bindings: simple manual assignment on property changed
        ((INotifyPropertyChanged)_vm).PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ConsoleAppViewModel.ResultsText))
            {
                _resultsBox.SetText(_vm.ResultsText);
            }
            else if (e.PropertyName == nameof(ConsoleAppViewModel.Status))
            {
                _statusLabel.Text = _vm.Status; _statusLabel.Invalidate();
            }
            else if (e.PropertyName == nameof(ConsoleAppViewModel.HistoryBanner))
            {
                _historyLabel.Text = TrimToWidth(_vm.HistoryBanner); _historyLabel.Invalidate();
            }
        };
    }

    private static string TrimToWidth(string txt)
        => txt.Length > Console.WindowWidth - 1 ? txt[..(Console.WindowWidth - 1)] : txt;

}
