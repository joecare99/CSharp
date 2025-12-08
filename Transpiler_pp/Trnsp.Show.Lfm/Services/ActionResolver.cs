using System.Collections.Generic;
using System.Linq;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Service to resolve action links and component references in component trees.
/// In Delphi/Lazarus, when a component has an Action property set,
/// it inherits Caption, Hint, ImageIndex, ShortCut, etc. from the action.
/// Also resolves ImageList references for ToolBars, MainMenus, etc.
/// </summary>
public interface IActionResolver
{
    /// <summary>
    /// Resolves all action links and component references in the component tree.
    /// </summary>
    void ResolveActions(LfmComponentBase root);
}

/// <summary>
/// Default implementation of action resolver.
/// </summary>
public class ActionResolver : IActionResolver
{
    /// <inheritdoc/>
    public void ResolveActions(LfmComponentBase root)
    {
        if (root == null) return;

        // First, collect all actions and image lists from the tree
        var actions = new Dictionary<string, TAction>(System.StringComparer.OrdinalIgnoreCase);
        var imageLists = new Dictionary<string, TImageList>(System.StringComparer.OrdinalIgnoreCase);
        
        CollectComponentsRecursive(root, actions, imageLists);

        // Then, resolve references for all components
        ResolveReferencesRecursive(root, actions, imageLists);
    }

    /// <summary>
    /// Collects all TAction and TImageList components from the tree.
    /// </summary>
    private void CollectComponentsRecursive(
        LfmComponentBase component, 
        Dictionary<string, TAction> actions,
        Dictionary<string, TImageList> imageLists)
    {
        // Collect actions
        if (component is TAction action && !string.IsNullOrEmpty(action.Name))
        {
            actions[action.Name] = action;
        }

        // Collect image lists
        if (component is TImageList imageList && !string.IsNullOrEmpty(imageList.Name))
        {
            imageLists[imageList.Name] = imageList;
        }

        // Process children
        foreach (var child in component.Children)
        {
            CollectComponentsRecursive(child, actions, imageLists);
        }
    }

    /// <summary>
    /// Resolves all references for components recursively.
    /// </summary>
    private void ResolveReferencesRecursive(
        LfmComponentBase component, 
        Dictionary<string, TAction> actions,
        Dictionary<string, TImageList> imageLists)
    {
        // Resolve action reference
        if (!string.IsNullOrEmpty(component.ActionName))
        {
            if (actions.TryGetValue(component.ActionName, out var action))
            {
                component.LinkedAction = action;
            }
        }

        // Resolve ImageList reference for TToolBar
        if (component is TToolBar toolBar && !string.IsNullOrEmpty(toolBar.Images))
        {
            if (imageLists.TryGetValue(toolBar.Images, out var imageList))
            {
                toolBar.ImageList = imageList;
            }
        }

        // Resolve ImageList reference for TMainMenu
        if (component is TMainMenu mainMenu && !string.IsNullOrEmpty(mainMenu.Images))
        {
            if (imageLists.TryGetValue(mainMenu.Images, out var imageList))
            {
                mainMenu.ImageList = imageList;
            }
        }

        // Resolve ImageList reference for TActionList
        if (component is TActionList actionList && !string.IsNullOrEmpty(actionList.Images))
        {
            if (imageLists.TryGetValue(actionList.Images, out var imageList))
            {
                actionList.ImageList = imageList;
            }
        }

        // Process children
        foreach (var child in component.Children)
        {
            ResolveReferencesRecursive(child, actions, imageLists);
        }
    }
}
