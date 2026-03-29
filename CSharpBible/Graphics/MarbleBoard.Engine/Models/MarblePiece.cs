using System;

namespace MarbleBoard.Engine.Models;

/// <summary>
/// Represents a single marble on the board.
/// </summary>
/// <param name="Id">The unique marble identifier.</param>
/// <param name="Color">The marble color used by the presentation layer.</param>
public sealed record MarblePiece(Guid Id, MarbleColor Color);
