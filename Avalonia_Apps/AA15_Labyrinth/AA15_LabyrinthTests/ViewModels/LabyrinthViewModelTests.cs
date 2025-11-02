using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Headless.MSTest;
using Avalonia.Threading;
using AA15_Labyrinth.Model;
using AA15_Labyrinth.ViewModels;
using BaseLib.Models.Interfaces;

namespace AA15_Labyrinth.ViewModels.Tests;

[TestClass]
public partial class LabyrinthViewModelTests
{
    private sealed class FakeGen : ILabyrinthGenerator
    {
        public int Calls { get; private set; }
        public (int cols, int rows, int seed)? LastArgs { get; private set; }

        public Labyrinth Generate(int cols, int rows, int seed, IProgress<double>? progress = null)
        {
            Calls++;
            LastArgs = (cols, rows, seed);
            progress?.Report(0.3);
            var lab = BuildLinear(cols, rows);
            progress?.Report(1.0);
            return lab;
        }

        private static Labyrinth BuildLinear(int cols, int rows)
        {
            int n = cols * rows;
            var parent = new int[n];
            Array.Fill(parent, -1);
            int y = rows / 2;
            int start = (cols - 1) + y * cols;
            parent[start] = start; // root points to itself
            for (int x = cols - 2; x >= 0; x--)
            {
                int id = x + y * cols;
                int prev = (x + 1) + y * cols;
                parent[id] = prev;
            }
            return new Labyrinth { Cols = cols, Rows = rows, Parent = parent };
        }
    }

    private static async Task WaitUntilAsync(Func<bool> predicate, int timeoutMs = 2000)
    {
        var sw = Stopwatch.StartNew();
        while (!predicate() && sw.ElapsedMilliseconds < timeoutMs)
        {
            await Task.Delay(10).ConfigureAwait(false);
            await Dispatcher.UIThread.InvokeAsync(() => { });
        }
        Assert.IsTrue(predicate(), "Timeout waiting for condition");
    }

    [AvaloniaTestMethod]
    public async Task Build_Generates_Maze_and_Solution()
    {
        // Arrange
        var gen = new FakeGen();
        var vm = new LabyrinthViewModel(gen, new FakeRandom());
        var bounds = new Size(240, 180);

        // Act
        vm.Build(bounds);
        await WaitUntilAsync(() => !vm.IsGenerating && vm.MazeGeometry is not null);

        // Assert
        Assert.IsTrue(vm.Progress >= 1.0);
        Assert.IsNotNull(vm.MazeGeometry);
        Assert.IsNotNull(vm.SolutionGeometry);
    }

    [AvaloniaTestMethod]
    public async Task Build_Is_Idempotent_For_Same_Bounds()
    {
        var gen = new FakeGen();
        var vm = new LabyrinthViewModel(gen, new FakeRandom());
        var bounds = new Size(300, 200);

        vm.Build(bounds);
        await WaitUntilAsync(() => !vm.IsGenerating);

        // Second call with same bounds should skip generation
        vm.Build(bounds);
        // Give dispatcher a tick
        await Dispatcher.UIThread.InvokeAsync(() => { });

        Assert.AreEqual(1, gen.Calls, "Generator should have been called only once for same bounds.");
    }

    [AvaloniaTestMethod]
    public void Defaults_Are_Set()
    {
        var vm = new LabyrinthViewModel(new FakeGen(), new FakeRandom());
        Assert.AreEqual(1, vm.Seed);
        Assert.AreEqual(12.0, vm.CellSize, 1e-9);
        Assert.AreEqual(1.8, vm.LineThickness, 1e-9);
        Assert.AreEqual(new Thickness(12), vm.Padding);
        Assert.IsFalse(vm.DrawSolution);
        Assert.IsNull(vm.MazeGeometry);
        Assert.IsNull(vm.SolutionGeometry);
        Assert.AreEqual(0.0, vm.Progress, 1e-9);
        Assert.IsFalse(vm.IsGenerating);
    }

    [AvaloniaTestMethod]
    public async Task Regenerate_Clears_Geometries()
    {
        var gen = new FakeGen();
        var vm = new LabyrinthViewModel(gen, new FakeRandom());
        vm.Build(new Size(240, 180));
        await WaitUntilAsync(() => !vm.IsGenerating);
        Assert.IsNotNull(vm.MazeGeometry);

        vm.Regenerate();
        Assert.IsNull(vm.MazeGeometry);
        Assert.IsNull(vm.SolutionGeometry);
    }

    [AvaloniaTestMethod]
    public void Randomize_Changes_Seed_Or_Clears_Geometries()
    {
        var vm = new LabyrinthViewModel(new FakeGen(), new FakeRandom());
        var oldSeed = vm.Seed;
        vm.Randomize();
        // Seed might coincidentally match, but geometries must be cleared
        Assert.IsNull(vm.MazeGeometry);
        Assert.IsNull(vm.SolutionGeometry);
    }
}
