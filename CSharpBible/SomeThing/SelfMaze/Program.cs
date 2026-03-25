using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
/* .................#
 * .##.###.##.##.##..
 * ..#.#........#..#.
 * ###.#.####.###.##.
 * .#........#......# 
 * .#.#.##.##.##.###.
 * ....#............E
 */
namespace SelfMaze
{
    /// <summary>
    /// Entry point: treats this source file as a character grid and finds the shortest
    /// path between two letters inside the file text (surprising "maze" defined by code).
    /// </summary>
    internal static class Program
    {
        private const string SourceFileName = "Program.cs";

        /// <summary>
        /// Main method: runs a simple self-test and then the "self-maze" demo.
        /// </summary>
        public static void Main(string[] args)
        {
            // Run a tiny built-in test first.
            RunTests();

            Console.WriteLine();
            Console.WriteLine("=== Self-Maze Demo ===");

            // Decide which two characters to connect.
            char startChar = 'S';
            char endChar = 'E';

            if (args.Length == 2 &&
                args[0].Length == 1 &&
                args[1].Length == 1)
            {
                startChar = args[0][0];
                endChar = args[1][0];
            }

            Console.WriteLine($"Looking for a path from '{startChar}' to '{endChar}' " +
                              $"inside {SourceFileName}.");

            var sourcePath = FindSourceFile();
            if (sourcePath == null)
            {
                Console.WriteLine("Could not locate source file; aborting demo.");
                return;
            }

            var grid = File.ReadAllLines(sourcePath, Encoding.UTF8);
            var maze = new SourceMaze(grid);

            var start = maze.FindFirst(startChar);
            var end = maze.FindFirst(endChar);

            if (start == null || end == null)
            {
                Console.WriteLine("Either start or end character not found in file.");
                return;
            }

            var path = maze.FindShortestPath(start.Value, end.Value);
            if (path == null)
            {
                Console.WriteLine("No path found.");
                return;
            }

            Console.WriteLine($"Path length: {path.Count - 1} steps");
            Console.WriteLine("Rendering file with path highlighted:");
            Console.WriteLine();

            maze.RenderWithPath(path);
        }

        /// <summary>
        /// Tries to guess where the current source file is.
        /// Works for usual "build from project" layouts.
        /// </summary>
        private static string? FindSourceFile()
        {
            // If running from bin/Debug or bin/Release, go up until we find SourceFileName.
            var baseDir = AppContext.BaseDirectory;
            var dir = new DirectoryInfo(baseDir);

            for (int i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, SourceFileName);
                if (File.Exists(candidate))
                    return candidate;
            }

            return null;
        }

        /// <summary>
        /// Very small "test harness" that validates BFS on a tiny in-memory example.
        /// </summary>
        private static void RunTests()
        {
            Console.WriteLine("Running built-in tests...");

            var tiny = new[]
            {
                "S..",
                ".#.",
                "..E"
            };
            var maze = new SourceMaze(tiny);
            var s = maze.FindFirst('S') ?? throw new InvalidOperationException("S missing");
            var e = maze.FindFirst('E') ?? throw new InvalidOperationException("E missing");
            var path = maze.FindShortestPath(s, e);

            if (path == null || path.Count == 0)
            {
                Console.WriteLine("Test FAILED: no path found in tiny maze.");
            }
            else
            {
                // In this layout the shortest path is 4 steps (5 cells).
                if (path.Count != 5)
                {
                    Console.WriteLine("Test FAILED: unexpected path length: " + path.Count);
                }
                else
                {
                    Console.WriteLine("Test OK.");
                }
            }
        }
    }
}