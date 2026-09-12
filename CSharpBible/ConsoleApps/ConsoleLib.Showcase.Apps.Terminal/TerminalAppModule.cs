using System;
using System.Drawing;
using System.IO;
using ConsoleLib;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Terminal;

public sealed class TerminalAppModule : IShowcaseAppModule
{
    public const string AppId = "Terminal";

    public void Register(ShowcaseAppRegistrationContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        context.Register(new ShowcaseAppDescriptor(AppId, "Terminal", ShowcaseAppCategory.Productivity,
            "A host-provided terminal session.", new Point(5, 2), new Size(78, 22),
            services => new TerminalViewModel(services.GetService(typeof(IShowcaseTerminalCapability)) as IShowcaseTerminalCapability),
            (_, model) => Load(model)));
    }

    private static CxamlLoadResult Load(object model)
    {
        var resourceName = typeof(TerminalAppModule).Assembly.GetManifestResourceNames()
            .Single(resource => resource.EndsWith("Pages.Terminal.cxaml", StringComparison.Ordinal));
        using var stream = typeof(TerminalAppModule).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The terminal CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().Load(reader, new CxamlLoadContext(model));
    }
}
