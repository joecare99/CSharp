using BaseLib.Models.Interfaces;

namespace AA15_Labyrinth.ViewModels.Tests;

public partial class LabyrinthViewModelTests
{
    private sealed class FakeRandom : IRandom
    {
        private int _next;
        public FakeRandom(int start = 1234) { _next = start; }

        public int Next(int v1, int v2 = -1) => (_next = (_next * 1103515245 + 12345) & int.MaxValue) % (v1);

        public double NextDouble()
        {
            throw new NotImplementedException();
        }

        public int NextInt() => _next = (_next * 1103515245 + 12345) & int.MaxValue;

        public void Seed(int value) => _next = value;
    }
}
