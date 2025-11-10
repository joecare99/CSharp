// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using AlternatingAppearanceOfItems.Models.Interfaces;

namespace AlternatingAppearanceOfItems.Models;

public class League : ILeague
{
    public League(string name)
    {
        Name = name;
        Divisions = new List<IDivision>();
    }

    public string Name { get; }
    public List<IDivision> Divisions { get; }
}