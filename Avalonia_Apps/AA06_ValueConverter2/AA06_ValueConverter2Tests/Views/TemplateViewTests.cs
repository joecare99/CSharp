using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Input;
using AA06_ValueConverter2.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using NSubstitute;

namespace AA06_ValueConverter2.Views.Tests;
[TestClass]
public class TemplateViewTests
{
    private IValueConverterViewModel? _vm;

    [AvaloniaTestMethod()]
    [DataRow("btnHome", nameof(IValueConverterViewModel.HomeCommand))]
    [DataRow("btnProcess", nameof(IValueConverterViewModel.ProcessCommand))]
    [DataRow("btnActions", nameof(IValueConverterViewModel.ActionsCommand))]
    [DataRow("btnMacros", nameof(IValueConverterViewModel.MacrosCommand))]
    [DataRow("btnReports", nameof(IValueConverterViewModel.ReportsCommand))]
    [DataRow("btnHistory", nameof(IValueConverterViewModel.HistoryCommand))]
    [DataRow("btnConfig", nameof(IValueConverterViewModel.ConfigCommand))]
    public void TemplateViewTest(string sAct,string sRelayCmd)
    {
        // Arrange
        ValueConverterView tv;
        var window = new Window()
        {
            Height = 800,
            Width = 1024,
            Content = tv = new ValueConverterView()
            {
                DataContext = _vm = Substitute.For<IValueConverterViewModel>()
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
