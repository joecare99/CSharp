namespace Config.Service.Tests;

public enum TestMode
{
    Manual,
    Automatic
}

public class TestConfig
{
    public string? Server { get; set; }
    public int Port { get; set; } = 5432;
    public string? Database { get; set; } = "testdb";
    public TestMode Mode { get; set; } = TestMode.Automatic;
}
