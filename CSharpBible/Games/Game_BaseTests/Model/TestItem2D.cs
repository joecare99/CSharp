using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Game.Model.Tests;

/// <summary>
/// Represents the 2D test item variant built on the shared placed-item base behavior.
/// </summary>
public class TestItem2D : TestPlacedItemBase<TestItem2D>
{
    /// <summary>
    /// Returns a string representation of the current test item.
    /// </summary>
    /// <returns>A formatted item description.</returns>
    public override string ToString() => $"TestItem2D({Name},{_place})";

}
