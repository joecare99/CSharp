using System;
using System.IO;
using System.Linq;
using ConsoleLib;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Desktop;

/// <summary>Loads a renderer-neutral embedded CXAML desktop page.</summary>
public sealed class DesktopPage
{
    private readonly CxamlLoader _loader;

    public DesktopPage(CxamlLoader? loader = null) =>
        _loader = loader ?? new CxamlLoader();

    /// <summary>Loads and binds the embedded page to the provided ViewModel.</summary>
    public CxamlLoadResult Load(object viewModel)
    {
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        var resourceName = typeof(DesktopPage).Assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith("Pages.Desktop.cxaml", StringComparison.OrdinalIgnoreCase));
        using var stream = resourceName is null
            ? null
            : typeof(DesktopPage).Assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new InvalidOperationException("The desktop CXAML page resource is missing.");
        }
        using var reader = new StreamReader(stream);
        return _loader.Load(reader, new CxamlLoadContext(viewModel));
    }
}
