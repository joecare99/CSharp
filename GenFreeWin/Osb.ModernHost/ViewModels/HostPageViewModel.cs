using System;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.ViewModels;

public sealed class HostPageViewModel : IHostPageViewModel
{
    public HostPageViewModel(ModernHostPage page, string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("A page title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("A page description is required.", nameof(description));
        }

        Page = page;
        Title = title;
        Description = description;
    }

    public string Description { get; }

    public ModernHostPage Page { get; }

    public string Title { get; }
}
