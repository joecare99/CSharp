using TranspilerLib.Interfaces.Code;
using TranspilerLib.Pascal.Models.Scanner;

namespace Trnsp.Show.Pas.Services
{
    public class PascalParserService : IPascalParserService
    {
        public ICodeBlock Parse(string code)
        {
            var parser = new PasCode();
            parser.OriginalCode = code;
            return parser.Parse();
        }
    }
}
