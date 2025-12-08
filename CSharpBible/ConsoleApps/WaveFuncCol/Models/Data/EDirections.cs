using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunCollapse.Models.Data;

public enum EDirections:int
{ 
    UpLeft,
    Up,
    UpRight,
    Left,
    None,
    Right,
    DownLeft,
    Down,
    DownRight,
} 

public static class EDirectionsExtensions
{
    public static EDirections Opposite(this EDirections direction) 
        => (EDirections)(((int)EDirections.DownRight - (int)direction));

    public static EDirections Neutral => EDirections.None;
}
