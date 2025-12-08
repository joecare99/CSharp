// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AlternatingAppearanceOfItems.Models.Interfaces;

namespace AlternatingAppearanceOfItems.Models;

public class Place : IPlace
{
    public Place()
    {
        CityName = string.Empty;
        State = string.Empty;
    }

    public Place(string name, string state)
    {
        CityName = name;
        State = state;
    }

    public string CityName { get; set; }
    public string State { get; set; }
}