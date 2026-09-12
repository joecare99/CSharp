using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Dispatcher-independent registry and navigation service for Frames.</summary>
public sealed class FrameNavigationService : IFrameNavigationService
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Frame> _frames = new(StringComparer.Ordinal);
    private readonly Dictionary<string, PageDescriptor> _pages = new(StringComparer.Ordinal);

    public FrameNavigationService(IServiceProvider services)
        => _services = services ?? throw new ArgumentNullException(nameof(services));

    public void Register(string regionName, Frame frame)
    {
        if (string.IsNullOrWhiteSpace(regionName))
            throw new ArgumentException("A frame region name is required.", nameof(regionName));
        _frames[regionName] = frame ?? throw new ArgumentNullException(nameof(frame));
    }

    public void RegisterPage(PageDescriptor descriptor)
    {
        if (descriptor is null)
            throw new ArgumentNullException(nameof(descriptor));
        _pages.Add(descriptor.Name, descriptor);
    }

    public Task NavigateAsync(string regionName, NavigationRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_frames.TryGetValue(regionName, out var frame))
            throw new KeyNotFoundException($"Frame region '{regionName}' is not registered.");
        if (!_pages.TryGetValue(request.PageName, out var descriptor))
            throw new KeyNotFoundException($"Page '{request.PageName}' is not registered.");

        var page = descriptor.Create(_services, request.DataContext ?? request.Selection);
        frame.SetContent(page);
        return Task.CompletedTask;
    }
}
