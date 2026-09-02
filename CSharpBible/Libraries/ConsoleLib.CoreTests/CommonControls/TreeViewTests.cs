using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

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

    [TestMethod]
    public void TreeView_HandlesChildExpansionAndCharacterShortcuts()
    {
        var tree = new TreeView();
        var root = new TreeNode("Root");
        var child = root.Add(new TreeNode("Child"));
        child.Add(new TreeNode("Grandchild"));
        tree.Nodes.Add(root);
        tree.Active = true;

        Assert.IsFalse(tree.ToggleExpansion());
        Assert.IsTrue(tree.SelectNext());
        var expand = new KeyEventStub(0, '+');
        tree.HandlePressKeyEvents(expand);
        Assert.IsTrue(expand.Handled);
        Assert.IsTrue(root.IsExpanded);

        Assert.IsTrue(tree.SelectNext());
        var childExpand = new KeyEventStub((ushort)System.ConsoleKey.RightArrow);
        tree.HandlePressKeyEvents(childExpand);
        Assert.IsTrue(childExpand.Handled);
        Assert.IsTrue(child.IsExpanded);

        var collapse = new KeyEventStub(0, '-');
        tree.HandlePressKeyEvents(collapse);
        Assert.IsTrue(collapse.Handled);
        Assert.AreSame(child, tree.SelectedNode);
    }

    [TestMethod]
    public void TreeView_EmptyAndBoundaryNavigationIsNotHandled()
    {
        var tree = new TreeView();
        Assert.IsFalse(tree.SelectNext());
        Assert.IsFalse(tree.SelectPrevious());
        Assert.IsFalse(tree.ToggleExpansion());

        var key = new KeyEventStub((ushort)System.ConsoleKey.DownArrow);
        tree.HandlePressKeyEvents(key);
        Assert.IsFalse(key.Handled);

        var root = new TreeNode("Root");
        tree.Nodes.Add(root);
        Assert.IsTrue(tree.SelectNext());
        Assert.IsFalse(tree.SelectNext());
        Assert.IsFalse(tree.SelectPrevious());
    }

    [TestMethod]
    public void TreeView_DrawUsesOptionalRenderer()
    {
        var renderer = Substitute.For<IWidgetSet, ITreeViewRenderer>();
        var app = new Application(renderer);
        var tree = new TreeView { Parent = app };

        tree.Draw();

        ((ITreeViewRenderer)renderer).Received(1).DrawTreeView(tree);
    }

    private sealed class KeyEventStub : IKeyEvent
    {
        public KeyEventStub(ushort keyCode, char keyChar = '\0')
        {
            usKeyCode = keyCode;
            KeyChar = keyChar;
        }
        public bool bKeyDown => true;
        public char KeyChar { get; }
        public ushort usKeyCode { get; }
        public ushort usScanCode => 0;
        public uint dwControlKeyState => 0;
        public bool Handled { get; set; }
    }
}
