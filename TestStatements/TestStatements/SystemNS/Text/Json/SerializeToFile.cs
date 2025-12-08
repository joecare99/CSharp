// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 03-22-2025
//
// Last Modified By : Mir
// Last Modified On : 03-22-2025
// ***********************************************************************
// <copyright file="SerializeToFile.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.IO;
using System.Text.Json;

/// <summary>
/// The Json namespace.
/// </summary>
namespace TestStatements.SystemNS.Text.Json;

/// <summary>
/// Class SerializeToFile.
/// </summary>
public class SerializeToFile
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    public static void Main()
    {
        var weatherForecast = new WeatherForecast
        {
            Date = DateTime.Parse("2019-08-01"),
            TemperatureCelsius = 25,
            Summary = "Hot"
        };

        string fileName = "WeatherForecast.json";
        string jsonString = JsonSerializer.Serialize(weatherForecast);
        File.WriteAllText(fileName, jsonString);

        Console.WriteLine(File.ReadAllText(fileName));
    }
}
