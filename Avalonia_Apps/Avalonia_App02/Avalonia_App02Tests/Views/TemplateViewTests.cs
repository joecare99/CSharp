using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Input;
using Avalonia_App02.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using NSubstitute;

namespace Avalonia_App02.Views.Tests;
[TestClass]
public class TemplateViewTests
{
    private ISomeTemplateViewModel? _vm;

    [AvaloniaTestMethod()]
    [DataRow("btnHome", nameof(ISomeTemplateViewModel.HomeCommand))]
    [DataRow("btnProcess", nameof(ISomeTemplateViewModel.ProcessCommand))]
    [DataRow("btnActions", nameof(ISomeTemplateViewModel.ActionsCommand))]
    [DataRow("btnMacros", nameof(ISomeTemplateViewModel.MacrosCommand))]
    [DataRow("btnReports", nameof(ISomeTemplateViewModel.ReportsCommand))]
    [DataRow("btnHistory", nameof(ISomeTemplateViewModel.HistoryCommand))]
    [DataRow("btnConfig", nameof(ISomeTemplateViewModel.ConfigCommand))]
    public void TemplateViewTest(string sAct,string sRelayCmd)
    {
        // Arrange
        SomeTemplateView tv;
        var window = new Window()
        {
            Height = 800,
            Width = 1024,
            Content = tv = new SomeTemplateView()
            {
                DataContext = _vm = Substitute.For<ISomeTemplateViewModel>()
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
