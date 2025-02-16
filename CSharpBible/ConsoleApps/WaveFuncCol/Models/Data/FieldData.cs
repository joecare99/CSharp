using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.Helper;

namespace WaveFunCollapse.Models.Data;

public class FieldData(int index)
{
    private ulong pTiles = unchecked((ulong)-1);

    public int Index { get; } = index;
    public int Tile { get; set; }=-1;

    public int pCount { get; private set; }
    ulong PTiles
    {
        get => pTiles; set
        {
            if (pTiles == value) return;
            pTiles = value;
            pCount = value.BitCount();
        }
    }
}
