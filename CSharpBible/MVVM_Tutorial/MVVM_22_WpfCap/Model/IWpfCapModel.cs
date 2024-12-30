using System;

namespace MVVM_22_WpfCap.Model
{
    public interface IWpfCapModel
    {
        /// <summary>Initializes the tiles of this instance.</summary>
        void Init();
        /// <summary>Shuffles the tiles of this instance.</summary>
        void Shuffle();

        /// <summary>Moves the tiles of the specified column up.</summary>
        /// <param name="column">The column.</param>
        void MoveUp(int column);
        /// <summary>Moves the tiles of the specified column down.</summary>
        /// <param name="column">The column.</param>
        void MoveDown(int column);
        /// <summary>Moves the tiles of the specified row to the left.</summary>
        /// <param name="row">The row.</param>
        void MoveLeft(int row);
        /// <summary>Moves the tiles of the specified row to the right.</summary>
        /// <param name="rpw">The RPW.</param>
        void MoveRight(int rpw);

        /// <summary>Returns the color of the Tile at the specified place.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        int TileColor(int x, int y);

        /// <summary>Occurs when the color of tiles were changed.</summary>
        event EventHandler TileColorChanged;
        /// <summary>Gets a value indicating whether the tiles of this instance are sorted.</summary>
        /// <value>
        ///   <c>true</c> if this instance is sorted; otherwise, <c>false</c>.</value>
        bool IsSorted { get; }

        /// <summary>Gets the width of the tile-field.</summary>
        /// <value>The width.</value>
        int Width { get; }
        /// <summary>Gets the height of the tile-field.</summary>
        /// <value>The height.</value>
        int Height { get; }
    }
}
