using System;
using System.Drawing;
using ConsoleLib;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Gallery;

/// <summary>Registers the reusable ConsoleLib controls gallery.</summary>
public sealed class GalleryAppModule : IShowcaseAppModule
{
    public const string AppId = "Gallery";

    public void Register(ShowcaseAppRegistrationContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        context.Register(new ShowcaseAppDescriptor(
            AppId,
            "Controls Gallery",
            ShowcaseAppCategory.Controls,
            "An interactive catalog of standard ConsoleLib controls and layouts.",
            new Point(2, 2),
            new Size(82, 30),
            _ => new GalleryViewModel(),
            (_, viewModel) => GalleryPage.Load((GalleryViewModel)viewModel)));
    }
}
