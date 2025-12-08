using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunCollapse.Models.Data;

public class TileData(int index, object? orgData,long lTHash)
{
    public int Index { get;  } = index;
    public object? OrgData { get; } = orgData;
    public Dictionary<EDirections, List<int>> Neighbors { get; } = [];
    public long LTHash { get; } = lTHash;

}
