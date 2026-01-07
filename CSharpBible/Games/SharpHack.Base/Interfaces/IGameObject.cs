using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces
{
    public interface IGameObject
    {
        ConsoleColor Color { get; set; }
        string Description { get; set; }
        Guid Id { get; }
        string Name { get; set; }
        Point Position { get; set; }
        char Symbol { get; set; }
    }
}