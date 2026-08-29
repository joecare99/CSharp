using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public sealed class KeyboardInteractionTests
{
    [TestMethod]
    [DataRow((ushort)ConsoleKey.Spacebar, ' ')]
    [DataRow((ushort)ConsoleKey.Enter, '\r')]
    public void FocusedButton_InvokesClickForSpaceAndEnter(ushort keyCode, char keyChar)
    {
        var button = new Button { Active = true };
        var clickCount = 0;
        button.OnClick += (_, _) => clickCount++;

        var key = new KeyEventStub(keyCode, keyChar);
        button.HandlePressKeyEvents(key);

        Assert.AreEqual(1, clickCount);
        Assert.IsTrue(key.Handled);
    }

    [TestMethod]
    public void SingleLineTextBox_RaisesEnterTrigger()
    {
        var textBox = new TextBox { Active = true, MultiLine = false };
        var entered = false;
        textBox.OnEnterKey += (_, _) => entered = true;

        var key = new KeyEventStub((ushort)ConsoleKey.Enter, '\r');
        textBox.HandlePressKeyEvents(key);

        Assert.IsTrue(entered);
        Assert.IsTrue(key.Handled);
    }

    [TestMethod]
    public void SingleLineTextBox_BubblesEnterToSiblingAccelerator()
    {
        var panel = new Panel();
        var textBox = new TextBox { Parent = panel, Active = true, MultiLine = false };
        var button = new Button { Parent = panel, Accelerator = '\r' };
        var clickCount = 0;
        button.OnClick += (_, _) => clickCount++;

        var key = new KeyEventStub((ushort)ConsoleKey.Enter, '\r');
        textBox.HandlePressKeyEvents(key);

        Assert.AreEqual(1, clickCount);
        Assert.IsTrue(key.Handled);
    }

    [TestMethod]
    public void MultiLineTextBox_KeepsEnterForNewLine()
    {
        var textBox = new TextBox { Active = true, MultiLine = true };

        textBox.HandlePressKeyEvents(new KeyEventStub((ushort)ConsoleKey.Enter, '\r'));

        Assert.AreEqual("\n", textBox.Text);
    }

    [TestMethod]
    public void ListBox_UsesNavigationKeysAndKeepsSelectionVisible()
    {
        var list = new ListBox
        {
            Active = true,
            Dimension = new Rectangle(0, 0, 10, 2),
            ItemsSource = new object[] { "a", "b", "c", "d" }
        };

        list.HandlePressKeyEvents(new KeyEventStub((ushort)ConsoleKey.DownArrow, '\0'));
        list.HandlePressKeyEvents(new KeyEventStub((ushort)ConsoleKey.End, '\0'));
        list.HandlePressKeyEvents(new KeyEventStub((ushort)ConsoleKey.Home, '\0'));

        Assert.AreEqual(0, list.SelectedIndex);
        Assert.AreEqual(0, list.GetTopIndex());
    }

    [TestMethod]
    public void TreeView_UsesPlusAndMinusToExpandAndCollapse()
    {
        var tree = new TreeView { Active = true };
        var root = new TreeNode("Root");
        root.Add(new TreeNode("Child"));
        tree.Nodes.Add(root);
        Assert.IsTrue(tree.SelectNext());

        tree.HandlePressKeyEvents(new KeyEventStub(0, '+'));
        Assert.IsTrue(root.IsExpanded);

        tree.HandlePressKeyEvents(new KeyEventStub(0, '-'));
        Assert.IsFalse(root.IsExpanded);
    }

    private sealed class KeyEventStub(ushort keyCode, char keyChar) : IKeyEvent
    {
        public bool bKeyDown => true;
        public char KeyChar => keyChar;
        public ushort usKeyCode => keyCode;
        public ushort usScanCode => 0;
        public uint dwControlKeyState => 0;
        public bool Handled { get; set; }
    }
}
