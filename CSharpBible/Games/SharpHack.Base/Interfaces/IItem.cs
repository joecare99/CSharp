namespace SharpHack.Base.Interfaces;

public interface IItem : IGameObject
{
    bool IsStackable { get; set; }
    int Quantity { get; set; }
    double Weight { get; set; }
}