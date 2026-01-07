using SharpHack.Base.Interfaces;
using System;

namespace SharpHack.Base.Model;

public abstract class GameObject : IGameObject
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    protected Point _oldPosition;

    public Point Position { get=>field; set { _oldPosition = field;field = value; } }
    public char Symbol { get; set; } = '?';
    public ConsoleColor Color { get; set; } = ConsoleColor.White;
}
