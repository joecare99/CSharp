using SharpHack.Base.Interfaces;

namespace SharpHack.Base.Model;

public class Item : GameObject, IItem
{
    public double Weight { get; set; }
    public bool IsStackable { get; set; }
    public int Quantity { get; set; } = 1;
}
