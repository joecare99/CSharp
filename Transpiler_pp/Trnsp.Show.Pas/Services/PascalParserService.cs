using TranspilerLib.Interfaces.Code;

namespace Trnsp.Show.Pas.Services;

public class PascalParserService(ICodeBase parser) : IPascalParserService
{
    public ICodeBlock Parse(string code)
    {
        parser.OriginalCode = code;
        return parser.Parse();
    }
}
