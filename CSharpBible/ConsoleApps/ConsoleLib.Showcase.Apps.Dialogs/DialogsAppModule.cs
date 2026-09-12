using System;
using System.Drawing;
using ConsoleLib;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Dialogs;

/// <summary>Registers the host-neutral settings and file-dialog showcase.</summary>
public sealed class DialogsAppModule : IShowcaseAppModule
{
    public const string AppId = "Dialogs";

    public void Register(ShowcaseAppRegistrationContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        context.Register(new ShowcaseAppDescriptor(
            AppId,
            "Dialogs",
            ShowcaseAppCategory.Controls,
            "Reusable settings and file-dialog resources.",
            new Point(6, 4),
            new Size(70, 21),
            services => new DialogsViewModel(ResolveFileDialogService(services)),
            (_, viewModel) => DialogsPage.Load((DialogsViewModel)viewModel)));
    }

    private static IShowcaseFileDialogService ResolveFileDialogService(IServiceProvider services) =>
        services.GetService(typeof(IShowcaseFileDialogService)) as IShowcaseFileDialogService
        ?? new UnavailableShowcaseFileDialogService();
}
