using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCAMS.Core.Components.Coloring;

public record CColorCube(int Nr, string Label="",params Color[] List)
{
    public static readonly CColorCube Empty = new CColorCube();

    public CColorCube() : this(0, string.Empty, Array.Empty<Color>())
    {
    }

    public static CColorCube Create(int Nr=0, string Label="", params Color[] List)=> new CColorCube(Nr, Label, List);
}
