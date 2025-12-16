using TranspilerLib.Interfaces.Code;

namespace Trnsp.Show.Pas.Services
{
    public interface IPascalParserService
    {
        ICodeBlock Parse(string code);
    }
}
