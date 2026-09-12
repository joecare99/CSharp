using System;
using System.Drawing;
using System.IO;
using ConsoleLib;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Characters;

public sealed class CharactersAppModule : IShowcaseAppModule
{
    public const string AppId = "Characters";

    public void Register(ShowcaseAppRegistrationContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        context.Register(new ShowcaseAppDescriptor(AppId, "Characters", ShowcaseAppCategory.Productivity,
            "A keyboard-friendly ASCII character browser.", new Point(20, 5), new Size(60, 22),
            services => new CharactersViewModel(services.GetService(typeof(IClipboardService)) as IClipboardService),
            (_, model) => Load(model)));
    }

    private static CxamlLoadResult Load(object model)
    {
        var resourceName = typeof(CharactersAppModule).Assembly.GetManifestResourceNames()
            .Single(resource => resource.EndsWith("Pages.Characters.cxaml", StringComparison.Ordinal));
        using var stream = typeof(CharactersAppModule).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The characters CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().Load(reader, new CxamlLoadContext(model));
    }
}
