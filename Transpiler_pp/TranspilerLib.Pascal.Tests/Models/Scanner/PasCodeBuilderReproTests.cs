using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Pascal.Models.Scanner;

namespace TranspilerLib.Pascal.Models.Scanner.Tests
{
    [TestClass]
    public class PasCodeBuilderReproTests
    {
        [TestMethod]
        public void Parse_MultipleVarDeclarations_ShouldBeInDeclarationBlock()
        {
            var code = @"
var
  x: Integer;
  y: String;
begin
end.";
            var pasCode = new PasCode { OriginalCode = code };
            var root = pasCode.Parse();

            var declBlock = root.SubBlocks.FirstOrDefault(b => b.Type == CodeBlockType.Declaration && b.Code == "var");
            Assert.IsNotNull(declBlock, "Var declaration block not found");

            var colons = declBlock.SubBlocks.Where(b => b.Type == CodeBlockType.Operation && b.Code == ":").ToList();
            
            // Debug output
            System.Console.WriteLine($"DeclBlock SubBlocks: {declBlock.SubBlocks.Count}");
            foreach(var sub in declBlock.SubBlocks)
            {
                System.Console.WriteLine($" - {sub.Type} {sub.Code}");
            }
            System.Console.WriteLine($"Root SubBlocks: {root.SubBlocks.Count}");
            foreach(var sub in root.SubBlocks)
            {
                System.Console.WriteLine($" - {sub.Type} {sub.Code}");
            }

            Assert.HasCount(2, colons, "Should have 2 declarations in var block");
        }

        [TestMethod]
        public void Parse_ProgramName_ShouldBeSetInMainBlock()
        {
            var code = "Program MyProg; begin end.";
            var pasCode = new PasCode { OriginalCode = code };
            var root = pasCode.Parse();

            Assert.AreEqual("MyProg", root.Name);
            Assert.AreEqual("Program", root.Code);

            var dot = root.SubBlocks.LastOrDefault();
            Assert.IsNotNull(dot, "Last block is null");
            Assert.AreEqual(".", dot.Code);
            Assert.AreEqual(CodeBlockType.Operation, dot.Type);
        }

        [TestMethod]
        public void Parse_UnitName_ShouldBeSetInMainBlock()
        {
            var code = "Unit MyUnit; interface implementation end.";
            var pasCode = new PasCode { OriginalCode = code };
            var root = pasCode.Parse();

            Assert.AreEqual("MyUnit", root.Name);
            Assert.AreEqual("Unit", root.Code);
        }

        [TestMethod]
        public void Parse_MultipleAssignments_ShouldBeSiblingsInBlock()
        {
            var code = @"
program Test3;
begin
  a := 1;
  b := 2;
end.";
            var pasCode = new PasCode { OriginalCode = code };
            var root = pasCode.Parse();

            var body = root.SubBlocks.FirstOrDefault(b => b.Type == CodeBlockType.Block && b.Code == "begin");
            Assert.IsNotNull(body, "Body block not found");

            // Debug
            System.Console.WriteLine($"Body SubBlocks: {body.SubBlocks.Count}");
            foreach(var sub in body.SubBlocks)
            {
                System.Console.WriteLine($" - {sub.Type} {sub.Code}");
            }

            var assignments = body.SubBlocks.Where(b => b.Type == CodeBlockType.Assignment).ToList();
            Assert.HasCount(2, assignments, "Should have 2 assignments");
            Assert.AreEqual("a", assignments[0].SubBlocks[0].Code);
            Assert.AreEqual("b", assignments[1].SubBlocks[0].Code);
        }

        [TestMethod]
        public void Parse_Test2_Repro()
        {
            // var json = "[{\"Code\":\"var\",\"type\":\"operation\",\"Level\":0,\"Pos\":0},{\"Code\":\"x\",\"type\":\"variable\",\"Level\":0,\"Pos\":4},{\"Code\":\":\",\"type\":\"operation\",\"Level\":0,\"Pos\":5},{\"Code\":\"integer\",\"type\":\"variable\",\"Level\":0,\"Pos\":6},{\"Code\":\";\",\"type\":\"separator\",\"Level\":0,\"Pos\":13},{\"Code\":\"begin\",\"type\":\"block\",\"Level\":1,\"Pos\":16},{\"Code\":\"x\",\"type\":\"variable\",\"Level\":1,\"Pos\":27},{\"Code\":":=\",\"type\":\"assignment\",\"Level\":1,\"Pos\":28},{\"Code\":\"$B10F\",\"type\":\"number\",\"Level\":1,\"Pos\":32},{\"Code\":\";\",\"type\":\"separator\",\"Level\":1,\"Pos\":37},{\"Code\":\"end\",\"type\":\"block\",\"Level\":1,\"Pos\":40},{\"Code\":\".\",\"type\":\"operation\",\"Level\":0,\"Pos\":43}]";
            
            var tokens = new List<TokenData>
            {
                new TokenData { Code = "var", type = CodeBlockType.Operation },
                new TokenData { Code = "x", type = CodeBlockType.Variable },
                new TokenData { Code = ":", type = CodeBlockType.Operation },
                new TokenData { Code = "integer", type = CodeBlockType.Variable },
                new TokenData { Code = ";", type = CodeBlockType.Separator },
                new TokenData { Code = "begin", type = CodeBlockType.Block },
                new TokenData { Code = "x", type = CodeBlockType.Variable },
                new TokenData { Code = ":=", type = CodeBlockType.Assignment },
                new TokenData { Code = "$B10F", type = CodeBlockType.Number },
                new TokenData { Code = ";", type = CodeBlockType.Separator },
                new TokenData { Code = "end", type = CodeBlockType.Block },
                new TokenData { Code = ".", type = CodeBlockType.Operation }
            };

            var pasCode = new PasCode();
            var root = pasCode.Parse(tokens);

            Assert.AreEqual("PascalRoot", root.Name);
            Assert.AreEqual(CodeBlockType.MainBlock, root.Type);

            // Check Declaration
            var decl = root.SubBlocks.FirstOrDefault(b => b.Type == CodeBlockType.Declaration);
            Assert.IsNotNull(decl, "Declaration block missing");
            Assert.AreEqual("var", decl.Code);

            // Check Colon
            Assert.HasCount(1, decl.SubBlocks, "Declaration should have 1 child (Colon)");
            var colon = decl.SubBlocks[0];
            Assert.AreEqual(CodeBlockType.Operation, colon.Type);
            Assert.AreEqual(":", colon.Code);

            // Check x
            Assert.HasCount(2, colon.SubBlocks, "Colon should have 2 children (x, integer)");
            Assert.AreEqual("x", colon.SubBlocks[0].Code);
            Assert.AreEqual("integer", colon.SubBlocks[1].Code);
        }
    }
}
