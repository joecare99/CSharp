using System.Diagnostics;
using System.Text;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class LLMClient : ILLMClient
{
    private readonly string _model;
    private readonly TimeSpan _timeout;

    public LLMClient(string model = "mistral", int timeoutSeconds = 60)
    {
        _model = model;
        _timeout = TimeSpan.FromSeconds(timeoutSeconds);
    }

    public async Task<string> AnalyzeImageAsync(string base64Image, string prompt)
    {
        // Construct prompt that includes the full base64 data as requested
        var sbPrompt = new StringBuilder();
        sbPrompt.AppendLine(prompt);
        sbPrompt.AppendLine();
        sbPrompt.AppendLine(base64Image ?? string.Empty);

        // Escape double quotes for command-line quoting
        var finalPrompt = sbPrompt.ToString().Replace("\"", "\\\"");

        var arguments = $"run {_model} \"{finalPrompt}\"";

        try
        {
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

            var waitTask = proc.WaitForExitAsync();
            if (await Task.WhenAny(waitTask, Task.Delay(_timeout)) != waitTask)
            {
                try { proc.Kill(); } catch { }
                throw new TimeoutException($"Timeout when running 'ollama {arguments}'");
            }

            var output = await outputTask;
            var error = await errorTask;

            if (!string.IsNullOrEmpty(error))
            {
                var le = error.ToLowerInvariant();
                if (le.Contains("unknown command") || le.Contains("unknown flag") || le.Contains("unknown option") || le.Contains("flag provided but not defined"))
                {
                    throw new InvalidOperationException($"ollama reported unsupported invocation: {error}");
                }
            }

            return string.IsNullOrEmpty(output) ? error ?? string.Empty : output;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to invoke ollama: {ex.Message}", ex);
        }
    }
}