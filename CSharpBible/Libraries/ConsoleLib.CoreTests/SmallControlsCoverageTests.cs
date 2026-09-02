using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace ConsoleLibTests;

[TestClass]
public class SmallControlsCoverageTests
{
    private sealed class KeyEvent : IKeyEvent
    {
        public bool bKeyDown { get; set; }
        public char KeyChar { get; set; }
        public ushort usKeyCode { get; set; }
        public ushort usScanCode { get; set; }
        public uint dwControlKeyState { get; set; }
        public bool Handled { get; set; }
    }

    [TestMethod]
    public void ComboBox_SelectsItemsAndHandlesArrowKeys()
    {
        var combo = new ComboBox { Active = true };
        combo.Items.Add("One");
        combo.Items.Add("Two");

        Assert.IsFalse(combo.SelectPrevious());
        Assert.IsTrue(combo.SelectNext());
        Assert.AreEqual(0, combo.SelectedIndex);
        Assert.AreEqual("One", combo.SelectedItem);
        Assert.IsTrue(combo.SelectNext());
        Assert.IsFalse(combo.SelectNext());

        var up = new KeyEvent { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.UpArrow };
        combo.HandlePressKeyEvents(up);
        Assert.AreEqual(0, combo.SelectedIndex);
        Assert.IsTrue(up.Handled);

        var down = new KeyEvent { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.DownArrow };
        combo.HandlePressKeyEvents(down);
        Assert.AreEqual(1, combo.SelectedIndex);
        Assert.IsTrue(down.Handled);
    }

    [TestMethod]
    public void ComboBox_IgnoresInputWhenInactiveOrKeyIsUp()
    {
        var combo = new ComboBox();
        combo.Items.Add("One");
        var inactive = new KeyEvent { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.DownArrow };
        combo.HandlePressKeyEvents(inactive);
        Assert.AreEqual(-1, combo.SelectedIndex);
        Assert.IsFalse(inactive.Handled);

        combo.Active = true;
        var keyUp = new KeyEvent { bKeyDown = false, usKeyCode = (ushort)ConsoleKey.DownArrow };
        combo.HandlePressKeyEvents(keyUp);
        Assert.AreEqual(-1, combo.SelectedIndex);
        Assert.IsFalse(keyUp.Handled);
    }

    [TestMethod]
    public void TabControl_SelectsItemsAndHandlesArrowKeys()
    {
        var tabs = new TabControl { Active = true };
        tabs.Items.Add(new TabItem("First"));
        tabs.Items.Add(new TabItem("Second"));

        Assert.IsFalse(tabs.SelectPrevious());
        Assert.IsTrue(tabs.SelectNext());
        Assert.AreEqual("First", tabs.SelectedItem!.Header);

        var right = new KeyEvent { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.RightArrow };
        tabs.HandlePressKeyEvents(right);
        Assert.AreEqual(1, tabs.SelectedIndex);
        Assert.IsTrue(right.Handled);

        var left = new KeyEvent { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.LeftArrow };
        tabs.HandlePressKeyEvents(left);
        Assert.AreEqual(0, tabs.SelectedIndex);
        Assert.IsTrue(left.Handled);
        Assert.Throws<ArgumentNullException>(() => new TabItem(null!));
    }

    [TestMethod]
    public void CheckBox_TogglesOnSpaceAndEnterAndRaisesClick()
    {
        var checkBox = new CheckBox { Active = true };
        var clicks = 0;
        checkBox.OnClick += (_, _) => clicks++;

        var space = new KeyEvent { bKeyDown = true, KeyChar = ' ' };
        checkBox.HandlePressKeyEvents(space);
        Assert.IsTrue(checkBox.IsChecked);
        Assert.IsTrue(space.Handled);
        Assert.AreEqual(1, clicks);

        var enter = new KeyEvent { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.Enter };
        checkBox.HandlePressKeyEvents(enter);
        Assert.IsFalse(checkBox.IsChecked);
        Assert.IsTrue(enter.Handled);
        Assert.AreEqual(2, clicks);
    }

    [TestMethod]
    public void CheckBox_DoesNotToggleWhenDisabledOrKeyIsUp()
    {
        var checkBox = new CheckBox { Active = true, Enabled = false };
        var disabled = new KeyEvent { bKeyDown = true, KeyChar = ' ' };
        checkBox.HandlePressKeyEvents(disabled);
        Assert.IsFalse(checkBox.IsChecked);
        Assert.IsFalse(disabled.Handled);

        checkBox.Enabled = true;
        var keyUp = new KeyEvent { bKeyDown = false, KeyChar = ' ' };
        checkBox.HandlePressKeyEvents(keyUp);
        Assert.IsFalse(checkBox.IsChecked);
        Assert.IsFalse(keyUp.Handled);
    }

    [TestMethod]
    public void RadioButton_SelectsItselfAndClearsSibling()
    {
        var panel = new Panel();
        var first = new RadioButton { Parent = panel, Active = true };
        var second = new RadioButton { Parent = panel, Active = true };
        first.Select();
        Assert.IsTrue(first.IsChecked);
        second.Select();
        Assert.IsFalse(first.IsChecked);
        Assert.IsTrue(second.IsChecked);

        first.Active = true;
        var key = new KeyEvent { bKeyDown = true, KeyChar = ' ' };
        first.HandlePressKeyEvents(key);
        Assert.IsTrue(first.IsChecked);
        Assert.IsFalse(second.IsChecked);
        Assert.IsTrue(key.Handled);
    }

    [TestMethod]
    public void StackPanel_ArrangesVerticalAndHorizontalChildren()
    {
        var panel = new StackPanel { Dimension = new Rectangle(0, 0, 10, 6), Spacing = 1 };
        var first = new Label { size = new Size(4, 2) };
        var second = new Label { size = new Size(3, 2) };
        panel.Add(first);
        panel.Add(second);
        panel.Dimension = new Rectangle(0, 0, 10, 6);

        Assert.AreEqual(new Rectangle(0, 0, 10, 2), first.Dimension);
        Assert.AreEqual(new Rectangle(0, 3, 10, 2), second.Dimension);

        first.size = new Size(4, 2);
        second.size = new Size(3, 2);
        panel.Orientation = Orientation.Horizontal;
        panel.HorizontalContentAlignment = HorizontalAlignment.Center;
        panel.VerticalContentAlignment = VerticalAlignment.Center;
        Assert.AreEqual(2, panel.Children.Count);
        Assert.AreEqual(Orientation.Horizontal, panel.Orientation);

        panel.Remove(first);
        Assert.AreEqual(1, panel.Children.Count);
        ((IGroupControl)panel).BringToFront(second);
    }
}
