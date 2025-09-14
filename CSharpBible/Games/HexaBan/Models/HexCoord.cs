using HexaBan.Models.Interfaces;
using System;

namespace HexaBan.Models;

public struct HexCoord
{
    public int Q { get; }
    public int R { get; }

    public HexCoord(int q, int r)
    {
        Q = q;
        R = r;
    }

    public HexCoord Move(HexDirection dir)
    {
        // Axial coordinates for hex grids
        return dir switch
        {
            HexDirection.UpRight => new HexCoord(Q + 1, R - 1),
            HexDirection.Right => new HexCoord(Q + 1, R),
            HexDirection.DownRight => new HexCoord(Q, R + 1),
            HexDirection.DownLeft => new HexCoord(Q - 1, R + 1),
            HexDirection.Left => new HexCoord(Q - 1, R),
            HexDirection.UpLeft => new HexCoord(Q, R - 1),
            _ => this
        };
    }

    public override bool Equals(object obj) =>
        obj is HexCoord other && Q == other.Q && R == other.R;

    public override int GetHashCode() => HashCode.Combine(Q, R);
}
