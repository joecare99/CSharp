using System;
using System.IO;
using ConsoleLib.CommonControls;

namespace ConsoleLib.Interfaces;

/// <summary>Named factory/resource descriptor used by frame navigation.</summary>
public sealed class PageDescriptor
{
    private readonly Func<IServiceProvider, object?, Page> _factory;

    private PageDescriptor(string name, Func<IServiceProvider, object?, Page> factory)
    {
        Name = name;
        _factory = factory;
    }

    public string Name { get; }

    public static PageDescriptor FromFactory(string name, Func<IServiceProvider, object?, Page> factory)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("A page name is required.", nameof(name));
        return new PageDescriptor(name, factory ?? throw new ArgumentNullException(nameof(factory)));
    }

    public static PageDescriptor FromCxaml(
        string name,
        string markup,
        Func<IServiceProvider, object?>? dataContextFactory = null)
    {
        if (markup is null)
            throw new ArgumentNullException(nameof(markup));

        return FromFactory(name, (services, selection) =>
        {
            var context = dataContextFactory?.Invoke(services) ?? selection ?? new object();
            return (Page)new CxamlLoader().LoadPage(new StringReader(markup), new CxamlLoadContext(context)).Root;
        });
    }

    public Page Create(IServiceProvider services, object? selection)
        => _factory(services, selection) ?? throw new InvalidOperationException($"Page '{Name}' returned null.");
}
