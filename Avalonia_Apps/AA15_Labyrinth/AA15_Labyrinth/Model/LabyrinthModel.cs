using System.Data;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;

namespace AA15_Labyrinth.Model;

public sealed class Labyrinth
{
    public required int Cols { get; init; }
    public required int Rows { get; init; }
    // parent tree: index -> parent index; root points to itself
    public required int[] Parent { get; init; }
}
