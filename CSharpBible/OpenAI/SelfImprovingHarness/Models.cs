namespace SelfImprovingHarness;

public sealed class OllamaOptions { 
    public string BaseUrl { get; set; } = "http://cachyos-x8664.fritz.box:11434";
    public string Model { get; set; } = "qwen3.8:27b"; 
    public int TimeoutSeconds { get; set; } = 1200; 
    public int MaxRetries { get; set; } = 2; 
}
public sealed class HarnessOptions { public int MaxGenerations { get; set; } = 3; public int MaxRepairAttempts { get; set; } = 2; public int BuildTimeoutSeconds { get; set; } = 120; public int BenchmarkIterations { get; set; } = 3; public double MinImprovement { get; set; } = .001; }
public sealed record BuildResult(bool Success, string Output, IReadOnlyList<string> Errors, TimeSpan Duration);
public sealed record FitnessResult(double Score, bool BuildPassed, bool SmokePassed, double? BenchmarkMs, string Details);
public sealed record GenerationResult(string Path, string Id);
public sealed record OllamaResponse(string Response, bool Done);
