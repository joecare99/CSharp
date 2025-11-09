using System;
using System.Windows;
using System.Windows.Media;

namespace BoxFlight.World;

// Ported obstacle geometry: Cylinder/Box HitTest + Reflect + RenderEntry

public enum ObjType { Cylinder, Box }

public sealed class RenderEntry
{
    public int Oo1 = -1, Oo2 = -1;
    public int Light, Height, Shad;
    public Color Color = Colors.White;
    public int Light2, Height2;
    public Color Color2 = Colors.White;
}

public abstract class Obstacle
{
    public Point Pos;
    public int Size;
    public int Rot;
    public Point Vp;
    public bool Visible, Hit;
    public Color Color;

    public abstract bool HitTest(Point cPoint, Vector renDir, out float rDistQ, out float r);
    public abstract Vector Reflect(Vector renDir, float rr, out float xx);
}
