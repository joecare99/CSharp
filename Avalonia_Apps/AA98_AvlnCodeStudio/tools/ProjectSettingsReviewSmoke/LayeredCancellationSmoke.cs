using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Config.Service;
using Config.UI;
using Config.UI.Avalonia.ViewModels;
using Config.UI.Avalonia.Views;
using Config.UI.ConfigService;
using Property.Editor.Avalonia;

internal static partial class Program
{
    private static async Task VerifyLayeredCancellationAsync(Window owner)
    {
        await VerifyCancelledApplyAsync(owner, "Apply");
        await VerifyCancelledApplyAsync(owner, "OK");
        await VerifyCancelledCloseAsync(owner, useCloseButton: false);
        await VerifyCancelledCloseAsync(owner, useCloseButton: true);
        await VerifyCancelledDraftResetAsync(owner);
        foreach (string action in new[] { "Load", "Reset", "Save" })
        {
            await VerifySectionButtonCancellationAsync(owner, action, direct: false);
            await VerifySectionButtonCancellationAsync(owner, action, direct: true);
        }
        Console.WriteLine("Shared config dialog: cancelled post-save refresh and draft reset, independent drafts and retry verified.");
    }

    private static async Task VerifyCancelledApplyAsync(Window owner, string action)
    {
        LayeredFixture fixture = new();
        ConfigUiDialog dialog = new(fixture.Config);
        Task showing = dialog.ShowDialog(owner);
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, "layered apply section");
        ListBox list = SectionList(dialog, fixture.Config);
        SetValue(fixture.Config, "First draft");
        list.SelectedItem = fixture.Shared[1];
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, "layered sibling");
        SetValue(fixture.Config, "Sibling draft");
        list.SelectedItem = fixture.Independent;
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, "layered independent");
        SetValue(fixture.Config, "Independent draft");
        list.SelectedItem = fixture.Shared[0];
        fixture.User.AfterSave = () => fixture.System.CancelNextLoad = true;
        Invoke(dialog, action);
        await WaitUntilAsync(() => !fixture.Config.IsBusy && fixture.User.SaveCount == 1, "cancelled post-save refresh");
        await Task.Delay(200);
        Check(dialog.IsVisible && FindButton(dialog, "Cancel").IsEnabled, "Cancelled Apply left the dialog unusable.");
        Check(fixture.Config.HasUnsavedChanges && FindButton(dialog, "Apply").IsEnabled,
            "Cancelled Apply did not retain retryable drafts.");
        Check(fixture.Config.OperationMessage.Contains("saved", StringComparison.OrdinalIgnoreCase),
            "Confirmed persistence was not reported.");
        Check(dialog.GetVisualDescendants().OfType<TextBlock>()
            .Any(text => text.IsVisible && text.Text == fixture.Config.OperationMessage), "Persistence message was not displayed.");
        Check(Equals(ValueIn(fixture.User.Snapshot, "First"), "First draft")
            && Equals(ValueIn(fixture.User.Snapshot, "Sibling"), "Sibling draft"), "Shared drafts were not persisted.");
        Check(fixture.Independent.SavedValue == "Original", "Independent draft was applied after cancellation.");
        list.SelectedItem = fixture.Independent;
        Check(Equals(fixture.Config.PropertyEditor.Items.Single().ScalarValue, "Independent draft"),
            "Independent draft was lost after cancelled Apply.");
        list.SelectedItem = fixture.Shared[0];
        fixture.User.AfterSave = null;
        Invoke(dialog, "Apply");
        await WaitUntilAsync(() => !fixture.Config.IsBusy && !fixture.Config.HasUnsavedChanges, "apply retry");
        Check(fixture.Independent.SavedValue == "Independent draft", "Apply retry did not persist independent draft.");
        Invoke(dialog, "Cancel");
        await showing.WaitAsync(TimeSpan.FromSeconds(30));
    }

    private static async Task VerifyCancelledCloseAsync(Window owner, bool useCloseButton)
    {
        LayeredFixture fixture = new();
        ConfigUiDialog dialog = new(fixture.Config);
        Task showing = dialog.ShowDialog(owner);
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, "close cancellation section");
        SetValue(fixture.Config, "Draft");
        fixture.System.CancelNextLoad = true;
        if (useCloseButton)
        {
            dialog.Close();
        }
        else
        {
            Invoke(dialog, "Cancel");
        }

        await WaitUntilAsync(() => !fixture.Config.IsBusy && fixture.Config.NeedsReloadRecovery, "cancelled close discard");
        await Task.Delay(200);
        Check(dialog.IsVisible && FindButton(dialog, "Retry discard/reload").IsEnabled
            && !FindButton(dialog, "Apply").IsEnabled, "Cancelled close did not leave recovery available.");
        Check(fixture.User.SaveCount == 0, "Cancelled close persisted the draft.");
        Invoke(dialog, "Retry discard/reload");
        await WaitUntilAsync(() => !fixture.Config.IsBusy && !fixture.Config.NeedsReloadRecovery, "close discard retry");
        Check(!fixture.Config.HasUnsavedChanges, "Close retry retained the discarded draft.");
        Invoke(dialog, "Cancel");
        await showing.WaitAsync(TimeSpan.FromSeconds(30));
    }

    private static async Task VerifyCancelledDraftResetAsync(Window owner)
    {
        LayeredFixture fixture = new(storedFirst: true);
        ConfigUiDialog dialog = new(fixture.Config);
        Task showing = dialog.ShowDialog(owner);
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, "layered reset section");
        ListBox list = SectionList(dialog, fixture.Config);
        list.SelectedItem = fixture.Shared[1];
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, "reset sibling");
        SetValue(fixture.Config, "Sibling draft");
        list.SelectedItem = fixture.Independent;
        await WaitUntilAsync(() => fixture.Config.IsSelectedSectionReady, "reset independent");
        SetValue(fixture.Config, "Independent draft");
        list.SelectedItem = fixture.Shared[0];
        fixture.System.CancelOnLoad = 2;
        try
        {
            await fixture.Config.ResetCommand.ExecuteAsync(null);
            throw new InvalidOperationException("Draft reset did not propagate cancellation.");
        }
        catch (OperationCanceledException)
        {
        }
        Check(dialog.IsVisible && !fixture.Config.IsBusy && fixture.Config.NeedsReloadRecovery,
            "Cancelled reset did not leave an operable recovery dialog.");
        Check(FindButton(dialog, "Retry discard/reload").IsVisible && FindButton(dialog, "Retry discard/reload").IsEnabled
            && !FindButton(dialog, "Apply").IsEnabled && fixture.Config.PropertyEditor.Items.Count == 0,
            "Cancelled reset exposed an unsafe draft or did not offer retry.");
        Check(fixture.User.SaveCount == 0 && Equals(ValueIn(fixture.User.Snapshot, "First"), "Stored"),
            "Draft reset changed persistence.");
        list.SelectedItem = fixture.Independent;
        Check(Equals(fixture.Config.PropertyEditor.Items.Single().ScalarValue, "Independent draft"),
            "Cancelled reset lost the independent draft.");
        list.SelectedItem = fixture.Shared[0];
        Invoke(dialog, "Retry discard/reload");
        await WaitUntilAsync(() => !fixture.Config.IsBusy && !fixture.Config.NeedsReloadRecovery, "reset recovery");
        Check(Equals(fixture.Config.PropertyEditor.Items.Single().ScalarValue, "Stored"),
            "Recovery did not restore stored value.");
        Check(FindButton(dialog, "Apply").IsEnabled && fixture.Config.HasUnsavedChanges,
            "Recovery did not preserve the independent draft.");
        list.SelectedItem = fixture.Shared[1];
        Check(Equals(fixture.Config.PropertyEditor.Items.Single().ScalarValue, "Default"),
            "Recovery did not discard the sibling shared draft.");
        Invoke(dialog, "Apply");
        await WaitUntilAsync(() => !fixture.Config.IsBusy && !fixture.Config.HasUnsavedChanges, "reset recovery apply");
        Check(fixture.Independent.SavedValue == "Independent draft" && fixture.User.SaveCount == 0,
            "Recovery apply did not save only the independent draft.");
        Invoke(dialog, "Cancel");
        await showing.WaitAsync(TimeSpan.FromSeconds(30));
    }

    private static ListBox SectionList(Window dialog, ConfigUiViewModel config) =>
        dialog.GetVisualDescendants().OfType<ListBox>().Single(list => ReferenceEquals(list.ItemsSource, config.Sections));

    private static void SetValue(ConfigUiViewModel config, string value) =>
        config.PropertyEditor.Items.Single().ScalarValue = value;

    private static object? ValueIn(ConfigLayerSnapshot snapshot, string section) =>
        snapshot.TryGetValue(new ConfigPropertyKey(section, nameof(SmokeSettings.Name)), out ConfigLayerValue? value)
            ? value.Value : null;

    private sealed class LayeredFixture
    {
        public FakeLayerStore User { get; } = new(ConfigLayer.User);
        public FakeLayerStore System { get; } = new(ConfigLayer.System);
        public IndependentSection Independent { get; } = new();
        public IReadOnlyList<IConfigUiSection> Shared { get; }
        public ConfigUiViewModel Config { get; }

        public LayeredFixture(bool storedFirst = false)
        {
            if (storedFirst)
            {
                User.Snapshot = User.Snapshot.WithValue(
                    new ConfigPropertyKey("First", nameof(SmokeSettings.Name)), new ConfigLayerValue("Stored"));
            }
            ConfigSectionRegistry providers = new();
            providers.AddSection(new SmokeProvider("First", 0));
            providers.AddSection(new SmokeProvider("Sibling", 1));
            ConfigLayeredSettingsResolver resolver = new(providers, [System, User]);
            Shared = new ConfigLayeredUiSectionRegistry(providers, resolver, User, ConfigLayer.User).Sections;
            Config = new ConfigUiViewModel(new SmokeRegistry([.. Shared, Independent]), new PropertyEditorViewModel());
        }
    }

    private sealed class SmokeRegistry(IReadOnlyList<IConfigUiSection> sections) : IConfigUiSectionRegistry
    {
        public IReadOnlyList<IConfigUiSection> Sections { get; } = sections;
    }

    private sealed class SmokeProvider(string name, int order) : IConfigSectionProvider
    {
        public string Name { get; } = name;
        public string DisplayName => Name;
        public string? Description => null;
        public int Order { get; } = order;
        public Type ModelType => typeof(SmokeSettings);
        public object CreateModel() => new SmokeSettings();
    }

    public sealed class SmokeSettings
    {
        public string Name { get; set; } = "Default";
    }

    private sealed class FakeLayerStore(ConfigLayer layer) : IConfigLayerStore
    {
        public ConfigLayer Layer { get; } = layer;
        public ConfigLayerSnapshot Snapshot { get; set; } = new(layer, []);
        public Action? AfterSave { get; set; }
        public int SaveCount { get; private set; }
        public bool CancelNextLoad { get; set; }
        private int _cancelOnLoad;
        public int CancelOnLoad { set => _cancelOnLoad = value; }
        private int _suspendOnLoad;
        public int SuspendOnLoad
        {
            set
            {
                _suspendOnLoad = value;
                LoadSuspended = false;
                SuspendedLoad = new(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }
        public TaskCompletionSource<ConfigLayerSnapshot> SuspendedLoad { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        public bool LoadSuspended { get; private set; }

        public Task<ConfigLayerSnapshot> LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_suspendOnLoad > 0 && --_suspendOnLoad == 0)
            {
                LoadSuspended = true;
                return SuspendedLoad.Task;
            }
            if (CancelNextLoad || (_cancelOnLoad > 0 && --_cancelOnLoad == 0))
            {
                CancelNextLoad = false;
                return Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException("Synthetic cancellation"));
            }
            return Task.FromResult(Snapshot);
        }

        public Task<ConfigLayerSnapshot> SaveAsync(ConfigLayerSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Snapshot = snapshot;
            SaveCount++;
            AfterSave?.Invoke();
            return Task.FromResult(snapshot);
        }
    }
}
