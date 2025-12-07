using System.IO;
using TranspilerLib.Pascal.Models;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Service implementation for parsing LFM files using TranspilerLib.Pascal.
/// </summary>
public class LfmParserService : ILfmParserService
{
    private readonly LfmTokenizer _tokenizer;
    private readonly LfmObjectBuilder _builder;

    public LfmParserService()
    {
        _tokenizer = new LfmTokenizer();
        _builder = new LfmObjectBuilder();
    }

    /// <inheritdoc/>
    public LfmObject? Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        _tokenizer.SetInput(content);
        var tokens = _tokenizer.Tokenize();
        return _builder.Build(tokens);
    }

    /// <inheritdoc/>
    public LfmObject? LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        var content = File.ReadAllText(filePath);
        return Parse(content);
    }
}
