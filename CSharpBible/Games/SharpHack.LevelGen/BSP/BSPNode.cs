using System.Drawing;

namespace SharpHack.LevelGen.BSP;

public class BSPNode
{
    public Rectangle Bounds { get; set; }
    public Rectangle Room { get; set; } // The actual room inside the bounds
    public BSPNode? Left { get; set; }
    public BSPNode? Right { get; set; }
    public BSPNode? Parent { get; set; }

    public bool IsLeaf => Left == null && Right == null;

    public BSPNode(Rectangle bounds)
    {
        Bounds = bounds;
    }
}
