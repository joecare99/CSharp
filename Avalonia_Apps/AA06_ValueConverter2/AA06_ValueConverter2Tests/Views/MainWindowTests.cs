using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Input;
using AA06_ValueConverter2.ViewModels.Interfaces;
using NSubstitute;

namespace AA06_ValueConverter2.Views.Tests;
[TestClass]
public class MainWindowTests
{
    private IValueConverterViewModel? _vm;

    [AvaloniaTestMethod()]
    public void MainWindowTest()
    {
        // Arrange
        var window = new MainWindow()
        {
            Height = 800,
            Width = 1024,
            DataContext = _vm = Substitute.For<IValueConverterViewModel>()            
        };
        
        window.Show();

        _vm.ConfigCommand.CanExecute(null).Returns(true);
        _vm.ConfigCommand.CanExecuteChanged += Raise.Event<EventHandler>(_vm.ConfigCommand, EventArgs.Empty);

        // Act
        var view = window.FindControl<ValueConverterView>("AppView");
        view?.FindControl<Button>("btnConfig")?.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter,RawInputModifiers.None);

        // Assert
        _vm.ConfigCommand.ReceivedWithAnyArgs(3).CanExecute(null);
        _vm.ConfigCommand.ReceivedWithAnyArgs(1).Execute(null);
    
    }
}
