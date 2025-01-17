using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Input;
using AA05_CommandParCalc.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using NSubstitute;
using AA05_CommandParCalc.Data;

namespace AA05_CommandParCalc.Views.Tests;

[TestClass]
public class CommandParCalculatorViewTests
{
    private ICommandParCalculatorViewModel? _vm;

    [AvaloniaTestMethod()]
    [DataRow("btn0", nameof(ICommandParCalculatorViewModel.NumberCommand),ENumbers._0,21)]
    [DataRow("btn1", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._1, 21)]
    [DataRow("btn2", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._2, 21)]
    [DataRow("btn3", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._3, 21)]
    [DataRow("btn4", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._4, 21)]
    [DataRow("btn5", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._5, 21)]
    [DataRow("btn6", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._6, 21)]
    [DataRow("btn7", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._7, 21)]
    [DataRow("btn8", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._8, 21)]
    [DataRow("btn9", nameof(ICommandParCalculatorViewModel.NumberCommand), ENumbers._9, 21)]
    [DataRow("btnPlus", nameof(ICommandParCalculatorViewModel.OperatorCommand),EOperations.Add,29)]
    [DataRow("btnMinus", nameof(ICommandParCalculatorViewModel.OperatorCommand),EOperations.Subtract,29)]
    [DataRow("btnMul", nameof(ICommandParCalculatorViewModel.OperatorCommand),EOperations.Multiply,29)]
    [DataRow("btnDiv", nameof(ICommandParCalculatorViewModel.OperatorCommand),EOperations.Divide,29)]
    [DataRow("btnC_CE", nameof(ICommandParCalculatorViewModel.CalculatorCommand),ECommands.ClearAll,25)]
    public void CommandParCalcViewTest(string sAct, string sRelayCmd, Enum xEnum,int iCanExCalls)
    {
        // Arrange
        CommandParCalculatorView tv;
        var window = new Window()
        {
            Height = 800,
            Width = 1024,
            Content = tv = new CommandParCalculatorView()
            {
                DataContext = _vm = Substitute.For<ICommandParCalculatorViewModel>()
            }
        };

        if (_vm?.GetType().GetProperty(sRelayCmd)?.GetValue(_vm) is IRelayCommand iRc)
        {
            iRc.CanExecute(xEnum).Returns(true);
            iRc.CanExecuteChanged += Raise.Event<EventHandler>(_vm, EventArgs.Empty);

            window.Show();

            // Act
            tv.FindControl<Button>(sAct)?.Focus();
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

            // Assert
            iRc.ReceivedWithAnyArgs(iCanExCalls).CanExecute(null);
            iRc.ReceivedWithAnyArgs(1).Execute(null);
            iRc.Received(1).Execute(xEnum);
        }
    }
}
