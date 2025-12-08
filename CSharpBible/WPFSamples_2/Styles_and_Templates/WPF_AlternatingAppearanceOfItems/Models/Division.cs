// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using AlternatingAppearanceOfItems.Models.Interfaces;

namespace AlternatingAppearanceOfItems.Models;

public class Division : IDivision
{
    public Division(string name)
    {
        Name = name;
        Teams = new List<ITeam>();
    }

    public string Name { get; }
    public List<ITeam> Teams { get; }
}