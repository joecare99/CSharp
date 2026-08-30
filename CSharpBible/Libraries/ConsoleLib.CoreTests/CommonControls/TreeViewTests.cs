using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class TreeViewTests
{
    [TestMethod]
    public void TreeView_ExpandsAndNavigatesVisibleNodes()
    {
        var tree = new TreeView();
        var root = new TreeNode("Root");
        root.Add(new TreeNode("Child"));
        tree.Nodes.Add(root);

        Assert.IsTrue(tree.SelectNext());
        Assert.AreSame(root, tree.SelectedNode);
        Assert.IsTrue(tree.ToggleExpansion());
        Assert.AreEqual(2, tree.GetVisibleNodes().Count);
        Assert.IsTrue(tree.SelectNext());
        Assert.AreEqual("Child", tree.SelectedNode!.Text);
    }

    [TestMethod]
    public void TreeView_LeftArrowSelectsParentThenCollapsesExpandedNode()
    {
        var tree = new TreeView();
        var root = new TreeNode("Root");
        root.Add(new TreeNode("Child"));
        tree.Nodes.Add(root);
        tree.Active = true;
        tree.SelectNext();
        tree.ToggleExpansion();
        tree.SelectNext();

        var selectParent = new KeyEventStub((ushort)System.ConsoleKey.LeftArrow);
        tree.HandlePressKeyEvents(selectParent);
        Assert.IsTrue(selectParent.Handled);
        Assert.AreSame(root, tree.SelectedNode);
        Assert.IsTrue(root.IsExpanded);

        var collapse = new KeyEventStub((ushort)System.ConsoleKey.LeftArrow);
        tree.HandlePressKeyEvents(collapse);
        Assert.IsTrue(collapse.Handled);
        Assert.IsFalse(root.IsExpanded);
        Assert.AreEqual(1, tree.GetVisibleNodes().Count);
    }

    private sealed class KeyEventStub : IKeyEvent
    {
        public KeyEventStub(ushort keyCode) => usKeyCode = keyCode;
        public bool bKeyDown => true;
        public char KeyChar => '\0';
        public ushort usKeyCode { get; }
        public ushort usScanCode => 0;
        public uint dwControlKeyState => 0;
        public bool Handled { get; set; }
    }
}
