using System;
using System.Text.Json;

namespace TestStatements.SystemNS.Text.Json;

public class SerializeBasic
{
    public static void Main()
    {
        var weatherForecast = new WeatherForecast
        {
            Date = DateTime.Parse("2019-08-01"),
            TemperatureCelsius = 25,
            Summary = "Hot"
        };

        string jsonString = JsonSerializer.Serialize(weatherForecast);

        Console.WriteLine(jsonString);
    }
}
