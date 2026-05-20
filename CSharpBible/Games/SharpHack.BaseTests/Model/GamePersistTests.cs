using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using SharpHack.BaseItems.Model;
using SharpHack.Persist;
using SharpHack.Persist.Models;

namespace SharpHack.BaseTests.Model;

[TestClass]
public class GamePersistTests
{
    [TestMethod]
    public void SaveLevel_And_LoadLevel_RoundTripsFileBackedState()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var persist = new GamePersist(tempDirectory, new SaveFileWriter(), new SaveFileReader());
            var state = CreateRepresentativeState();

            persist.SaveLevel(3, state.Map, state.Player, state.Enemies);

            var saveFilePath = Path.Combine(tempDirectory, "level-3.shs");
            Assert.IsTrue(File.Exists(saveFilePath));

            var result = persist.LoadLevel(3, out var loadedMap, out var loadedEnemies);

            Assert.IsTrue(result);
            Assert.IsNotNull(loadedMap);
            Assert.IsNotNull(loadedEnemies);
            Assert.AreEqual(TileType.Room, loadedMap[0, 0].Type);
            Assert.IsTrue(loadedMap[0, 0].IsExplored);
            Assert.IsTrue(loadedMap[0, 0].IsVisible);
            Assert.AreEqual(TileType.DoorClosed, loadedMap[1, 1].Type);
            Assert.AreEqual(TileType.StairsDown, loadedMap[2, 2].Type);
            Assert.AreEqual(1, loadedMap[2, 2].Items.Count);
            Assert.AreEqual("Potion", loadedMap[2, 2].Items[0].Name);
            Assert.AreEqual(2, loadedMap[2, 2].Items[0].Quantity);
            Assert.IsTrue(loadedMap[2, 2].Items[0].IsStackable);

            Assert.IsNotNull(loadedMap[0, 0].Creature);
            Assert.AreEqual("Hero", loadedMap[0, 0].Creature.Name);
            Assert.AreEqual(42, loadedMap[0, 0].Creature.HP);
            Assert.AreEqual(2, loadedMap[0, 0].Creature.Inventory.Count);
            Assert.IsNotNull(loadedMap[0, 0].Creature.MainHand);
            Assert.IsNotNull(loadedMap[0, 0].Creature.Body);
            Assert.AreEqual(5, loadedMap[0, 0].Creature.MainHand.AttackBonus);
            Assert.AreEqual(2, loadedMap[0, 0].Creature.Body.DefenseBonus);

            Assert.AreEqual(1, loadedEnemies.Count);
            Assert.AreEqual("Goblin", loadedEnemies[0].Name);
            Assert.AreEqual(new Point(1, 2), loadedEnemies[0].Position);
            Assert.AreEqual(12, loadedEnemies[0].HP);
            Assert.AreSame(loadedEnemies[0], loadedMap[1, 2].Creature);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [TestMethod]
    public void LoadLevel_ReturnsFalse_ForMissingSaveFile()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var persist = new GamePersist(tempDirectory, new SaveFileWriter(), new SaveFileReader());

            var result = persist.LoadLevel(9, out var loadedMap, out var loadedEnemies);

            Assert.IsFalse(result);
            Assert.IsNull(loadedMap);
            Assert.IsNull(loadedEnemies);
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [TestMethod]
    public void LoadLevel_ThrowsCorruptException_ForCorruptSaveFile()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var persist = new GamePersist(tempDirectory, new SaveFileWriter(), new SaveFileReader());
            var saveFilePath = Path.Combine(tempDirectory, "level-1.shs");
            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(saveFilePath, "not-a-gzip-save");

            try
            {
                persist.LoadLevel(1, out _, out _);
                Assert.Fail("Expected a SaveFileCorruptException for corrupt save contents.");
            }
            catch (SaveFileCorruptException)
            {
            }
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    [TestMethod]
    public void LoadLevel_ThrowsIncompatibleException_ForUnsupportedSaveVersion()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            Directory.CreateDirectory(tempDirectory);
            var writer = new SaveFileWriter();
            var persist = new GamePersist(tempDirectory, writer, new SaveFileReader());
            var saveFilePath = Path.Combine(tempDirectory, "level-1.shs");

            writer.Write(saveFilePath, CreateUnsupportedVersionSave());

            try
            {
                persist.LoadLevel(1, out _, out _);
                Assert.Fail("Expected a SaveFileIncompatibleException for an unsupported save version.");
            }
            catch (SaveFileIncompatibleException)
            {
            }
        }
        finally
        {
            DeleteDirectoryIfExists(tempDirectory);
        }
    }

    private static string CreateTempDirectory()
    {
        return Path.Combine(Path.GetTempPath(), "SharpHack.BaseTests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    private static (Map Map, Creature Player, IList<ICreature> Enemies) CreateRepresentativeState()
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

        return (map, player, new List<ICreature> { enemy });
    }

    private static SaveGameDto CreateUnsupportedVersionSave()
    {
        return new SaveGameDto
        {
            Metadata = new SaveGameMetadataDto
            {
                SaveVersion = 99,
                CreatedUtc = DateTimeOffset.UtcNow,
                UpdatedUtc = DateTimeOffset.UtcNow
            },
            Run = new SaveRunStateDto
            {
                CurrentLevel = 1,
                RunState = GameRunState.Running
            },
            Player = new SaveCreatureDto
            {
                Id = Guid.NewGuid().ToString("D"),
                CreatureType = typeof(Creature).FullName ?? nameof(Creature),
                Name = "Hero",
                Position = new SavePointDto()
            },
            Levels =
            [
                new SaveLevelDto
                {
                    LevelNumber = 1,
                    Map = new SaveMapDto
                    {
                        Width = 1,
                        Height = 1,
                        Tiles =
                        [
                            new SaveTileDto
                            {
                                Position = new SavePointDto(),
                                Type = TileType.Room,
                                IsExplored = true,
                                IsVisible = true,
                                CreatureId = string.Empty,
                                ItemIds = []
                            }
                        ]
                    },
                    Creatures = [],
                    Items = []
                }
            ]
        };
    }
}
