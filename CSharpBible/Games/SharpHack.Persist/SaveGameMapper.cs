using System;
using System.Collections.Generic;
using System.Linq;
using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using SharpHack.BaseItems.Model;
using SharpHack.Persist.Models;

namespace SharpHack.Persist;

/// <summary>
/// Maps runtime domain state into serializable save DTOs.
/// </summary>
public static class SaveGameMapper
{
    private const int SupportedSaveVersion = 1;

    /// <summary>
    /// Creates a save DTO from the provided runtime state.
    /// </summary>
    /// <param name="map">The active level map.</param>
    /// <param name="player">The player creature.</param>
    /// <param name="enemies">The active level enemies.</param>
    /// <param name="level">The current level number.</param>
    /// <param name="runState">The current run lifecycle state.</param>
    /// <param name="turnsTaken">The number of turns completed.</param>
    /// <param name="victoryObjective">The current victory objective label.</param>
    /// <param name="completionSummary">The current completion summary.</param>
    /// <param name="createdUtc">Optional created timestamp override.</param>
    /// <param name="updatedUtc">Optional updated timestamp override.</param>
    /// <param name="runId">Optional stable run identifier.</param>
    /// <param name="sequenceNumber">Optional save sequence number.</param>
    /// <param name="source">Optional source identifier.</param>
    /// <param name="diagnosticsTag">Optional diagnostics tag.</param>
    /// <returns>A serializable save DTO representing the supplied runtime state.</returns>
    public static SaveGameDto MapToSaveGame(
        IMap map,
        ICreature player,
        IEnumerable<ICreature> enemies,
        int level,
        GameRunState runState,
        int turnsTaken,
        string victoryObjective,
        string completionSummary,
        DateTimeOffset? createdUtc = null,
        DateTimeOffset? updatedUtc = null,
        Guid? runId = null,
        long sequenceNumber = 0,
        string source = "SharpHack",
        string diagnosticsTag = "")
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        if (enemies == null)
        {
            throw new ArgumentNullException(nameof(enemies));
        }

        var enemyList = enemies.ToList();
        var allItems = GetDistinctItems(map, allCreatures: null).ToList();
        var allCreatures = new List<ICreature>(enemyList.Count + 1) { player };
        allCreatures.AddRange(enemyList);
        allItems = GetDistinctItems(map, allCreatures).ToList();
        var now = updatedUtc ?? DateTimeOffset.UtcNow;
        var created = createdUtc ?? now;

        return new SaveGameDto
        {
            Metadata = new SaveGameMetadataDto
            {
                SaveVersion = 1,
                CreatedUtc = created,
                UpdatedUtc = now,
                Source = source,
                DiagnosticsTag = diagnosticsTag
            },
            Recovery = new SaveRecoveryMetadataDto
            {
                RunId = runId ?? Guid.NewGuid(),
                SequenceNumber = sequenceNumber,
                Checksum = string.Empty,
                HasRecoveryCopy = false,
                RecoveryUpdatedUtc = null
            },
            Run = new SaveRunStateDto
            {
                RunState = runState,
                HasWon = runState == GameRunState.Victory,
                CurrentLevel = level,
                TurnsTaken = turnsTaken,
                VictoryObjective = victoryObjective ?? string.Empty,
                CompletionSummary = completionSummary ?? string.Empty
            },
            Player = MapCreature(player),
            Levels =
            [
                new SaveLevelDto
                {
                    LevelNumber = level,
                    Map = MapMap(map),
                    Creatures = allCreatures.Select(MapCreature).ToList(),
                    Items = allItems.Select(MapItem).ToList()
                }
            ]
        };
    }

    /// <summary>
    /// Restores runtime domain state from a save DTO.
    /// </summary>
    /// <param name="saveGame">The save payload to restore.</param>
    /// <returns>The reconstructed runtime state.</returns>
    public static RestoreGameState MapToRuntimeState(SaveGameDto saveGame)
    {
        if (saveGame == null)
        {
            throw new ArgumentNullException(nameof(saveGame));
        }

        if (saveGame.Metadata == null)
        {
            throw new InvalidOperationException("Save metadata is required.");
        }

        if (saveGame.Metadata.SaveVersion != SupportedSaveVersion)
        {
            throw new NotSupportedException($"Save version '{saveGame.Metadata.SaveVersion}' is not supported.");
        }

        if (saveGame.Run == null)
        {
            throw new InvalidOperationException("Run state is required.");
        }

        if (saveGame.Player == null)
        {
            throw new InvalidOperationException("Player state is required.");
        }

        var levelDto = saveGame.Levels.SingleOrDefault(level => level.LevelNumber == saveGame.Run.CurrentLevel)
            ?? saveGame.Levels.SingleOrDefault()
            ?? throw new InvalidOperationException("At least one saved level is required.");

        if (levelDto.Map == null)
        {
            throw new InvalidOperationException("Saved map state is required.");
        }

        var itemLookup = RestoreItems(levelDto.Items ?? []);
        var creatureLookup = RestoreCreatures(levelDto.Creatures ?? [], itemLookup);

        if (!creatureLookup.TryGetValue(saveGame.Player.Id, out var player))
        {
            player = RestoreCreature(saveGame.Player, itemLookup);
            creatureLookup[saveGame.Player.Id] = player;
        }

        var map = RestoreMap(levelDto.Map, levelDto, itemLookup, creatureLookup);
        var enemies = creatureLookup
            .Where(pair => pair.Key != saveGame.Player.Id)
            .Select(pair => pair.Value)
            .ToList();

        return new RestoreGameState
        {
            Map = map,
            Player = player,
            Enemies = enemies,
            Level = saveGame.Run.CurrentLevel,
            RunState = saveGame.Run.RunState,
            TurnsTaken = saveGame.Run.TurnsTaken,
            VictoryObjective = saveGame.Run.VictoryObjective,
            CompletionSummary = saveGame.Run.CompletionSummary
        };
    }

    private static SaveMapDto MapMap(IMap map)
    {
        return new SaveMapDto
        {
            Width = map.Width,
            Height = map.Height,
            Tiles = GetTiles(map)
                .Select(MapTile)
                .ToList()
        };
    }

    private static Map RestoreMap(
        SaveMapDto mapDto,
        SaveLevelDto levelDto,
        IDictionary<string, IItem> itemLookup,
        IDictionary<string, ICreature> creatureLookup)
    {
        var map = new Map(mapDto.Width, mapDto.Height);

        foreach (var tileDto in mapDto.Tiles)
        {
            var tile = map[tileDto.Position.X, tileDto.Position.Y];
            tile.Type = tileDto.Type;
            tile.IsVisible = tileDto.IsVisible;
            tile.IsExplored = tileDto.IsExplored;
            tile.Items.Clear();

            foreach (var itemId in tileDto.ItemIds)
            {
                if (!itemLookup.TryGetValue(itemId, out var item))
                {
                    throw new InvalidOperationException($"Tile item '{itemId}' could not be restored.");
                }

                item.Position = tile.Position;
                tile.Items.Add(item);
            }

            if (!string.IsNullOrWhiteSpace(tileDto.CreatureId))
            {
                if (!creatureLookup.TryGetValue(tileDto.CreatureId, out var creature))
                {
                    throw new InvalidOperationException($"Tile creature '{tileDto.CreatureId}' could not be restored.");
                }

                creature.Position = tile.Position;
                tile.Creature = creature;
            }
        }

        foreach (var creatureDto in levelDto.Creatures)
        {
            if (!creatureLookup.TryGetValue(creatureDto.Id, out var creature))
            {
                continue;
            }

            if (map.IsValid(creature.Position))
            {
                map[creature.Position].Creature = creature;
            }
        }

        return map;
    }

    private static SaveTileDto MapTile(Tile tile)
    {
        return new SaveTileDto
        {
            Position = MapPoint(tile.Position),
            Type = tile.Type,
            IsVisible = tile.IsVisible,
            IsExplored = tile.IsExplored,
            CreatureId = tile.Creature is null ? string.Empty : tile.Creature.Id.ToString("D"),
            ItemIds = tile.Items.Select(item => item.Id.ToString("D")).ToList()
        };
    }

    private static Dictionary<string, ICreature> RestoreCreatures(IEnumerable<SaveCreatureDto> creatureDtos, IDictionary<string, IItem> itemLookup)
    {
        var creatures = new Dictionary<string, ICreature>(StringComparer.OrdinalIgnoreCase);

        foreach (var creatureDto in creatureDtos)
        {
            creatures[creatureDto.Id] = RestoreCreature(creatureDto, itemLookup);
        }

        return creatures;
    }

    private static ICreature RestoreCreature(SaveCreatureDto creatureDto, IDictionary<string, IItem> itemLookup)
    {
        if (!IsSupportedCreatureType(creatureDto.CreatureType))
        {
            throw new NotSupportedException($"Creature type '{creatureDto.CreatureType}' is not supported.");
        }

        var creature = new Creature
        {
            Name = creatureDto.Name,
            Description = creatureDto.Description,
            Position = RestorePoint(creatureDto.Position),
            Symbol = creatureDto.Symbol,
            Color = RestoreColor(creatureDto.Color),
            HP = creatureDto.HP,
            MaxHP = creatureDto.MaxHP,
            BaseAttack = creatureDto.BaseAttack,
            BaseDefense = creatureDto.BaseDefense,
            Speed = creatureDto.Speed
        };

        creature.Inventory.Clear();
        foreach (var itemId in creatureDto.InventoryItemIds)
        {
            if (!itemLookup.TryGetValue(itemId, out var item))
            {
                throw new InvalidOperationException($"Inventory item '{itemId}' could not be restored.");
            }

            creature.Inventory.Add(item);
        }

        if (!string.IsNullOrWhiteSpace(creatureDto.Equipment.MainHandItemId))
        {
            if (!itemLookup.TryGetValue(creatureDto.Equipment.MainHandItemId, out var mainHandItem) || mainHandItem is not IWeapon weapon)
            {
                throw new InvalidOperationException($"Main-hand item '{creatureDto.Equipment.MainHandItemId}' could not be restored.");
            }

            creature.MainHand = weapon;
        }

        if (!string.IsNullOrWhiteSpace(creatureDto.Equipment.BodyItemId))
        {
            if (!itemLookup.TryGetValue(creatureDto.Equipment.BodyItemId, out var bodyItem) || bodyItem is not IArmor armor)
            {
                throw new InvalidOperationException($"Body item '{creatureDto.Equipment.BodyItemId}' could not be restored.");
            }

            creature.Body = armor;
        }

        return creature;
    }

    private static SaveCreatureDto MapCreature(ICreature creature)
    {
        return new SaveCreatureDto
        {
            Id = creature.Id.ToString("D"),
            CreatureType = creature.GetType().FullName ?? creature.GetType().Name,
            Name = creature.Name,
            Description = creature.Description,
            Position = MapPoint(creature.Position),
            Symbol = creature.Symbol,
            Color = creature.Color.ToString(),
            HP = creature.HP,
            MaxHP = creature.MaxHP,
            BaseAttack = creature.BaseAttack,
            BaseDefense = creature.BaseDefense,
            Speed = creature.Speed,
            InventoryItemIds = creature.Inventory.Select(item => item.Id.ToString("D")).ToList(),
            Equipment = new SaveEquipmentDto
            {
                MainHandItemId = creature.MainHand?.Id.ToString("D") ?? string.Empty,
                BodyItemId = creature.Body?.Id.ToString("D") ?? string.Empty
            }
        };
    }

    private static SaveItemDto MapItem(IItem item)
    {
        var itemDto = new SaveItemDto
        {
            Id = item.Id.ToString("D"),
            ItemType = item.GetType().FullName ?? item.GetType().Name,
            Name = item.Name,
            Description = item.Description,
            Position = MapPoint(item.Position),
            Symbol = item.Symbol,
            Color = item.Color.ToString(),
            Weight = item.Weight,
            IsStackable = item.IsStackable,
            Quantity = item.Quantity,
            AttackBonus = item is IWeapon weapon ? weapon.AttackBonus : null,
            DefenseBonus = item is IArmor armor ? armor.DefenseBonus : null
        };

        if (item is IContainerItem containerItem)
        {
            itemDto.ContainedItems = containerItem.Items.Select(MapItem).ToList();
        }

        return itemDto;
    }

    private static Dictionary<string, IItem> RestoreItems(IEnumerable<SaveItemDto> itemDtos)
    {
        var items = new Dictionary<string, IItem>(StringComparer.OrdinalIgnoreCase);

        foreach (var itemDto in itemDtos)
        {
            items[itemDto.Id] = RestoreItem(itemDto);
        }

        return items;
    }

    private static IItem RestoreItem(SaveItemDto itemDto)
    {
        var item = CreateItem(itemDto);
        item.Name = itemDto.Name;
        item.Description = itemDto.Description;
        item.Position = RestorePoint(itemDto.Position);
        item.Symbol = itemDto.Symbol;
        item.Color = RestoreColor(itemDto.Color);
        item.Weight = itemDto.Weight;
        item.IsStackable = itemDto.IsStackable;
        item.Quantity = itemDto.Quantity;

        return item;
    }

    private static Item CreateItem(SaveItemDto itemDto)
    {
        if (IsWeaponType(itemDto.ItemType))
        {
            return new Weapon
            {
                AttackBonus = itemDto.AttackBonus ?? 0
            };
        }

        if (IsArmorType(itemDto.ItemType))
        {
            return new Armor
            {
                DefenseBonus = itemDto.DefenseBonus ?? 0
            };
        }

        if (IsPlainItemType(itemDto.ItemType))
        {
            return new Item();
        }

        throw new NotSupportedException($"Item type '{itemDto.ItemType}' is not supported.");
    }

    private static SavePointDto MapPoint(Point point)
    {
        return new SavePointDto
        {
            X = point.X,
            Y = point.Y
        };
    }

    private static Point RestorePoint(SavePointDto pointDto)
    {
        return new Point(pointDto.X, pointDto.Y);
    }

    private static ConsoleColor RestoreColor(string colorName)
    {
        if (Enum.TryParse(colorName, ignoreCase: true, out ConsoleColor color))
        {
            return color;
        }

        return ConsoleColor.White;
    }

    private static bool IsSupportedCreatureType(string creatureType)
    {
        return string.Equals(creatureType, typeof(Creature).FullName, StringComparison.Ordinal)
            || string.Equals(creatureType, typeof(Creature).Name, StringComparison.Ordinal);
    }

    private static bool IsPlainItemType(string itemType)
    {
        return string.Equals(itemType, typeof(Item).FullName, StringComparison.Ordinal)
            || string.Equals(itemType, typeof(Item).Name, StringComparison.Ordinal);
    }

    private static bool IsWeaponType(string itemType)
    {
        return string.Equals(itemType, typeof(Weapon).FullName, StringComparison.Ordinal)
            || string.Equals(itemType, typeof(Weapon).Name, StringComparison.Ordinal);
    }

    private static bool IsArmorType(string itemType)
    {
        return string.Equals(itemType, typeof(Armor).FullName, StringComparison.Ordinal)
            || string.Equals(itemType, typeof(Armor).Name, StringComparison.Ordinal);
    }

    private static IEnumerable<Tile> GetTiles(IMap map)
    {
        for (var x = 0; x < map.Width; x++)
        {
            for (var y = 0; y < map.Height; y++)
            {
                yield return map[x, y];
            }
        }
    }

    private static IEnumerable<IItem> GetDistinctItems(IMap map, IEnumerable<ICreature>? allCreatures)
    {
        var seenIds = new HashSet<Guid>();
        foreach (var tile in GetTiles(map))
        {
            foreach (var item in tile.Items)
            {
                if (seenIds.Add(item.Id))
                {
                    yield return item;
                }
            }
        }

        if (allCreatures == null)
        {
            yield break;
        }

        foreach (var creature in allCreatures)
        {
            foreach (var item in creature.Inventory)
            {
                foreach (var distinctItem in GetNestedDistinctItems(item, seenIds))
                {
                    yield return distinctItem;
                }
            }

            if (creature.MainHand != null)
            {
                foreach (var distinctItem in GetNestedDistinctItems(creature.MainHand, seenIds))
                {
                    yield return distinctItem;
                }
            }

            if (creature.Body != null)
            {
                foreach (var distinctItem in GetNestedDistinctItems(creature.Body, seenIds))
                {
                    yield return distinctItem;
                }
            }
        }
    }

    private static IEnumerable<IItem> GetNestedDistinctItems(IItem item, ISet<Guid> seenIds)
    {
        if (seenIds.Add(item.Id))
        {
            yield return item;
        }

        if (item is IContainerItem containerItem)
        {
            foreach (var containedItem in containerItem.Items)
            {
                foreach (var nestedItem in GetNestedDistinctItems(containedItem, seenIds))
                {
                    yield return nestedItem;
                }
            }
        }
    }
}
