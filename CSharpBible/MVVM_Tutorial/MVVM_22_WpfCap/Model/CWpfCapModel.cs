using System;
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;

namespace MVVM_22_WpfCap.Model;

public class CWpfCapModel : IWpfCapModel
{
    #region Properties
    /// <summary>Gets a value indicating whether the tiles of this instance are sorted.</summary>
    /// <value>
    ///   <c>true</c> if this instance is sorted; otherwise, <c>false</c>.</value>
    public bool IsSorted => _IsSorted();
    
    public int Width => _width;

    public int Height => _height;

    /// <summary>Occurs when the color of tiles were changed.</summary>
    public event EventHandler? TileColorChanged;

    private int[] _tiles;
    private readonly int _width;
    private readonly int _height;
    private readonly IRandom _random;
    #endregion

    #region Methods
    public CWpfCapModel(IRandom random) {
        _width = 4;
        _height = 4;
        _random = random;
        _tiles = new int[_width * _height];
    }

    public void Init()
    {
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                SetTile(x, y, y + 1);
        TileColorChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Gets a value indicating whether the tiles of this instance are sorted.</summary>
    /// <value>
    ///   <c>true</c> if this instance is sorted; otherwise, <c>false</c>.</value>
    private bool _IsSorted()
    {
        bool result = true;
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                result &= TileColor(x, y) == y + 1;
        return result;
    }

    private void SetTile(int x, int y, int tile)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height) return;
        _tiles[y * _width + x] = tile;
    }

    public void MoveLeft(int row) => MoveLeft(row, true);
    public void MoveRight(int rpw) => MoveRight(rpw, true);
    public void MoveUp(int column) => MoveUp(column,true);
    public void MoveDown(int column) => MoveDown(column, true);

    public void Shuffle()
    {
        for (int i = 0; i < 100; i++)
        {
            var move = _random.Next(16);
            switch (move % 4)
            {
                case 0:
                    MoveRight(move / 4,false);
                    break;
                case 1:
                    MoveLeft(move / 4, false);
                    break;
                case 2:
                    MoveUp(move / 4, false);
                    break;
//                    case 3:
                default:
                    MoveDown(move / 4, false);
                    break;

            }
        }
        TileColorChanged?.Invoke(this, EventArgs.Empty);
    }

    public int TileColor(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height) return 0;
        else
        return _tiles[y * _width + x];
    }

    public void SetTiles(int[] tls)=> _tiles = tls;

    private void MoveLeft(int row, bool notify)
    {
        var s = TileColor(0, row);
        for (int i = 1; i < _width; i++)
            SetTile(i - 1, row, TileColor(i, row));
        SetTile(_width - 1, row, s);
        if (notify) TileColorChanged?.Invoke(this, EventArgs.Empty);
    }
    private void MoveRight(int row, bool notify)
    {
        var s = TileColor(_width - 1, row);
        for (int i = _width - 1; i > 0; i--)
            SetTile(i, row, TileColor(i - 1, row));
        SetTile(0, row, s);
        if (notify) TileColorChanged?.Invoke(this, EventArgs.Empty);
    }
    private void MoveUp(int column, bool notify)
    {
        var s = TileColor(column,0);
        for (int i = 1; i < _height; i++)
            SetTile(column,i-1, TileColor(column,i ));
        SetTile(column, _height - 1, s);

        if (notify) TileColorChanged?.Invoke(this, EventArgs.Empty);

    }
    private void MoveDown(int column, bool notify)
    {
        var s = TileColor( column, _height - 1);
        for (int i = _height - 1; i > 0; i--)
            SetTile(column, i, TileColor(column, i-1 ));
        SetTile(column, 0, s);

        if (notify) TileColorChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
