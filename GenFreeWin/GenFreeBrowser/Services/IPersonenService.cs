using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Model;

namespace GenFreeBrowser;

public interface IPersonenService
{
    Task<IEnumerable<DispPersones>> LadeAlleAsync(CancellationToken ct = default);
    Task<(IReadOnlyList<DispPersones> Items,int TotalCount)> QueryAsync(PersonenQuery query, CancellationToken ct = default);
}
