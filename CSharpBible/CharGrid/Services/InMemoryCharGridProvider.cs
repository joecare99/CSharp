using System;

namespace CSharpBible.CharGrid.Services
{
    public class InMemoryCharGridProvider : ICharGridProvider
    {
        private readonly char[,] _grid;
        public int Rows { get; }
        public int Columns { get; }

        public InMemoryCharGridProvider(int rows = 16, int columns = 32, IRandomCharService random = null)
        {
            Rows = rows;
            Columns = columns;
            _grid = new char[rows, columns];
            var rnd = random ?? new RandomCharService();
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < columns; c++)
                    _grid[r, c] = rnd.NextChar();
        }

        public char GetChar(int row, int column) => _grid[row, column];
        public void SetChar(int row, int column, char value) => _grid[row, column] = value;
    }
}
