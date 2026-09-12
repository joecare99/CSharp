using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Interfaces;

public interface IFrameNavigationService
{
    void Register(string regionName, CommonControls.Frame frame);
    void RegisterPage(PageDescriptor descriptor);
    Task NavigateAsync(string regionName, NavigationRequest request, CancellationToken cancellationToken = default);
}
