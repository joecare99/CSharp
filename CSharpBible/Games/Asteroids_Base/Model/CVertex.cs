using Asteroids.Model.Interfaces;

namespace Asteroids.Model;

public class CVertex(float Angle,float Length) : IVertex
{
    public float Angle { get; } = Angle;
    public float Length { get; } = Length;
}