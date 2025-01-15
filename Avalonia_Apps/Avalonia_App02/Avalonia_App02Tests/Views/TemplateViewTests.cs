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
    private ITemplateViewModel _vm;

    [AvaloniaTestMethod()]
    [DataRow("btnHome", nameof(ITemplateViewModel.HomeCommand))]
    [DataRow("btnProcess", nameof(ITemplateViewModel.ProcessCommand))]
    [DataRow("btnActions", nameof(ITemplateViewModel.ActionsCommand))]
    [DataRow("btnMacros", nameof(ITemplateViewModel.MacrosCommand))]
    [DataRow("btnReports", nameof(ITemplateViewModel.ReportsCommand))]
    [DataRow("btnHistory", nameof(ITemplateViewModel.HistoryCommand))]
    [DataRow("btnConfig", nameof(ITemplateViewModel.ConfigCommand))]
    public void TemplateViewTest(string sAct,string sRelayCmd)
    {
        // Arrange
        TemplateView tv;
        var window = new Window()
        {
            Height = 800,
            Width = 1024,
            Content = tv = new TemplateView()
            {
                DataContext = _vm = Substitute.For<ITemplateViewModel>()
            }
        };

        if (_vm.GetType().GetProperty(sRelayCmd).GetValue(_vm) is IRelayCommand iRc)
        {  
            iRc.CanExecute(null).Returns(true);
            iRc.CanExecuteChanged += Raise.Event<EventHandler>(_vm.HomeCommand, EventArgs.Empty);
        
        
        window.Show();

        // Act
        tv.FindControl<Button>(sAct).Focus();
        window.KeyPressQwerty(PhysicalKey.Enter,RawInputModifiers.None);

        // Assert
        iRc.ReceivedWithAnyArgs(3).CanExecute(null);
        iRc.ReceivedWithAnyArgs(1).Execute(null);
        }
    }
}
