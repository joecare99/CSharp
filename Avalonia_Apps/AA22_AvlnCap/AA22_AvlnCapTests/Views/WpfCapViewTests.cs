// ***********************************************************************
// Assembly         : AA22_AvlnCap
// Author           : Mir
// Created          : 05-14-2023
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using System;
using System.Threading;
using NSubstitute;
using AA22_AvlnCap.ViewModels.Interfaces;

namespace AA22_AvlnCap.Views.Tests;

[TestClass()]
public class WpfCapViewTests
{
    WpfCapView testView = null!;

    IWpfCapViewModel _vm;

    [TestInitialize]
    public void Init()
    {
        testView = new()
        {
            DataContext = _vm = Substitute.For<IWpfCapViewModel>()
        };
    }

    [AvaloniaTestMethod]
    public void WpfCapViewTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(WpfCapView));    
    }
}
