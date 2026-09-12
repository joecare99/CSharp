using System;
using System.IO;
using System.Linq;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Apps.Dialogs;

/// <summary>Loads the showcase page and reusable dialog resources.</summary>
public static class DialogsPage
{
    public static CxamlLoadResult Load(DialogsViewModel viewModel)
    {
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        return LoadPage("Dialogs.cxaml", viewModel);
    }

    public static Dialog LoadSettingsDialog(SettingsViewModel viewModel)
    {
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        var result = LoadDialog("SettingsDialog.cxaml", viewModel);
        WireSettingsInteractions(result, viewModel);
        return (Dialog)result.Root;
    }

    public static Dialog LoadOpenFileDialog(DialogFileViewModel viewModel) =>
        LoadFileDialog("OpenFileDialog.cxaml", viewModel);

    public static Dialog LoadSaveFileDialog(DialogFileViewModel viewModel) =>
        LoadFileDialog("SaveFileDialog.cxaml", viewModel);

    public static Dialog LoadPickFolderDialog(DialogFileViewModel viewModel) =>
        LoadFileDialog("PickFolderDialog.cxaml", viewModel);

    private static Dialog LoadFileDialog(string resourceName, DialogFileViewModel viewModel)
    {
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));
        return (Dialog)LoadDialog(resourceName, viewModel).Root;
    }

    private static CxamlLoadResult LoadDialog(string resourceName, object viewModel)
    {
        using var stream = OpenResource(resourceName);
        using var reader = new StreamReader(stream);
        return new CxamlLoader().LoadDialog(reader, new CxamlLoadContext(viewModel));
    }

    private static CxamlLoadResult LoadPage(string resourceName, object viewModel)
    {
        using var stream = OpenResource(resourceName);
        using var reader = new StreamReader(stream);
        return new CxamlLoader().LoadPage(reader, new CxamlLoadContext(viewModel));
    }

    private static void WireSettingsInteractions(CxamlLoadResult result, SettingsViewModel viewModel)
    {
        var theme = (TextBox)result.NamedControls["ThemeTextBox"];
        var animations = (CheckBox)result.NamedControls["AnimationsCheckBox"];
        var statusBar = (CheckBox)result.NamedControls["StatusBarCheckBox"];
        var dialog = (Dialog)result.Root;

        theme.Text = viewModel.Theme;
        animations.IsChecked = viewModel.AnimationsEnabled;
        statusBar.IsChecked = viewModel.StatusBarVisible;
        ((Button)result.NamedControls["ApplyButton"]).OnClick += (_, _) =>
        {
            viewModel.Apply(new SettingsSnapshot(theme.Text, animations.IsChecked, statusBar.IsChecked));
            dialog.Hide();
        };
        ((Button)result.NamedControls["CancelButton"]).OnClick += (_, _) =>
        {
            viewModel.Cancel();
            dialog.Hide();
        };
    }

    private static Stream OpenResource(string resourceName)
    {
        var assembly = typeof(DialogsPage).Assembly;
        var manifestName = assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith("." + resourceName, StringComparison.Ordinal));
        if (manifestName is null)
            throw new InvalidOperationException($"The dialog resource '{resourceName}' is missing.");
        return assembly.GetManifestResourceStream(manifestName)
            ?? throw new InvalidOperationException($"The dialog resource '{resourceName}' could not be opened.");
    }

}
