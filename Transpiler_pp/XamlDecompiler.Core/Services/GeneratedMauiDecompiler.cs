using XamlDecompiler.Core.Models;

namespace XamlDecompiler.Core.Services;

/// <summary>
/// Decompiles generated MAUI source files into reconstructed XAML and code-behind text.
/// </summary>
public sealed class GeneratedMauiDecompiler
{
    private readonly GeneratedSourceParser _parser = new();
    private readonly XamlDocumentWriter _xamlWriter = new();
    private readonly CodeBehindWriter _codeBehindWriter = new();

    public DecompilationResult Decompile(string sourceText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceText);

        GeneratedSourceModel model = _parser.Parse(sourceText);

        return new DecompilationResult
        {
            ClassName = model.ClassName,
            Namespace = model.Namespace,
            RootTypeName = model.RootTypeName,
            XamlFilePath = model.XamlFilePath,
            XamlText = _xamlWriter.Write(model),
            CodeBehindText = _codeBehindWriter.Write(model),
            Diagnostics = model.Diagnostics
        };
    }
}
