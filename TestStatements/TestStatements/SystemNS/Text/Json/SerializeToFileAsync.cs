using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestStatements.SystemNS.Text.Json;

public class SerializeToFileAsync
{
    public static async Task Main()
    {
        var weatherForecast = new WeatherForecast
        {
            Date = DateTime.Parse("2019-08-01"),
            TemperatureCelsius = 25,
            Summary = "Hot"
        };
        string fileName = Path.Combine( Path.GetRandomFileName(), "WeatherForecast.json");
        Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
        string jsonString = JsonSerializer.Serialize(weatherForecast);
        File.WriteAllText(fileName, jsonString);
        Console.WriteLine(File.ReadAllText(fileName));
        Directory.Delete(Path.GetDirectoryName(fileName) ?? string.Empty, true);
    }
}
