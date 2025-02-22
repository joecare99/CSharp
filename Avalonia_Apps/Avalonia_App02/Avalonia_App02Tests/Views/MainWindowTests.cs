﻿using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Input;
using Avalonia_App02.ViewModels.Interfaces;
using NSubstitute;

namespace Avalonia_App02.Views.Tests;
[TestClass]
public class MainWindowTests
{
    private ISomeTemplateViewModel? _vm;

    [AvaloniaTestMethod()]
    public void MainWindowTest()
    {
        // Arrange
        var window = new MainWindow()
        {
            Height = 800,
            Width = 1024,
            DataContext = _vm = Substitute.For<ISomeTemplateViewModel>()            
        };
        
        window.Show();

        _vm.ConfigCommand.CanExecute(null).Returns(true);
        _vm.ConfigCommand.CanExecuteChanged += Raise.Event<EventHandler>(_vm.ConfigCommand, EventArgs.Empty);

        // Act
        var view = window.FindControl<SomeTemplateView>("AppView");
        view?.FindControl<Button>("btnConfig")?.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter,RawInputModifiers.None);

        // Assert
        _vm.ConfigCommand.ReceivedWithAnyArgs(3).CanExecute(null);
        _vm.ConfigCommand.ReceivedWithAnyArgs(1).Execute(null);
    
    }
}
