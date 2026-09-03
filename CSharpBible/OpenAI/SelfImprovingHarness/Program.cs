using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.CommandLine;
using SelfImprovingHarness;
var root = AppContext.BaseDirectory; while (!File.Exists(Path.Combine(root, "SelfImprovingHarness.csproj")) && Directory.GetParent(root) is { } p) root = p.FullName;
var config = new ConfigurationBuilder().SetBasePath(root).AddJsonFile("appsettings.json", optional: true).AddCommandLine(args).Build();
// Short CLI aliases are mapped explicitly to the nested option sections.
var cli = new Dictionary<string, string?>();
foreach (var arg in args.Where(a => a.StartsWith("--") && a.Contains('='))) { var parts = arg[2..].Split('=', 2); cli[parts[0].ToLowerInvariant()] = parts[1]; }
var merged = new ConfigurationBuilder().AddConfiguration(config)
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Ollama:Model"] = cli.GetValueOrDefault("model"),
        ["Ollama:BaseUrl"] = cli.GetValueOrDefault("ollama-url"),
        ["Harness:MaxGenerations"] = cli.GetValueOrDefault("generations")
    }).Build();
var services = new ServiceCollection();
services.AddOptions<OllamaOptions>().Bind(merged.GetSection("Ollama"));
services.AddOptions<HarnessOptions>().Bind(merged.GetSection("Harness"));
services.AddLogging(b => b.AddSimpleConsole(o => o.SingleLine = true).SetMinimumLevel(LogLevel.Information));
services.AddHttpClient<IOllamaClient, OllamaClient>((sp, client) =>
{
    var o = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    client.BaseAddress = new Uri(o.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(o.TimeoutSeconds);
});
services.AddSingleton(new RunLogger(root));
services.AddSingleton<ICompilerService, CompilerService>();
services.AddSingleton<ISelfModifier, SelfModifier>();
services.AddSingleton<IFitnessEvaluator, FitnessEvaluator>();
services.AddSingleton<Orchestrator>();
using var provider = services.BuildServiceProvider();
if (args.Contains("--smoke-test"))
{
    Console.WriteLine("SMOKE_OK"); return;
}
if (args.Contains("--help"))
{
    Console.WriteLine("SelfImprovingHarness --model=llama3.1 --generations=3 --ollama-url=http://localhost:11434"); return;
}
await provider.GetRequiredService<Orchestrator>().RunAsync(root);
