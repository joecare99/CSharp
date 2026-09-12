using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.CommonControls;
using ConsoleLib.Showcase.Apps;
using ConsoleLib.Showcase.Apps.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Apps.Dialogs.Tests;

[TestClass]
public sealed class DialogsModuleTests
{
    [TestMethod]
    public void Register_ExposesDialogsDescriptorAndLoadsPage()
    {
        var context = new ShowcaseAppRegistrationContext(new EmptyServices());

        new DialogsAppModule().Register(context);

        var descriptor = context.Apps[DialogsAppModule.AppId];
        var model = descriptor.CreateViewModel(context.Services);
        var page = descriptor.LoadPage(context.Services, model);

        Assert.AreEqual(ShowcaseAppCategory.Controls, descriptor.Category);
        Assert.IsInstanceOfType<DialogsViewModel>(model);
        Assert.IsInstanceOfType<Page>(page.Root);
        Assert.IsTrue(page.NamedControls.ContainsKey("SettingsButton"));
    }

    [TestMethod]
    public void DialogResources_LoadWithExpectedNamedControls()
    {
        var settings = DialogsPage.LoadSettingsDialog(new SettingsViewModel());
        var open = DialogsPage.LoadOpenFileDialog(new DialogFileViewModel("Open"));
        var save = DialogsPage.LoadSaveFileDialog(new DialogFileViewModel("Save"));
        var folder = DialogsPage.LoadPickFolderDialog(new DialogFileViewModel("Folder"));

        Assert.IsTrue(settings.Children.Count > 0);
        Assert.IsInstanceOfType(open, typeof(Dialog));
        Assert.IsInstanceOfType(save, typeof(Dialog));
        Assert.IsInstanceOfType(folder, typeof(Dialog));
    }

    [TestMethod]
    public void SettingsDialog_ApplyAndCancelUpdateState()
    {
        var model = new SettingsViewModel();
        var dialog = DialogsPage.LoadSettingsDialog(model);
        var applied = (TextBox)dialog.Children[2];
        var apply = (Button)dialog.Children[5];

        applied.Text = "Dark";
        apply.Click();

        Assert.AreEqual("Dark", model.Theme);
        Assert.AreEqual("Settings applied.", model.Status);
        Assert.IsFalse(dialog.Visible);

        dialog = DialogsPage.LoadSettingsDialog(model);
        ((Button)dialog.Children[6]).Click();
        Assert.AreEqual("Settings changes cancelled.", model.Status);
        Assert.IsFalse(dialog.Visible);
    }

    [TestMethod]
    public async Task FileCommands_ReportAcceptedCancelledAndUnavailableResults()
    {
        var service = new FakeFileDialogService
        {
            OpenResult = FileDialogResult.Accepted("C:\\open.txt"),
            SaveResult = FileDialogResult.Cancelled(),
            FolderResult = FileDialogResult.Failed("Picker failed.")
        };
        var model = new DialogsViewModel(service);

        await model.OpenFileCommand.ExecuteAsync(null);
        Assert.AreEqual("Selected: C:\\open.txt", model.Status);
        await model.SaveFileCommand.ExecuteAsync(null);
        Assert.AreEqual("Selection cancelled.", model.Status);
        await model.PickFolderCommand.ExecuteAsync(null);
        Assert.AreEqual("Picker failed.", model.Status);
    }

    private sealed class EmptyServices : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    private sealed class FakeFileDialogService : IShowcaseFileDialogService
    {
        public FileDialogResult OpenResult { get; set; } = FileDialogResult.Cancelled();
        public FileDialogResult SaveResult { get; set; } = FileDialogResult.Cancelled();
        public FileDialogResult FolderResult { get; set; } = FileDialogResult.Cancelled();

        public Task<FileDialogResult> OpenFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(OpenResult);
        public Task<FileDialogResult> SaveFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(SaveResult);
        public Task<FileDialogResult> PickFolderAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(FolderResult);
    }
}
