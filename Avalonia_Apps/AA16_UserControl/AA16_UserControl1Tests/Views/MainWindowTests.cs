using AA16_UserControl1.ViewModels.Interfaces;
using Avalonia.Headless.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;

namespace AA16_UserControl1.Views.Tests;

[TestClass()]
public class MainWindowTests
{
    [AvaloniaTestMethod]
    public void MainWindowTest()
    {
        MainWindow? mw = new()
        {
            Height = 600,
            Width = 800,
            DataContext = Substitute.For<IMainWindowViewModel>()
        };
        Assert.IsNotNull(mw);
        Assert.IsInstanceOfType(mw, typeof(MainWindow));
    }
}