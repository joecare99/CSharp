// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AlternatingAppearanceOfItems.Models.Interfaces;

namespace AlternatingAppearanceOfItems.Models;

public class Team : ITeam
{
    public Team(string name)
    {
        Name = name;
    }

    public string Name { get; }
}