using System;
using AA15_Labyrinth.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Microsoft.VSDiagnostics;

namespace Benchmarks
{
    [CPUUsageDiagnoser]
    public class LabyrinthGeneratorBench
    {
        private ILabyrinthGenerator _gen = null !;
        [Params(20, 40, 60)]
        public int Size;
        [GlobalSetup]
        public void Setup()
        {
           
            _gen = new LabyrinthGenerator();
        }

        [Benchmark]
        public Labyrinth Generate_Maze()
        {
            // Quadratisches Labyrinth Size x Size
            return _gen.Generate(Size, Size, seed: 12345);
        }
    }
}