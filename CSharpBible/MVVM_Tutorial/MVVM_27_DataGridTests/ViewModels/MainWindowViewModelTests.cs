﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace MVVM_27_DataGrid.ViewModels.Tests;

[TestClass()]
public class MainWindowViewModelTests
{
    MainWindowViewModel testModel;

    [TestInitialize]
    public void Init()
    {
        testModel = new();
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }
}