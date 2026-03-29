namespace MarbleBoard.Wpf.ViewModels;

/// <summary>
/// Represents a single rendered board slot.
/// </summary>
/// <param name="Column">The zero-based column of the slot.</param>
/// <param name="Row">The zero-based row of the slot.</param>
public readonly record struct BoardSlotViewModel(int Column, int Row);
