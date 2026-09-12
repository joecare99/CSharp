using System;
using System.Drawing;
using System.IO;
using ConsoleLib;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Notepad;

public sealed class NotepadAppModule : IShowcaseAppModule
{
    public const string AppId = "Notepad";

    public void Register(ShowcaseAppRegistrationContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        context.Register(new ShowcaseAppDescriptor(AppId, "Notepad", ShowcaseAppCategory.Productivity,
            "A host-aware text editor.", new Point(16, 3), new Size(60, 22),
            services => new NotepadViewModel(services.GetService(typeof(IClipboardService)) as IClipboardService),
            (_, model) => Load(model)));
    }

    private static CxamlLoadResult Load(object model)
    {
        var resourceName = typeof(NotepadAppModule).Assembly.GetManifestResourceNames()
            .Single(resource => resource.EndsWith("Pages.Notepad.cxaml", StringComparison.Ordinal));
        using var stream = typeof(NotepadAppModule).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The notepad CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().Load(reader, new CxamlLoadContext(model));
    }
}
