using MVVM.ViewModel;
using System;

namespace MVVM_TiledDisplay.ViewModel
{
    /// <summary>
    /// Class TileViewViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class TileViewViewModel : BaseViewModel
    {
        #region Properties
        private TileData[] _tiles=new TileData[0];
        /// <summary>
        /// Gets or sets the tiles.
        /// </summary>
        /// <value>The tiles.</value>
        public TileData[] Tiles { get => _tiles; set => SetProperty(ref _tiles, value); }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TileViewViewModel"/> class.
        /// </summary>
        public TileViewViewModel()
        {
            var _tiles = new TileData[20];
            for (int i = 0; i < 20; i++)
            {
                _tiles[i].Idx = i;
                _tiles[i].position =
                    _tiles[i].destination = new System.Drawing.Point((i % 5) * 3, (i / 5) * 3);
                _tiles[i].tileType = i % 9;
            }
            Tiles = _tiles;
//            Model.Model.PropertyChanged += ModelPropertyChanged;
        }

        /// <summary>
        /// Handles the Completed event of the Storyboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        internal void Storyboard_Completed(object sender, EventArgs e)
        {
            
        }

        #endregion
    }
}
