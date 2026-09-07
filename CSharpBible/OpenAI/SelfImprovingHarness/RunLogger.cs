using System.Text.Json;
namespace SelfImprovingHarness;
public sealed class RunLogger
{
    private readonly string _file; private readonly SemaphoreSlim _gate = new(1, 1);
    public RunLogger(string root) { _file = Path.Combine(root, "run-log.jsonl"); }
    public async Task EventAsync(string type, object data, CancellationToken ct = default) { var line = JsonSerializer.Serialize(new { timestamp = DateTimeOffset.UtcNow, type, data }) + Environment.NewLine; await _gate.WaitAsync(ct); try { await File.AppendAllTextAsync(_file, line, ct); } finally { _gate.Release(); } }
}
