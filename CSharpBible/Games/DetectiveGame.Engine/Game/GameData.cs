using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Engine.Game;

public static class GameData
{
    public static IReadOnlyList<Card> Persons { get; } = new List<Card>
    {
        new("Person1", CardCategory.Person),
        new("Person2", CardCategory.Person),
        new("Person3", CardCategory.Person),
        new("Person4", CardCategory.Person),
        new("Person5", CardCategory.Person),
        new("Person6", CardCategory.Person),
        new("Person7", CardCategory.Person),
        new("Person8", CardCategory.Person),
        new("Person9", CardCategory.Person)
    };

    public static IReadOnlyList<Card> Weapons { get; } = new List<Card>
    {
        new("Weapon1", CardCategory.Weapon),
        new("Weapon2", CardCategory.Weapon),
        new("Weapon3", CardCategory.Weapon),
        new("Weapon4", CardCategory.Weapon),
        new("Weapon5", CardCategory.Weapon),
        new("Weapon6", CardCategory.Weapon),
        new("Weapon7", CardCategory.Weapon),
        new("Weapon8", CardCategory.Weapon),
        new("Weapon9", CardCategory.Weapon)
    };

    public static IReadOnlyList<Card> Rooms { get; } = new List<Card>
    {
        new("Room1", CardCategory.Room),
        new("Room2", CardCategory.Room),
        new("Room3", CardCategory.Room),
        new("Room4", CardCategory.Room),
        new("Room5", CardCategory.Room),
        new("Room6", CardCategory.Room),
        new("Room7", CardCategory.Room),
        new("Room8", CardCategory.Room),
        new("Room9", CardCategory.Room)
    };

    public static IReadOnlyList<Card> All => Persons.Concat(Weapons).Concat(Rooms).ToList();
}