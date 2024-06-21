using System.Drawing;

namespace MVVM_TiledDisplay.ViewModel
{
    /// <summary>
    /// Struct TileData
    /// </summary>
    public struct TileData
    {
        /// <summary>
        /// The index
        /// </summary>
        public int Idx;
        /// <summary>
        /// The position
        /// </summary>
        public PointF position;
        /// <summary>
        /// The destination
        /// </summary>
        public Point destination;
        /// <summary>
        /// The tile type
        /// </summary>
        public int tileType;
    }
}
