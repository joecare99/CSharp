using Osb.ModernHost.Models;

namespace Osb.ModernHost.ViewModels;

public interface IHostPageViewModel
{
    string Description { get; }

    ModernHostPage Page { get; }

    string Title { get; }
}
