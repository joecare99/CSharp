using System;
using System.Text.Json.Serialization;

namespace TestStatements.SystemNS.Text.Json;

public class WeatherForecastWithIgnoreAttribute
{
    public DateTimeOffset Date { get; set; }
    public int TemperatureCelsius { get; set; }
    [JsonIgnore]
    public string? Summary { get; set; }
}