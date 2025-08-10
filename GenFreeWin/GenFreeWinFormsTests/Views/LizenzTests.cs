using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFreeWin.Views.Tests;

[TestClass()]
public class LizenzTests
{
    private ILizenzViewModel _viewModel;
    private IApplUserTexts _strings;
    private Lizenz _testView;

    [TestInitialize]
    public void Init()
    {
        // Arrange
        _viewModel = Substitute.For<ILizenzViewModel>();
        _strings = Substitute.For<IApplUserTexts>();
        // Act
        _testView = new Lizenz(_viewModel,_strings);
    }

    [TestMethod()]
    public void LizenzTest()
    {
        // Assert
        Assert.IsNotNull(_testView.ViewModel);
        Assert.AreEqual(_viewModel, _testView.ViewModel);
    }

    [TestMethod]
    public void LizenzView_CancelClick()
    {
        _testView.Show();
        _viewModel.CancelCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.CancelCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnCancel.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.CancelCommand.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void LizenzView_VerifyClick()
    {
        _testView.Show();
        _viewModel.VerifyCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.VerifyCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnVerify.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.VerifyCommand.ReceivedWithAnyArgs(1).Execute(null);
    }


}