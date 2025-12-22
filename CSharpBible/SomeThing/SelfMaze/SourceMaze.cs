namespace SelfMaze
{
    /// <summary>
    /// Represents a "maze" defined by lines of text. Any character except '#'
    /// is walkable, and BFS is used to find the shortest path between cells.
    /// </summary>
    internal sealed class SourceMaze
    {
        private readonly string[] _lines;
        private readonly int _height;
        private readonly int _width;

        /// <summary>
        /// Creates a new maze from the given lines of text.
        /// </summary>
        public SourceMaze(string[] lines)
        {
            _lines = lines;
            _height = lines.Length;
            _width = lines.Length > 0 ? lines.Max(l => l.Length) : 0;
        }

        /// <summary>
        /// Simple 2D integer coordinate.
        /// </summary>
        public readonly record struct Cell(int X, int Y);

        /// <summary>
        /// Finds the first occurrence of the given character in the grid, line by line.
        /// Returns null if not found.
        /// </summary>
        public Cell? FindFirst(char ch)
        {
            for (int y = 0; y < _height; y++)
            {
                var line = _lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == ch)
                        return new Cell(x, y);
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the shortest path between two cells using BFS.
        /// Cells with '#' are considered blocked.
        /// </summary>
        public List<Cell>? FindShortestPath(Cell start, Cell end)
        {
            var queue = new Queue<Cell>();
            var visited = new bool[_width, _height];
            var prev = new Dictionary<Cell, Cell>();

            queue.Enqueue(start);
            visited[start.X, start.Y] = true;

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.Equals(end))
                    return ReconstructPath(start, end, prev);

                for (int dir = 0; dir < 4; dir++)
                {
                    int nx = current.X + dx[dir];
                    int ny = current.Y + dy[dir];

                    if (!IsWalkable(nx, ny, visited))
                        continue;

                    var next = new Cell(nx, ny);
                    visited[nx, ny] = true;
                    prev[next] = current;
                    queue.Enqueue(next);
                }
            }

            return null;
        }

        /// <summary>
        /// Writes the original text to the console, drawing path cells in inverted
        /// colors if the terminal supports it (otherwise just prints normally).
        /// </summary>
        public void RenderWithPath(IReadOnlyCollection<Cell> path)
        {
            var pathSet = new HashSet<Cell>(path);

            var originalFg = Console.ForegroundColor;
            var originalBg = Console.BackgroundColor;

            for (int y = 0; y < _height; y++)
            {
                var line = _lines[y].PadRight(_width);
                for (int x = 0; x < _width; x++)
                {
                    var cell = new Cell(x, y);
                    if (pathSet.Contains(cell))
                    {
                        // Invert colors for path cells.
                        Console.BackgroundColor = originalFg == ConsoleColor.Black
                            ? ConsoleColor.Gray
                            : ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = originalFg;
                        Console.BackgroundColor = originalBg;
                    }

                    Console.Write(line[x]);
                }

                Console.ForegroundColor = originalFg;
                Console.BackgroundColor = originalBg;
                Console.WriteLine();
            }

            Console.ForegroundColor = originalFg;
            Console.BackgroundColor = originalBg;
        }

        private bool IsWalkable(int x, int y, bool[,] visited)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return false;

            if (visited[x, y])
                return false;

            var line = _lines[y];
            if (x >= line.Length)
                return false;

            var ch = line[x];
            return ch != '#';
        }

        private static List<Cell> ReconstructPath(
            Cell start,
            Cell end,
            Dictionary<Cell, Cell> prev)
        {
            var path = new List<Cell>();
            var current = end;
            path.Add(current);

            // Walk backwards until we reach start.
            while (!current.Equals(start))
            {
                if (!prev.TryGetValue(current, out var p))
                {
                    // No path; should not happen if BFS was correct.
                    break;
                }

                current = p;
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }
}