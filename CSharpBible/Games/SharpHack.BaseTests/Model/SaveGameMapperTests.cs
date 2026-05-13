using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Data;
using SharpHack.Base.Model;
using SharpHack.BaseItems.Model;
using SharpHack.Persist;

namespace SharpHack.BaseTests.Model;

[TestClass]
public class SaveGameMapperTests
{
    [TestMethod]
    public void MapToSaveGame_And_MapToRuntimeState_RoundTripsRepresentativeState()
    {
        var map = new Map(4, 4);
        map[0, 0].Type = TileType.Room;
        map[0, 0].IsExplored = true;
        map[0, 0].IsVisible = true;
        map[1, 1].Type = TileType.DoorClosed;
        map[1, 1].IsExplored = true;
        map[2, 2].Type = TileType.StairsDown;
        map[2, 2].IsExplored = true;
        map[2, 2].IsVisible = true;

        var sword = new Weapon
        {
            Name = "Sword",
            Description = "A sharp blade",
            Position = new Point(0, 0),
            Symbol = '/',
            Color = ConsoleColor.Cyan,
            Weight = 3.5,
            AttackBonus = 5
        };

        var armor = new Armor
        {
            Name = "Leather Armor",
            Description = "Simple armor",
            Position = new Point(0, 0),
            Symbol = '[',
            Color = ConsoleColor.Yellow,
            Weight = 7.0,
            DefenseBonus = 2
        };

        var floorItem = new Item
        {
            Name = "Potion",
            Description = "Healing potion",
            Position = new Point(2, 2),
            Symbol = '!',
            Color = ConsoleColor.Red,
            Weight = 0.5,
            IsStackable = true,
            Quantity = 2
        };

        var player = new Creature
        {
            Name = "Hero",
            Description = "Player character",
            Position = new Point(0, 0),
            Symbol = '@',
            Color = ConsoleColor.White,
            HP = 42,
            MaxHP = 50,
            BaseAttack = 10,
            BaseDefense = 3,
            Speed = 12,
            MainHand = sword,
            Body = armor
        };
        player.Inventory.Add(sword);
        player.Inventory.Add(armor);
        map[player.Position].Creature = player;

        var enemy = new Creature
        {
            Name = "Goblin",
            Description = "Cave goblin",
            Position = new Point(1, 2),
            Symbol = 'g',
            Color = ConsoleColor.Green,
            HP = 12,
            MaxHP = 20,
            BaseAttack = 4,
            BaseDefense = 1,
            Speed = 9
        };
        map[enemy.Position].Creature = enemy;
        map[floorItem.Position].Items.Add(floorItem);

        var createdUtc = new DateTimeOffset(2025, 1, 2, 3, 4, 5, TimeSpan.Zero);
        var updatedUtc = createdUtc.AddMinutes(10);
        var runId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var saveGame = SaveGameMapper.MapToSaveGame(
            map,
            player,
            new[] { enemy },
            level: 3,
            runState: GameRunState.Running,
            turnsTaken: 27,
            victoryObjective: "Amulet of JoCarneer",
            completionSummary: string.Empty,
            createdUtc: createdUtc,
            updatedUtc: updatedUtc,
            runId: runId,
            sequenceNumber: 4,
            source: "Tests",
            diagnosticsTag: "round-trip");

        var restored = SaveGameMapper.MapToRuntimeState(saveGame);

        Assert.IsNotNull(restored.Map);
        Assert.IsNotNull(restored.Player);
        Assert.AreEqual(3, restored.Level);
        Assert.AreEqual(GameRunState.Running, restored.RunState);
        Assert.AreEqual(27, restored.TurnsTaken);
        Assert.AreEqual("Amulet of JoCarneer", restored.VictoryObjective);
        Assert.AreEqual(createdUtc, saveGame.Metadata.CreatedUtc);
        Assert.AreEqual(updatedUtc, saveGame.Metadata.UpdatedUtc);
        Assert.AreEqual(runId, saveGame.Recovery.RunId);
        Assert.AreEqual(4, saveGame.Recovery.SequenceNumber);

        Assert.AreEqual(TileType.Room, restored.Map![0, 0].Type);
        Assert.IsTrue(restored.Map[0, 0].IsExplored);
        Assert.IsTrue(restored.Map[0, 0].IsVisible);
        Assert.AreEqual(TileType.DoorClosed, restored.Map[1, 1].Type);
        Assert.AreEqual(TileType.StairsDown, restored.Map[2, 2].Type);

        Assert.AreEqual("Hero", restored.Player!.Name);
        Assert.AreEqual(42, restored.Player.HP);
        Assert.AreEqual(50, restored.Player.MaxHP);
        Assert.AreEqual(10, restored.Player.BaseAttack);
        Assert.AreEqual(3, restored.Player.BaseDefense);
        Assert.AreEqual(2, restored.Player.Inventory.Count);
        Assert.IsNotNull(restored.Player.MainHand);
        Assert.IsNotNull(restored.Player.Body);
        Assert.AreSame(restored.Player.MainHand, restored.Player.Inventory[0]);
        Assert.AreSame(restored.Player.Body, restored.Player.Inventory[1]);
        Assert.AreEqual(5, restored.Player.MainHand.AttackBonus);
        Assert.AreEqual(2, restored.Player.Body.DefenseBonus);

        Assert.AreEqual(1, restored.Enemies.Count);
        Assert.AreEqual("Goblin", restored.Enemies[0].Name);
        Assert.AreEqual(new Point(1, 2), restored.Enemies[0].Position);
        Assert.AreEqual(12, restored.Enemies[0].HP);

        Assert.AreEqual(1, restored.Map[2, 2].Items.Count);
        Assert.AreEqual("Potion", restored.Map[2, 2].Items[0].Name);
        Assert.AreEqual(2, restored.Map[2, 2].Items[0].Quantity);
        Assert.IsTrue(restored.Map[2, 2].Items[0].IsStackable);
    }

    [TestMethod]
    public void MapToRuntimeState_ThrowsForUnsupportedSaveVersion()
    {
        var saveGame = new SharpHack.Persist.Models.SaveGameDto
        {
            Metadata = new SharpHack.Persist.Models.SaveGameMetadataDto
            {
                SaveVersion = 99,
                CreatedUtc = DateTimeOffset.UtcNow,
                UpdatedUtc = DateTimeOffset.UtcNow
            },
            Run = new SharpHack.Persist.Models.SaveRunStateDto
            {
                CurrentLevel = 1,
                RunState = GameRunState.Running
            },
            Player = new SharpHack.Persist.Models.SaveCreatureDto
            {
                Id = Guid.NewGuid().ToString("D"),
                CreatureType = typeof(Creature).FullName ?? nameof(Creature),
                Name = "Hero",
                Position = new SharpHack.Persist.Models.SavePointDto()
            },
            Levels =
            [
                new SharpHack.Persist.Models.SaveLevelDto
                {
                    LevelNumber = 1,
                    Map = new SharpHack.Persist.Models.SaveMapDto { Width = 1, Height = 1, Tiles = [] },
                    Creatures = [],
                    Items = []
                }
            ]
        };

        try
        {
            SaveGameMapper.MapToRuntimeState(saveGame);
            Assert.Fail("Expected a NotSupportedException for an unsupported save version.");
        }
        catch (NotSupportedException)
        {
        }
    }
}
