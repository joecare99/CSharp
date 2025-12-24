namespace SharpHack.Base.Model;

public class Item : GameObject
{
    public double Weight { get; set; }
    public bool IsStackable { get; set; }
    public int Quantity { get; set; } = 1;
}
