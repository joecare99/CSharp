using System;
using System.Drawing;
using System.IO;
using ConsoleLib;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Clock;

public sealed class ClockAppModule : IShowcaseAppModule
{
    public const string AppId = "Clock";

    public void Register(ShowcaseAppRegistrationContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        context.Register(new ShowcaseAppDescriptor(AppId, "Clock", ShowcaseAppCategory.Productivity,
            "An analog alarm clock with host alerts.", new Point(12, 3), new Size(38, 22),
            services => new ClockViewModel(alert: services.GetService(typeof(IShowcaseAlertService)) as IShowcaseAlertService),
            (_, model) => Load(model)));
    }

    private static CxamlLoadResult Load(object model)
    {
        var registry = new CxamlComponentRegistry();
        registry.Register("ClockPixelCanvas", (_, _) => new Controls.ClockPixelCanvas());
        var resourceName = typeof(ClockAppModule).Assembly.GetManifestResourceNames()
            .Single(resource => resource.EndsWith("Pages.Clock.cxaml", StringComparison.Ordinal));
        using var stream = typeof(ClockAppModule).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The clock CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader(registry).Load(reader, new CxamlLoadContext(model));
    }
}
