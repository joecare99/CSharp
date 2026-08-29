using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using NSubstitute;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class ListBoxTests : TestBase
{
    private sealed class SelectionModel : INotifyPropertyChanged
    {
        private object? _selected;
        public event PropertyChangedEventHandler? PropertyChanged;

        public object? Selected
        {
            get => _selected;
            set
            {
                if (ReferenceEquals(value, _selected)) return;
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }
    }

    [TestMethod]
    public void Ctor_Defaults()
    {
        var lb = new ListBox();
        Assert.AreEqual(ConsoleColor.Black, lb.BackColor);
        Assert.AreEqual(ConsoleColor.Gray, lb.ForeColor);
    }

    [TestMethod]
    public void ItemsSource_Set_Empty_Clears_Selection()
    {
        var lb = new ListBox();
        lb.ItemsSource = new object[] { "a", "b" };
        lb.SelectedIndex = 1;

        lb.ItemsSource = Array.Empty<object>();

        Assert.AreEqual(-1, lb.SelectedIndex);
        Assert.IsNull(lb.SelectedItem);
    }

    [TestMethod]
    public void SelectedIndex_Sets_SelectedItem()
    {
        var lb = new ListBox();
        lb.ItemsSource = new object[] { "a", "b", "c" };

        lb.SelectedIndex = 2;

        Assert.AreEqual("c", lb.SelectedItem);
    }

    [TestMethod]
    public void SelectedItem_Sets_SelectedIndex()
    {
        var lb = new ListBox();
        lb.ItemsSource = new object[] { "a", "b", "c" };

        lb.SelectedItem = "b";

        Assert.AreEqual(1, lb.SelectedIndex);
        Assert.AreEqual("b", lb.SelectedItem);
    }

    [TestMethod]
    public void SelectedItem_Set_To_Unknown_Does_Not_Change()
    {
        var lb = new ListBox();
        lb.ItemsSource = new object[] { "a", "b" };
        lb.SelectedIndex = 0;

        lb.SelectedItem = "x";

        Assert.AreEqual(0, lb.SelectedIndex);
        Assert.AreEqual("a", lb.SelectedItem);
    }

    [TestMethod]
    public void BindSelected_InitialValue_Pulls_From_Model()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var model = new SelectionModel { Selected = "b" };

        var lb = new ListBox { ItemsSource = items };
        lb.BindSelected(model, nameof(SelectionModel.Selected));

        Assert.AreEqual("b", lb.SelectedItem);
        Assert.AreEqual(1, lb.SelectedIndex);
    }

    [TestMethod]
    public void BindSelected_Changing_ListBox_Pushes_To_Model()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var model = new SelectionModel();
        var lb = new ListBox { ItemsSource = items };
        lb.BindSelected(model, nameof(SelectionModel.Selected));

        lb.SelectedIndex = 2;

        Assert.AreEqual("c", model.Selected);
    }

    [TestMethod]
    public void BindSelected_Changing_Model_Pulls_Into_ListBox()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var model = new SelectionModel();
        var lb = new ListBox { ItemsSource = items };
        lb.BindSelected(model, nameof(SelectionModel.Selected));

        model.Selected = "a";

        Assert.AreEqual(0, lb.SelectedIndex);
        Assert.AreEqual("a", lb.SelectedItem);
    }

    [TestMethod]
    public void ObservableCollection_Remove_SelectedItem_Clamps_Selection()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var lb = new ListBox { ItemsSource = items };
        lb.SelectedIndex = 2;

        items.RemoveAt(2);

        Assert.AreEqual(1, lb.SelectedIndex);
        Assert.AreEqual("b", lb.SelectedItem);
    }

    [TestMethod]
    public void MouseClick_Selects_Item_By_Row()
    {
        var lb = new ListBox { ItemsSource = new object[] { "a", "b", "c" } };
        lb.Dimension = new Rectangle(0, 0, 10, 3);

        var me = Substitute.For<IMouseEvent>();
        me.MousePos.Returns(new Point(1, 1));
        me.MouseWheel.Returns(0);

        lb.MouseClick(me);

        Assert.AreEqual(1, lb.SelectedIndex);
        Assert.AreEqual("b", lb.SelectedItem);
    }

    [TestMethod]
    public void MouseWheel_Scrolls_TopIndex_And_Selection_Still_Functional()
    {
        var items = new ObservableCollection<string>();
        for (int i = 0; i < 20; i++) items.Add($"i{i}");

        var lb = new ListBox { ItemsSource = items };
        lb.Dimension = new Rectangle(0, 0, 10, 3);

        var me = Substitute.For<IMouseEvent>();
        me.MousePos.Returns(new Point(1, 1));
        me.MouseWheel.Returns(-120); // scroll down => topIndex++

        lb.MouseMove(me, new Point(1, 1));
        lb.MouseMove(me, new Point(1, 1));

        // now click the middle row - should not be the original index 1 anymore
        me.MouseWheel.Returns(0);
        lb.MouseClick(me);

        Assert.IsTrue(lb.SelectedIndex >= 1);
        Assert.AreEqual(items[lb.SelectedIndex], lb.SelectedItem);
    }

    [TestMethod]
    public void Keyboard_JK_Changes_Selection_And_Handled()
    {
        var lb = new ListBox { ItemsSource = new object[] { "a", "b", "c" } };
        lb.SelectedIndex = 0;

        var key = Substitute.For<IKeyEvent>();
        key.bKeyDown.Returns(true);

        key.KeyChar.Returns('J');
        lb.HandlePressKeyEvents(key);
        Assert.AreEqual(1, lb.SelectedIndex);
        Assert.IsTrue(key.Handled);

        key.Handled.Returns(false);
        key.KeyChar.Returns('K');
        lb.HandlePressKeyEvents(key);
        Assert.AreEqual(0, lb.SelectedIndex);
    }

    [TestMethod]
    public void Dispose_Detaches_Binding_No_Further_Model_Updates()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var model = new SelectionModel();
        var lb = new ListBox { ItemsSource = items };
        lb.BindSelected(model, nameof(SelectionModel.Selected));

        lb.Dispose();

        lb.SelectedIndex = 2; // should not push into model anymore
        Assert.IsNull(model.Selected);

        model.Selected = "a"; // should not pull into listbox anymore
        Assert.AreEqual(2, lb.SelectedIndex);
    }
}
