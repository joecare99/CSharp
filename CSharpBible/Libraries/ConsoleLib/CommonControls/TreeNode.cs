using System;
using System.Collections.Generic;

namespace ConsoleLib.CommonControls;

/// <summary>Represents one expandable node in a <see cref="TreeView"/>.</summary>
public sealed class TreeNode
{
    public TreeNode(string text)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public string Text { get; set; }
    public bool IsExpanded { get; set; }
    public bool IsSelected { get; internal set; }
    public TreeNode? Parent { get; internal set; }
    public IList<TreeNode> Children { get; } = new List<TreeNode>();

    public TreeNode Add(TreeNode child)
    {
        if (child is null)
            throw new ArgumentNullException(nameof(child));
        child.Parent = this;
        Children.Add(child);
        return child;
    }
}
