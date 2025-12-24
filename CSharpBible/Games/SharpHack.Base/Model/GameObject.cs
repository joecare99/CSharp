using System;

namespace SharpHack.Base.Model;

public abstract class GameObject
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Point Position { get; set; }
    public char Symbol { get; set; } = '?';
    public ConsoleColor Color { get; set; } = ConsoleColor.White;
}
