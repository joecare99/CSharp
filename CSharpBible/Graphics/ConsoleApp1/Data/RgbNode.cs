using System.Drawing;

namespace ColorVis.Data;
class RgbNode
{
    public int R, G, B;           // 0..7
    public float H, S, L;         // 0..1
    public Color ColorRgb;
    public List<int> Neighbors = new List<int>();
    // 3D position in HSL space
    public Vector3 Pos3 => new Vector3((float)(0.5+0.5*S*Math.Cos(H*Math.PI*2 )), 1.0f-L, (float)(0.5 + 0.5 * S * Math.Sin(H * Math.PI * 2)));
}
