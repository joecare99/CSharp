using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Places;

public interface ICoordinateSchemaProbe
{
    Task<CoordinateSchemaReport> ProbeAsync(
        CancellationToken cancellationToken = default);
}
