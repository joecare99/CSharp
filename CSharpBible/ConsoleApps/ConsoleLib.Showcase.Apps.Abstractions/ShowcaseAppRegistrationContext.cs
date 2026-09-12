using System;
using System.Collections.Generic;

namespace ConsoleLib.Showcase.Apps;

/// <summary>Explicit registry supplied by a host while composing feature modules.</summary>
public sealed class ShowcaseAppRegistrationContext
{
    private readonly Dictionary<string, ShowcaseAppDescriptor> _apps = new(StringComparer.Ordinal);

    public ShowcaseAppRegistrationContext(IServiceProvider services)
        => Services = services ?? throw new ArgumentNullException(nameof(services));

    public IServiceProvider Services { get; }
    public IReadOnlyDictionary<string, ShowcaseAppDescriptor> Apps => _apps;

    public void Register(ShowcaseAppDescriptor descriptor)
    {
        if (descriptor is null)
            throw new ArgumentNullException(nameof(descriptor));
        _apps.Add(descriptor.Id, descriptor);
    }
}
