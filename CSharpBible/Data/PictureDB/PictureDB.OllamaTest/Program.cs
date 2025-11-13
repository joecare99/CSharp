using System.Diagnostics;
using System.Text;

Console.WriteLine("PictureDB Ollama Test - simple CLI wrapper\n");

var model = args.Length > 0 ? args[0] : "mistral"; // beispiel
var prompt = args.Length > 1 ? string.Join(' ', args.Skip(1)) : "Guten Morgen";

// Candidate commands and argument patterns because ollama CLI versions differ
string[] candidateCommands = new[] { "run", "predict", "query" };
string[] argPatterns = new[]
{
    // common: positional prompt
    "{0} {1} \"{2}\"",
    // long flag
    "{0} {1} --prompt \"{2}\"",
    // short flag
    "{0} {1} p \"{2}\"",
    // some versions accept --text or --question
    "{0} {1} --text \"{2}\"",
};

string? finalOutput = null;
string? finalError = null;
string? usedInvocation = null;

foreach (var cmd in candidateCommands)
{
    foreach (var pattern in argPatterns)
    {
        var arguments = string.Format(pattern, cmd, model, prompt);
        try
        {
            Console.WriteLine($"Versuche: ollama {arguments}\n");

            var psi = new ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            using var proc = Process.Start(psi) ?? throw new InvalidOperationException("Could not start ollama process");

            var outputTask = proc.StandardOutput.ReadToEndAsync();
            var errorTask = proc.StandardError.ReadToEndAsync();

            var timeout = TimeSpan.FromSeconds(30);
            var waitTask = proc.WaitForExitAsync();
            if (await Task.WhenAny(waitTask, Task.Delay(timeout)) != waitTask)
            {
                try { proc.Kill(); } catch { }
                Console.WriteLine($"Timeout when running 'ollama {arguments}'.\n");
                continue;
            }

            string output = await outputTask;
            string error = await errorTask;

            // If CLI reports unsupported command/flag, try next pattern
            if (!string.IsNullOrEmpty(error))
            {
                var le = error.ToLowerInvariant();
                if (le.Contains("unknown command") || le.Contains("unknown flag") || le.Contains("flag provided but not defined") || le.Contains("unknown option"))
                {
                    Console.WriteLine($"Invocation 'ollama {arguments}' not supported by this version. Trying next...\n");
                    continue;
                }
            }

            // Accept either output or non-fatal error
            finalOutput = output;
            finalError = error;
            usedInvocation = arguments;
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Versuch mit 'ollama {arguments}': {ex.Message}\n");
            // try next pattern
        }
    }

    if (finalOutput != null || finalError != null) break;
}

if (finalOutput == null && finalError == null)
{
    Console.WriteLine("Alle Versuche fehlgeschlagen. Bitte prüfe, ob 'ollama' installiert und in PATH ist, und welche Subcommands bzw. Optionen verfügbar sind (z.B. 'ollama --help').");
    return;
}

Console.WriteLine($"--- Used invocation: ollama {usedInvocation} ---\n");
Console.WriteLine("--- Output ---");
if (!string.IsNullOrEmpty(finalOutput)) Console.WriteLine(finalOutput);

if (!string.IsNullOrEmpty(finalError))
{
    Console.WriteLine("--- Error ---");
    Console.WriteLine(finalError);
}
