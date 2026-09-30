using System.Threading;
using System.Threading.Tasks;

namespace IDR.App.Services;

public interface IFileSelectionService
{
    Task<string?> SelectPeImageAsync(CancellationToken cancellationToken);
}
