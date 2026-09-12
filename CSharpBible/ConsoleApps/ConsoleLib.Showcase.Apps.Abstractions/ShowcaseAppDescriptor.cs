using System;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Apps;

/// <summary>Host-independent metadata and factories for an embeddable application.</summary>
public sealed class ShowcaseAppDescriptor
{
    public ShowcaseAppDescriptor(
        string id,
        string title,
        ShowcaseAppCategory category,
        string description,
        Point initialPosition,
        Size initialSize,
        Func<IServiceProvider, object> createViewModel,
        Func<IServiceProvider, object, CxamlLoadResult> loadPage)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("An app ID is required.", nameof(id)) : id;
        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("An app title is required.", nameof(title)) : title;
        Category = category;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        InitialPosition = initialPosition;
        InitialSize = initialSize;
        CreateViewModel = createViewModel ?? throw new ArgumentNullException(nameof(createViewModel));
        LoadPage = loadPage ?? throw new ArgumentNullException(nameof(loadPage));
    }

    public string Id { get; }
    public string Title { get; }
    public ShowcaseAppCategory Category { get; }
    public string Description { get; }
    public Point InitialPosition { get; }
    public Size InitialSize { get; }
    public Func<IServiceProvider, object> CreateViewModel { get; }
    public Func<IServiceProvider, object, CxamlLoadResult> LoadPage { get; }
}
