using System;
// Todo: Create the ITokenizeData interface in the TranspilerLib.Interfaces.Code namespace to handle the tokenization of the code.
using TranspilerLib.Models;

namespace TranspilerLib.Interfaces.Code
{
    public interface ITokenHandler
    {
        string[] reservedWords { set; }

        bool TryGetValue(int state, out Action<ICodeBase.TokenDelegate?, string, TokenizeData> handler);
    }
}