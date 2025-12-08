using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Input;
using AA05_CommandParCalc.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using NSubstitute;

namespace AA05_CommandParCalc.Views.Tests;
[TestClass]
public class CommandParCalcViewTests
{
    private ICommandParCalcViewModel? _vm;

    [AvaloniaTestMethod()]
    [DataRow("btnHome", nameof(ICommandParCalcViewModel.HomeCommand))]
    [DataRow("btnProcess", nameof(ICommandParCalcViewModel.ProcessCommand))]
    [DataRow("btnActions", nameof(ICommandParCalcViewModel.ActionsCommand))]
    [DataRow("btnMacros", nameof(ICommandParCalcViewModel.MacrosCommand))]
    [DataRow("btnReports", nameof(ICommandParCalcViewModel.ReportsCommand))]
    [DataRow("btnHistory", nameof(ICommandParCalcViewModel.HistoryCommand))]
    [DataRow("btnConfig", nameof(ICommandParCalcViewModel.ConfigCommand))]
    public void CommandParCalcViewTest(string sAct,string sRelayCmd)
    {
        // Arrange
        CommandParCalcView tv;
        var window = new Window()
        {
            Height = 800,
            Width = 1024,
            Content = tv = new CommandParCalcView()
            {
                DataContext = _vm = Substitute.For<ICommandParCalcViewModel>()
            }
        };

        if (_vm?.GetType().GetProperty(sRelayCmd)?.GetValue(_vm) is IRelayCommand iRc)
        {  
            iRc.CanExecute(null).Returns(true);
            iRc.CanExecuteChanged += Raise.Event<EventHandler>(_vm.HomeCommand, EventArgs.Empty);
        
        
        window.Show();

        // Act
        tv.FindControl<Button>(sAct)?.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter,RawInputModifiers.None);

        // Assert
        iRc.ReceivedWithAnyArgs(3).CanExecute(null);
        iRc.ReceivedWithAnyArgs(1).Execute(null);
        }
    }
}
