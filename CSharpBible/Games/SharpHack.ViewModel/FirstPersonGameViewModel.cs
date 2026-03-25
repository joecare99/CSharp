using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using SharpHack.Engine;

namespace SharpHack.ViewModel;

/// <summary>
/// ViewModel for the first-person view of the dungeon.
/// </summary>
public partial class FirstPersonGameViewModel : GameViewModel
{
    [ObservableProperty]
    private Direction _facingDirection = Direction.North;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirstPersonGameViewModel"/> class.
    /// </summary>
    /// <param name="session">The game session.</param>
    public FirstPersonGameViewModel(GameSession session) : base(session)
    {
    }

    /// <summary>
    /// Rotates the player left.
    /// </summary>
    [RelayCommand]
    public void RotateLeft()
    {
        FacingDirection = FacingDirection switch
        {
            Direction.North => Direction.West,
            Direction.West => Direction.South,
            Direction.South => Direction.East,
            Direction.East => Direction.North,
            _ => FacingDirection
        };
    }

    /// <summary>
    /// Rotates the player right.
    /// </summary>
    [RelayCommand]
    public void RotateRight()
    {
        FacingDirection = FacingDirection switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => FacingDirection
        };
    }

    /// <summary>
    /// Moves the player forward in the current facing direction.
    /// </summary>
    [RelayCommand]
    public void MoveForward()
    {
        Move(FacingDirection);
    }

    /// <summary>
    /// Gets the tile at the relative position from the player.
    /// </summary>
    /// <param name="forward">Distance forward.</param>
    /// <param name="right">Distance to the right (negative for left).</param>
    /// <returns>The tile at the relative position.</returns>
    public ITile? GetRelativeTile(int forward, int right)
    {
        var playerPos = Player.Position;
        int dx = 0, dy = 0;

        switch (FacingDirection)
        {
            case Direction.North:
                dx = right;
                dy = -forward;
                break;
            case Direction.South:
                dx = -right;
                dy = forward;
                break;
            case Direction.East:
                dx = forward;
                dy = right;
                break;
            case Direction.West:
                dx = -forward;
                dy = -right;
                break;
        }

        int targetX = playerPos.X + dx;
        int targetY = playerPos.Y + dy;

        if (Map.IsValid(targetX, targetY))
        {
            return Map[targetX, targetY];
        }

        return null;
    }
}
