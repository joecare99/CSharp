using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Pascal.Models.Scanner;

#pragma warning disable IDE0130
namespace TranspilerLib.Pascal.Helper.Tests;

[TestClass]
public class SerializeTests
{
    [TestMethod]
    public void Test_SerializePasCodeBlock()
    {
        var block = new PasCodeBlock
        {
            Name = "MyFunction",
            Code = "function MyFunction(): Integer",
            Type = CodeBlockType.Function,
            SubBlocks =
            [
                new PasCodeBlock
                {
                    Name = "Body",
                    Code = "begin",
                    Type = CodeBlockType.Block,
                    //SubBlocks =
                    //[
                    //    new PasCodeBlock
                    //    {
                    //        Name = "ReturnStatement",
                    //        Code = ":=",
                    //        Type = CodeBlockType.Assignment,
                    //        SubBlocks =
                    //        [
                    //            new PasCodeBlock
                    //            {
                    //                Name = "ReturnStatement",
                    //                Code = "Result",
                    //                Type = CodeBlockType.Variable
                    //            },
                    //            new PasCodeBlock
                    //            {
                    //                Name = "Number",
                    //                Code = "42",
                    //                Type = CodeBlockType.Number
                    //            }
                    //        ]
                    //    }
                    //]
                }
            ]
        };
        var options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new ICodeBlockPasCodeBlockConverter()
            }
        };
        var json = System.Text.Json.JsonSerializer.Serialize<ICodeBlock>(block, options);
        Console.WriteLine(json);
        var deserializedBlock = System.Text.Json.JsonSerializer.Deserialize<ICodeBlock>(json, options);
        Assert.IsNotNull(deserializedBlock);
        Assert.AreEqual(block.Name, deserializedBlock.Name);
        Assert.AreEqual(block.Code, deserializedBlock.Code);
        Assert.AreEqual(block.Type, deserializedBlock.Type);
        Assert.HasCount(block.SubBlocks.Count, deserializedBlock.SubBlocks);
    }
}
