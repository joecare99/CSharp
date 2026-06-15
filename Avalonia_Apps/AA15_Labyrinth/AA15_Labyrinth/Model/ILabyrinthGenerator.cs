using System;

namespace AA15_Labyrinth.Model;

public interface ILabyrinthGenerator
{
    // progress: reports values in [0,1]
    Labyrinth Generate(int cols, int rows, int seed, IProgress<double>? progress = null);
}
