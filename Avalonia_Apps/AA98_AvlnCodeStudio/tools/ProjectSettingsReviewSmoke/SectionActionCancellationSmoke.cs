using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Config.UI.Avalonia.Views;

internal static partial class Program
{
    private static async Task VerifySectionButtonCancellationAsync(Window owner, string action, bool direct)
    {
        LayeredFixture fixture = new(storedFirst: true);
        ConfigUiDialog dialog = new(fixture.Config);
        Task showing = dialog.ShowDialog(owner);
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, $"{action} initial section");
        dialog.GetVisualDescendants().OfType<ConfigUiView>().Single().ShowSectionActions = true;
        ListBox list = SectionList(dialog, fixture.Config);
        SetValue(fixture.Config, "First draft");
        list.SelectedItem = fixture.Independent;
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, $"{action} independent section");
        SetValue(fixture.Config, "Independent draft");
        list.SelectedItem = fixture.Shared[0];

        if (action == "Save")
        {
            fixture.User.AfterSave = () => fixture.System.SuspendOnLoad = 1;
        }
        else
        {
            fixture.System.SuspendOnLoad = action == "Reset" ? 2 : 1;
        }

        Button button = FindButton(dialog, action);
        Check(button.IsVisible && button.IsEnabled, $"{action} button is not usable.");
        Task? executing = null;
        if (direct)
        {
            executing = action switch
            {
                "Load" => fixture.Config.LoadCommand.ExecuteAsync(null),
                "Reset" => fixture.Config.ResetCommand.ExecuteAsync(null),
                "Save" => fixture.Config.SaveCommand.ExecuteAsync(null),
                _ => throw new InvalidOperationException(action),
            };
        }
        else
        {
            Invoke(dialog, action);
        }

        await WaitUntilAsync(() => fixture.System.LoadSuspended && fixture.Config.IsBusy, $"{action} in progress");
        dialog.Close();
        Check(dialog.IsVisible && fixture.Config.IsBusy, $"{action} allowed the dialog to close while busy.");
        fixture.System.SuspendedLoad.SetException(new OperationCanceledException("Synthetic cancellation"));
        if (direct)
        {
            try
            {
                await executing!;
                throw new InvalidOperationException($"{action} ExecuteAsync did not propagate cancellation.");
            }
            catch (OperationCanceledException)
            {
            }
        }
        await WaitUntilAsync(() => !fixture.Config.IsBusy, $"{action} cancellation");
        await Task.Delay(200);
        Check(dialog.IsVisible && FindButton(dialog, "Cancel").IsEnabled, $"{action} cancellation closed or locked dialog.");
        string message = action == "Load" ? fixture.Config.ErrorMessage : fixture.Config.OperationMessage;
        Check(!string.IsNullOrWhiteSpace(message) && dialog.GetVisualDescendants().OfType<TextBlock>()
            .Any(text => text.IsVisible && text.Text == message), $"{action} cancellation message is not visible.");
        Check(fixture.Independent.SavedValue == "Original", $"{action} saved an unrelated draft.");
        list.SelectedItem = fixture.Independent;
        Check(Equals(fixture.Config.PropertyEditor.Items.Single().ScalarValue, "Independent draft"),
            $"{action} lost the independent draft.");
        list.SelectedItem = fixture.Shared[0];

        if (action == "Save")
        {
            Check(fixture.User.SaveCount == 1 && Equals(ValueIn(fixture.User.Snapshot, "First"), "First draft")
                && message.Contains("saved", StringComparison.OrdinalIgnoreCase),
                "Cancelled Save did not report the confirmed write.");
            fixture.User.AfterSave = null;
            Invoke(dialog, "Apply");
        }
        else
        {
            Check(fixture.User.SaveCount == 0 && Equals(ValueIn(fixture.User.Snapshot, "First"), "Stored"),
                $"{action} unexpectedly wrote settings.");
            Check(fixture.Config.NeedsReloadRecovery && fixture.Config.PropertyEditor.Items.Count == 0
                && FindButton(dialog, "Retry discard/reload").IsVisible
                && FindButton(dialog, "Retry discard/reload").IsEnabled
                && !FindButton(dialog, "Apply").IsEnabled, $"{action} did not expose safe recovery.");
            if (action == "Load" && !direct)
            {
                fixture.System.SuspendOnLoad = 1;
                Invoke(dialog, "Retry discard/reload");
                await WaitUntilAsync(() => fixture.System.LoadSuspended && fixture.Config.IsBusy, "retry in progress");
                fixture.System.SuspendedLoad.SetException(new OperationCanceledException("Synthetic retry cancellation"));
                await WaitUntilAsync(() => !fixture.Config.IsBusy, "cancelled retry");
                await Task.Delay(200);
                Check(dialog.IsVisible && fixture.Config.NeedsReloadRecovery
                    && FindButton(dialog, "Retry discard/reload").IsEnabled, "Cancelled retry did not retain recovery.");
            }
            Invoke(dialog, "Retry discard/reload");
            await WaitUntilAsync(() => !fixture.Config.IsBusy && !fixture.Config.NeedsReloadRecovery, $"{action} retry");
            Check(Equals(fixture.Config.PropertyEditor.Items.Single().ScalarValue, "Stored"),
                $"{action} retry did not reload stored settings.");
            Invoke(dialog, "Apply");
        }

        await WaitUntilAsync(() => !fixture.Config.IsBusy && !fixture.Config.HasUnsavedChanges, $"{action} apply retry");
        Check(fixture.Independent.SavedValue == "Independent draft", $"{action} retry did not save independent draft.");
        Invoke(dialog, "Cancel");
        await showing.WaitAsync(TimeSpan.FromSeconds(30));
    }
}
