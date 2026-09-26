using System;

namespace Views;

/// <summary>
/// Activates every supported declarative WinForms binding on a view.
/// </summary>
public static class ViewBinding
{
    /// <summary>
    /// Binds the annotated fields of <paramref name="view"/> to <paramref name="viewModel"/>.
    /// </summary>
    /// <param name="view">The WinForms view that contains annotated control fields.</param>
    /// <param name="viewModel">The source for bound properties and commands.</param>
    public static void Commit(object view, object viewModel)
    {
        if (view == null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        TextBindingAttribute.Commit(view, viewModel);
        CheckedBindingAttribute.Commit(view, viewModel);
        ListBindingAttribute.Commit(view, viewModel);
        EnabledBindingAttribute.Commit(view, viewModel);
        VisibilityBindingAttribute.Commit(view, viewModel);
        BackColorBindingAttribute.Commit(view, viewModel);
        CommandBindingAttribute.Commit(view, viewModel);
        DblClickBindingAttribute.Commit(view, viewModel);
        KeyBindingAttribute.Commit(view, viewModel);
    }
}
