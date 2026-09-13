namespace Config.Service.Tests;

public class TestModel
{
    [SensitiveConfigProperty]
    public string? Password { get; set; }

    public string? Server { get; set; }
    public int Port { get; set; } = 3306;
}
