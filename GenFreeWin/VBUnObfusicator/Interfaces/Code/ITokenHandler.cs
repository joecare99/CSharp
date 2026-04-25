using System;
// Todo: Create ITokenizeData interface to replace TokenizeData and remove this using statement 
using VBUnObfusicator.Models;

namespace VBUnObfusicator.Interfaces.Code
{
    public interface ITokenHandler
    {
        string[] reservedWords { set; }

        bool TryGetValue(int state, out Action<ICodeBase.TokenDelegate?, string, TokenizeData> handler);
    }
}