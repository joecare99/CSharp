using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Gen_FreeWin.Views;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.ComponentModel;
using GenFree.Helper;
using CommunityToolkit.Mvvm.Input;
using GenFree.Interfaces.UI;

namespace GenFreeWin.Views.Tests;

[TestClass]
public class OFBViewTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private IOFBViewModel _viewModel;
    private OFB _testView;
    private IApplUserTexts uText;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize]
    public void Init()
    {
        // Arrange
        _viewModel = Substitute.For<IOFBViewModel>();
        _viewModel.List1_Items = [];
        _viewModel.List2_Items = [];
        _viewModel.List3_Items = [];
        _viewModel.List4_Items = [];
        _viewModel.List50_Items = [];
        _viewModel.List51_Items = [];
        _viewModel.List52_Items = [];

        uText = Substitute.For<IApplUserTexts>();

        // Act
        _testView = new OFB(_viewModel,uText);
    }

    [TestMethod]
    public void OFBView_Constructor_ShouldInitializeViewModel()
    {
        // Arrange
        var viewModel = Substitute.For<IOFBViewModel>();
        viewModel.List1_Items = [];
        viewModel.List2_Items = [];
        viewModel.List3_Items = [];
        viewModel.List4_Items = [];
        viewModel.List50_Items = [];
        viewModel.List51_Items = [];
        viewModel.List52_Items = [];

        var uText = Substitute.For<IApplUserTexts>();
        // Act
        var ofbView = new OFB(viewModel,uText);

        // Assert
        Assert.IsNotNull(ofbView);
        Assert.AreEqual(viewModel, ofbView.ViewModel);
    }
    // Additional tests can be added here to cover more functionality of the OFB view.
    [TestMethod]
    public void OFBView_ShouldHaveCorrectViewModel()
    {
        // Assert
        Assert.IsNotNull(_testView.ViewModel);
        Assert.AreEqual(_viewModel, _testView.ViewModel);
    }

    [TestMethod]
    public void OFBView_List1_Items()
    {
        // Arrange
        _viewModel.List1_Items.Add("Item1");
        _viewModel.List1_Items.Add("Item2");
        _viewModel.Text1_Text = "Test Text";
        // Act
        // Assert
        Assert.AreEqual("Test Text", _testView.ViewModel.Text1_Text);
        CollectionAssert.AreEqual(new List<string> { "Item1", "Item2" }, _testView.List1.Items);
    }

    [TestMethod]
    public void OFBView_List4_Items()
    {
        //    _testView.Show();
        // Arrange
        _viewModel.List4_Items.Add(new ListItem<int>("Item1", 1));
        _viewModel.List4_Items.Add(new ListItem<int>("Item2", 2));
        _viewModel.List4_Visible = true;
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.List4_Visible)));
        _viewModel.Text2_0_Text = "Test Text";
        // Act
        // Assert
        Assert.AreEqual("Test Text", _testView.ViewModel.Text2_0_Text);
        Assert.AreEqual(2, _testView.List4.Items.Count);
        CollectionAssert.AreEqual(new List<IListItem<int>> { _viewModel.List4_Items[0], _viewModel.List4_Items[1] }, _testView.List4.Items);
    }

    [TestMethod]
    public void OFBView_ApplyButtonClick()
    {
        _testView.Show(); 
        _viewModel.ApplyCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.ApplyCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnApply.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.ApplyCommand.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void OFBView_List50AddButtonClick()
    {
        _testView.Show();
        _viewModel.List50_AddCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.List50_AddCommand.CanExecuteChanged += Raise.Event();
        // Act
        _testView.btnList50_Add.PerformClick(); // PerformClick needs the control to be visible and enabled
        // Assert
        _viewModel.List50_AddCommand.ReceivedWithAnyArgs(1).Execute(null);
    }

    [TestMethod]
    public void Text1_EnterShouldFireEvent()
    {
        _testView.Show();
        // Arrange
        _viewModel.Text1_Text = "Test Text";
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.Text1_Text)));
        _viewModel.Text1_KeyEnterCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _testView.Text1.Focus();

        // Act
        SendKeys.SendWait("A{Enter}");

        // Assert
        Assert.AreEqual("A", _viewModel.Text1_Text);
        _viewModel.Text1_KeyEnterCommand.Received(1).Execute(Arg.Any<EventArgs>());
    }

    [TestMethod]
    public void Text2_0_EnterShouldFireEvent()
    {
        _testView.Show();
        // Arrange
        _viewModel.Text2_0_Text = "Test Text";
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.Text2_0_Text)));
        _viewModel.List50_AddCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _testView._Text2_0.Focus();

        // Act
        SendKeys.SendWait("B{Enter}");

        // Assert
        Assert.AreEqual("B", _viewModel.Text2_0_Text);
        _viewModel.List50_AddCommand.Received(1).Execute(Arg.Any<EventArgs>());
    }

    [TestMethod]
    public void Text2_1_EnterShouldFireEvent()
    {
        _testView.Show();
        // Arrange
        _viewModel.Text2_1_Text = "Test Text";
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.Text2_1_Text)));
        _viewModel.List51_AddCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _testView._Text2_1.Focus();

        // Act
        SendKeys.SendWait("C{Enter}");

        // Assert
        Assert.AreEqual("C", _viewModel.Text2_1_Text);
        _viewModel.List51_AddCommand.Received(1).Execute(Arg.Any<EventArgs>());
    }

    [TestMethod]
    public void Text2_2_EnterShouldFireEvent()
    {
        _testView.Show();
        // Arrange
        _viewModel.Text2_2_Text = "Test Text";
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.Text2_2_Text)));
        _viewModel.List52_AddCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _testView._Text2_2.Focus();

        // Act
        SendKeys.SendWait("D{Enter}");

        // Assert
        Assert.AreEqual("D", _viewModel.Text2_2_Text);
        _viewModel.List52_AddCommand.Received(1).Execute(Arg.Any<EventArgs>());
    }

    [TestMethod]
    public void List1_DelShouldFireEvent()
    {
        _testView.Show();
        // Arrange
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.List1_SelectedItem)));
        _viewModel.List1_DblClickCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _viewModel.List1_Items.Add("Select");
        _viewModel.List1_Items.Add("Item");
        _viewModel.List1_Items.Add("and hit");
        _viewModel.List1_Items.Add("[Del]-Button");

        _testView.btnApply.Enabled  = true;
        _testView.List1.Visible = true;
        _testView.List1.Enabled = true;
        _testView.List1.Focus();
        _viewModel.ApplyCommand.When(x=> x.Execute(Arg.Any<object>())).Do(_ => _testView.Hide());
        // Act
        //     while (_testView.Visible)
        //         Application.DoEvents();
        SendKeys.SendWait("{Down}{Delete}");

        // Assert
        _viewModel.List1_DblClickCommand.Received(1).Execute(Arg.Any<EventArgs>());
    }

    [TestMethod]
    public void List50_DelShouldFireEvent()
    {
        _testView.Show();
        // Arrange
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.List50_SelectedItem)));
        _viewModel.List5_0_DblClickCommand.CanExecute(Arg.Any<object>()).Returns(true);
        _testView._List5_0.Visible = true;
        _testView._List5_0.Enabled = true;
        _testView._List5_0.Focus();

        // Act
        SendKeys.SendWait("1{Delete}");

        // Assert
        _viewModel.List5_0_DblClickCommand.Received(1).Execute(Arg.Any<EventArgs>());
    }

    [TestMethod]
    public void Check1_ShouldToggleCheckedState()
    {
        _testView.Show();
        // Arrange
        _viewModel.Check1_Checked = true;
        _viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(_viewModel.Check1_Checked)));
        Assert.IsTrue(_testView.Check1.Checked);
        // Act
        _testView.Check1.Checked = false;
        // Assert
        Assert.IsFalse(_viewModel.Check1_Checked);
    }
}