namespace VectorGfx.Models.Interfaces;

public interface IPositionedObject : IHasPoint
{
    int Z { get; set; }
    int ZRot { get; set; }

    int X { get; set; }
    int Y { get; set; }
}