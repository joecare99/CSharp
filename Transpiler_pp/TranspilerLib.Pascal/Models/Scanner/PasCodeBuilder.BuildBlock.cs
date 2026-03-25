using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Pascal.Data;

namespace TranspilerLib.Pascal.Models.Scanner;

public partial class PasCodeBuilder
{
    private void BuildBlock(TokenData tokenData, ICodeBuilderData data)
    {
        var erw = TryGetResWord(tokenData.Code);
        if (erw == EPasResWords.BEGIN)
        {
            if (IsDeclContext(data))
            {
                var decl = GetDecl(data);
                if (decl?.Parent != null)
                    data.actualBlock = decl.Parent;
            }

            var isMetaRoot = data.actualBlock.Type == CodeBlockType.MainBlock || data.actualBlock.Type == CodeBlockType.Function;
            var name = isMetaRoot ? "Body" : "Block";

            data.actualBlock = NewCodeBlock(name, CodeBlockType.Block, tokenData.Code.ToLower(), data.actualBlock, tokenData.Pos);
            ClearDeclState(data);
        }
        else if (erw == EPasResWords.END)
        {
            var block = data.actualBlock;
            while (block != null && block.Type != CodeBlockType.Block && block.Type != CodeBlockType.Function && block.Type != CodeBlockType.MainBlock)
            {
                block = block.Parent;
            }

            if (block != null && block.Parent != null)
                data.actualBlock = block.Parent;
        }
        else
        {
            AddRawToken(tokenData, data);
        }
    }
}
