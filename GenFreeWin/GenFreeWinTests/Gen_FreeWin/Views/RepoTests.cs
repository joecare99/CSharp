using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFreeWin.Views.Tests;

[TestClass()]
public class RepoTests
{
    private IRepoViewModel _viewModel;
    private Repo _testView;

    [TestInitialize]
    public void Init()
    {
        // Arrange
        _viewModel = Substitute.For<IRepoViewModel>();
        _viewModel.Repolist_Items.Returns ( new ObservableCollection<IListItem<int>>() );
        _viewModel.Sources_Items.Returns ( new ObservableCollection<IListItem<int>>() );
        _viewModel.FontSize.Returns( 9.3f);
        // Act
        _testView = new Repo(_viewModel);
    }

    [TestMethod()]
    public void RepoTest()
    {
        // Assert
        Assert.IsNotNull(_testView);
        Assert.IsNotNull(_testView.ViewModel);
        Assert.AreEqual(_viewModel, _testView.ViewModel);
    }

    [TestMethod]
    public void RepoView_SaveClick()
    {
        _testView.Show();
        _viewModel.SaveCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.SaveCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnSave.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.SaveCommand.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void RepoView_Save2Click()
    {
        _testView.Show();
        _viewModel.Save2Command.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.Save2Command.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnSave2.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.Save2Command.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void RepoView_CloseClick()
    {
        _testView.Show();
        _viewModel.CloseCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.CloseCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnClose.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.CloseCommand.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void RepoView_NewEntryClick()
    {
        _testView.Show();
        _viewModel.NewEntryCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.NewEntryCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnNewEntry.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.NewEntryCommand.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void RepoView_DeleteClick()
    {
        _testView.Show();
        _viewModel.DeleteCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.DeleteCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnDelete.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.DeleteCommand.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void RepoView_RepoList_Items()
    {
        //    _testView.Show();
        // Arrange
        _viewModel.Repolist_Items.Add(new ListItem<int>("Item1", 1));
        _viewModel.Repolist_Items.Add(new ListItem<int>("Item2", 2));
        // Act
        // Assert
        Assert.AreEqual(2, _testView.ListBox1.Items.Count);
        CollectionAssert.AreEqual(new List<IListItem<int>> { _viewModel.Repolist_Items[0], _viewModel.Repolist_Items[1] }, _testView.ListBox1.Items);
    }

}