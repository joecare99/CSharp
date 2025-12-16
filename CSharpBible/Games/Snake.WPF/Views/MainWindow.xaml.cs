using System.Windows;
using System.Windows.Input;
using Snake_Base.ViewModels;
using Snake_Base.Models.Interfaces;
using Snake.WPF.Services;

namespace Snake.WPF.Views;

public partial class MainWindow : Window
{
    private readonly GameLoopService _loop;
    private readonly ISnakeViewModel _vm;

    public MainWindow(ISnakeViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        _vm = vm;
        _loop = new GameLoopService(vm, this.Dispatcher);
        Loaded += (_, __) => { _loop.Start(); this.Focus(); };
        Closed += (_, __) => _loop.Stop();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        _vm.UserAction = e.Key switch
        {
            Key.Up => UserAction.GoNorth,
            Key.Down => UserAction.GoSouth,
            Key.Left => UserAction.GoWest,
            Key.Right => UserAction.GoEast,
            Key.Escape => UserAction.Quit,
            Key.F1 => UserAction.Help,
            Key.R => UserAction.Restart,
            _ => UserAction.Nop
        };
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        if (!_vm.IsRunning) _loop.Start();
    }

}