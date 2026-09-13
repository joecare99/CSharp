using ConsoleLib.CommonControls;

namespace ConsoleLib.Interfaces;

/// <summary>Optional renderer capability for hierarchical tree controls.</summary>
public interface ITreeViewRenderer
{
    void DrawTreeView(TreeView treeView);
}
