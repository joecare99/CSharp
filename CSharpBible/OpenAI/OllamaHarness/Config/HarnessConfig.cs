// Config/HarnessConfig.cs
namespace OllamaHarness.Config;

public sealed record HarnessConfig
{
    public required string OllamaBaseUrl { get; init; } = "http://localhost:11434";
    public required string Model { get; init; } = "llama3.1:70b";
    public required string SourceDirectory { get; init; } = "./Sources";
    public required string StagingDirectory { get; init; } = "./Staging";
    public required string OutputDirectory { get; init; } = "./Output";
    public int MaxIterations { get; init; } = 10;
    public int MaxSourceTokens { get; init; } = 8192;
    public double Temperature { get; init; } = 0.2;
    public TimeSpan CompileTimeout { get; init; } = TimeSpan.FromSeconds(120);
    public TimeSpan ValidationTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public string[] OptimizationGoals { get; init; } =
    [
        "performance",
        "readability",
        "error-handling",
        "modularity"
    ];
}