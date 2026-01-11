using System;

namespace SharpHack.ViewModel;

/// <summary>
/// Provides NetHack tile indices that the view layers can translate to glyphs or sprites.
/// </summary>
public enum DisplayTile
{
    /// <summary>
    /// Represents an empty cell / void tile.
    /// </summary>
    Empty = 829,

    /// <summary>
    /// Classic NetHack goblin (index 60).
    /// </summary>
    Goblin = 60,

    /// <summary>
    /// Archeologist player tile (index 335).
    /// </summary>
    Archaeologist = 335,

    /// <summary>
    /// Basic sword pickup (index 432).
    /// </summary>
    Sword = 432,

    /// <summary>
    /// Basic armor pickup (index 509).
    /// </summary>
    Armor = 509,

    /// <summary>
    /// Specifies the wall north-south orientation type.
    /// </summary>
    Wall_NS = 830,

    /// <summary>
    /// Horizontal wall (index 831).
    /// </summary>
    Wall_EW = 831,

    /// <summary>
    /// Specifies the wall east-south orientation type.
    /// </summary>
    Wall_EN = 832,

    /// <summary>
    /// Specifies the wall west-south orientation type.
    /// </summary>
    Wall_NW = 833,

    /// <summary>
    /// Specifies the wall north-east orientation type.
    /// </summary>
    Wall_ES = 834,

    /// <summary>
    /// Represents a wall segment oriented in the northwest direction.
    /// </summary>
    Wall_WS = 835,

    /// <summary>
    /// Specifies a wall facing east-northwest-south orientation in the architectural model.
    /// </summary>
    Wall_ENWS = 836,

    Wall_ENW = 837,

    Wall_EWS = 838,

    Wall_NWS = 839,

    Wall_ENS = 840,

    /// <summary>
    /// Open door (index 843).
    /// </summary>
    Door_Open_EW = 843,
    /// <summary>
    /// Open door (index 843).
    /// </summary>
    Door_Open_NS = 842,

    /// <summary>
    /// Closed door (index 842).
    /// </summary>
    Door_Closed_EW= 845,

    /// <summary>
    /// Closed door (index 842).
    /// </summary>
    Door_Closed_NS = 844,

    /// <summary>
    /// Lit floor (index 848).
    /// </summary>
    Floor_Lit = 848,

    Floor_Dark = 849,
    /// <summary>
    /// Stairs leading up (index 851).
    /// </summary>
    Stairs_Up = 851,

    /// <summary>
    /// Stairs leading down (index 852).
    /// </summary>
    Stairs_Down = 852,
}
