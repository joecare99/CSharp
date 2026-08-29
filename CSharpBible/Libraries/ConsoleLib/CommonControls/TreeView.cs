using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Hierarchical keyboard-navigable tree control.</summary>
public sealed class TreeView : Control
{
    private readonly List<TreeNode> _roots = new();
    private int _selectedIndex = -1;

    public IList<TreeNode> Nodes => _roots;
    public TreeNode? SelectedNode { get; private set; }

    public override void Draw()
    {
        if (WidgetSet is ITreeViewRenderer renderer)
            renderer.DrawTreeView(this);
        else
            base.Draw();
    }

    public bool SelectNext() => SelectRelative(1);
    public bool SelectPrevious() => SelectRelative(-1);

    public bool ToggleExpansion()
    {
        if (SelectedNode is null || SelectedNode.Children.Count == 0)
            return false;
        SelectedNode.IsExpanded = !SelectedNode.IsExpanded;
        Invalidate();
        return true;
    }

    public override void HandlePressKeyEvents(IKeyEvent e)
    {
        if (!Enabled || !Active)
        {
            base.HandlePressKeyEvents(e);
            return;
        }

        if (e.bKeyDown)
        {
            var handled = e.usKeyCode switch
            {
                (ushort)ConsoleKey.UpArrow => SelectPrevious(),
                (ushort)ConsoleKey.DownArrow => SelectNext(),
                (ushort)ConsoleKey.LeftArrow => CollapseOrSelectParent(),
                (ushort)ConsoleKey.RightArrow => ExpandOrSelectChild(),
                _ => false
            };
            if (!handled && e.KeyChar is '+' or '=')
                handled = ExpandOrSelectChild();
            if (!handled && e.KeyChar is '-')
                handled = CollapseOrSelectParent();
            if (handled)
            {
                e.Handled = true;
                return;
            }
        }
        base.HandlePressKeyEvents(e);
    }

    public List<TreeNode> GetVisibleNodes()
    {
        var result = new List<TreeNode>();
        foreach (var node in _roots)
            AddVisible(node, result);
        return result;
    }

    private bool SelectRelative(int direction)
    {
        var visible = GetVisibleNodes();
        if (visible.Count == 0)
            return false;
        var index = _selectedIndex < 0 ? (direction > 0 ? -1 : visible.Count) : _selectedIndex;
        var next = index + direction;
        if (next < 0 || next >= visible.Count)
            return false;
        Select(visible[next], next);
        return true;
    }

    private bool CollapseOrSelectParent()
    {
        if (SelectedNode is null)
            return false;
        if (SelectedNode.IsExpanded && SelectedNode.Children.Count > 0)
        {
            SelectedNode.IsExpanded = false;
            Invalidate();
            return true;
        }
        return SelectedNode.Parent is not null && SelectNode(SelectedNode.Parent);
    }

    private bool ExpandOrSelectChild()
    {
        if (SelectedNode is null || SelectedNode.Children.Count == 0)
            return false;
        if (!SelectedNode.IsExpanded)
        {
            SelectedNode.IsExpanded = true;
            Invalidate();
            return true;
        }
        return SelectNode(SelectedNode.Children[0]);
    }

    private bool SelectNode(TreeNode node)
    {
        var visible = GetVisibleNodes();
        var index = visible.IndexOf(node);
        if (index < 0)
            return false;
        Select(node, index);
        return true;
    }

    private void Select(TreeNode node, int index)
    {
        if (SelectedNode is not null)
            SelectedNode.IsSelected = false;
        SelectedNode = node;
        SelectedNode.IsSelected = true;
        _selectedIndex = index;
        Invalidate();
    }

    private static void AddVisible(TreeNode node, ICollection<TreeNode> result)
    {
        result.Add(node);
        if (!node.IsExpanded)
            return;
        foreach (var child in node.Children)
            AddVisible(child, result);
    }
}
