// ***********************************************************************
// Assembly         : AA22_AvlnCap
// Author           : Mir
// Created          : 05-14-2023
// ***********************************************************************
using AA22_AvlnCap.ViewModels.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.MSTest;
using Avalonia.VisualTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;

namespace AA22_AvlnCap.Views.Tests;

[TestClass()]
public class MainWindowTests
{
    MainWindow testView = null!;
    IWpfCapViewModel _vm = null!;

    [TestInitialize]
    public void Init()
    {

        testView = new()
        {
            DataContext = _vm = Substitute.For<IWpfCapViewModel>()
        };


    }

    [AvaloniaTestMethod]
    public void MainWindowTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(MainWindow));    
    }
}
