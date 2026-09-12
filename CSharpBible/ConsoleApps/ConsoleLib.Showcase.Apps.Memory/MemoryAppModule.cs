using System;
using System.Drawing;
using System.IO;
using System.Linq;
using ConsoleLib;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Memory;

public sealed class MemoryAppModule : IShowcaseAppModule
{
    public void Register(ShowcaseAppRegistrationContext context)
    {
        context.Register(new ShowcaseAppDescriptor(
            "Memory",
            "Memory",
            ShowcaseAppCategory.Games,
            "A deterministic, pair-matching memory game.",
            new Point(5, 4),
            new Size(34, 17),
            _ => new MemoryViewModel(),
            (_, viewModel) => Load(viewModel)));
    }

    private static CxamlLoadResult Load(object viewModel)
    {
        var assembly = typeof(MemoryAppModule).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith(".Memory.cxaml", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The Memory CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().LoadPage(reader, new CxamlLoadContext(viewModel));
    }
}
