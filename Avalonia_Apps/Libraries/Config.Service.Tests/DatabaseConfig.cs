namespace Config.Service.Tests;

public class DatabaseConfig
{
    public string? Server { get; set; }
    public int Port { get; set; } = 3306;
    public string? Database { get; set; } = "production";
}
