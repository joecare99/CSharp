using Microsoft.VisualStudio.TestTools.UnitTesting;
using TranspilerLib.Pascal.Interpreter;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
using TranspilerLib.Data;
using System;
using TranspilerLib.Pascal.Models.Scanner;
using System.Collections.Generic;

namespace TranspilerLib.Pascal.Tests.Models.Interpreter;

[TestClass]
public class PasInterpreterTests
{
    private static ICodeBlock DummyBlock(IList<ICodeBlock> s) => new CodeBlock { Code = "", Type = CodeBlockType.Block, SubBlocks = s };
    private static ICodeBlock DummyBlock() => new CodeBlock { Code = "", Type = CodeBlockType.Block };

    [TestMethod]
    public void Interpret_ReturnsTrue_ForSimpleBlock()
    {
        var sut = new PasInterpreter();
        var block = DummyBlock();

        var result = sut.Interpret(block);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Variables_DefaultEmpty_AndSettable()
    {
        var sut = new PasInterpreter();

        Assert.AreEqual(0, sut.Variables.Count);
        sut.Variables["x"] = 42;
        Assert.AreEqual(42, sut.Variables["x"]);
    }

    [TestMethod]
    public void Externals_DefaultEmpty_AndInvocable()
    {
        var sut = new PasInterpreter();
        Assert.AreEqual(0, sut.Externals.Count);

        bool called = false;
        sut.Externals["id"] = o => { called = true; return o; };

        var f = sut.Externals["id"];
        var result = f(5);

        Assert.IsTrue(called);
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void Prepare_ReturnsFalse_When_CodeBlock_IsNull()
    {
        var sut = new PasInterpreter();
        ICodeBlock? root = null;

        var result = sut.Prepare(root!);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Prepare_Registers_Variables_From_Declaration_Code()
    {
        var sut = new PasInterpreter();
        var root = DummyBlock();
        var decl = new PasCodeBlock
        {
            Type = CodeBlockType.Declaration,
            Code = ":",
            SubBlocks = [ 
              new PasCodeBlock
              {
                  Type = CodeBlockType.Variable,
                  Code = "x, y"
              },
              new PasCodeBlock
              {
                  Type = CodeBlockType.Variable,
                  Code = "Integer"
              }
            ]
        };
        root.SubBlocks.Add(decl);

        var ok = sut.Prepare(root);

        Assert.IsTrue(ok);
        Assert.IsTrue(sut.Variables.ContainsKey("x"));
        Assert.IsTrue(sut.Variables.ContainsKey("y"));
    }

    [TestMethod]
    public void Prepare_Registers_Variables_From_Name()
    {
        var sut = new PasInterpreter();
        var root = DummyBlock();
        var varNode = new CodeBlock
        {
            Type = CodeBlockType.Variable,
            Name = "z"
        };
        root.SubBlocks.Add(varNode);

        var ok = sut.Prepare(root);

        Assert.IsTrue(ok);
        Assert.IsTrue(sut.Variables.ContainsKey("z"));
    }

    [TestMethod]
    public void Prepare_Adds_External_Stub_For_Undeclared_Call()
    {
        var sut = new PasInterpreter();
        var root = DummyBlock();
        root.SubBlocks.Add(new CodeBlock
        {
            Type = CodeBlockType.Function,
            Code = "foo",
            SubBlocks = [new PasCodeBlock { Type=CodeBlockType.Number,Code = "1" }]
        });

        var ok = sut.Prepare(root);

        Assert.IsTrue(ok);
        Assert.IsTrue(sut.Externals.ContainsKey("foo"));
        Assert.ThrowsExactly<InvalidOperationException>(() => sut.Externals["foo"](null!));
    }

    [TestMethod]
    public void Prepare_Does_Not_Add_Stub_For_Declared_Function()
    {
        var sut = new PasInterpreter();
        var root = DummyBlock();

        var call = new CodeBlock { Type = CodeBlockType.Operation, Code = "foo();" };
        var func = new CodeBlock { Type = CodeBlockType.Function, Name = "foo" };

        root.SubBlocks.Add(call);
        root.SubBlocks.Add(func);

        var ok = sut.Prepare(root);

        Assert.IsTrue(ok);
        Assert.IsFalse(sut.Externals.ContainsKey("foo"));
    }

    [TestMethod]
    public void Prepare_Skips_Keywords_And_Strings_And_Tracks_Other_Calls()
    {
        var sut = new PasInterpreter();
        /* Struktur:
         * 1) Zuweisung: x := 'bar(baz)'
         * 2) if a then begin end
         * 3) noop();
         */
        var root = DummyBlock([
        // 1) String-Block: "'bar(baz)'"
        new PasCodeBlock{ Type = CodeBlockType.Assignment, Code = ":=", SubBlocks =[
            new PasCodeBlock{ Type = CodeBlockType.Variable, Code = "x" },
            new PasCodeBlock{ Type = CodeBlockType.String, Code = "'bar(baz)'" }] },

        // 2) if-Block mit Subblocks: a (Bedingung) und einem leeren begin-end Block
        new PasCodeBlock{ Type = CodeBlockType.Operation, Code = "if", SubBlocks =[
            new PasCodeBlock { Type = CodeBlockType.Variable, Code = "a" },
            new PasCodeBlock {Type = CodeBlockType.Block, Code = "begin" }] },

        // 3) letzter Block: Funktionsaufruf "noop"
        new PasCodeBlock { Type = CodeBlockType.Function, Code = "noop" }]);

        var ok = sut.Prepare(root);

        Assert.IsTrue(ok);
        Assert.IsTrue(sut.Externals.ContainsKey("noop"));
        Assert.IsFalse(sut.Externals.ContainsKey("if"));     // keyword gefiltert
        Assert.IsFalse(sut.Externals.ContainsKey("bar"));    // im String, ignoriert
    }

    [TestMethod]
    public void Prepare_Does_Not_Override_Existing_External()
    {
        var sut = new PasInterpreter();
        bool called = false;
        Func<object, object> existing = o => { called = true; return 123; };
        sut.Externals["foo"] = existing;

        var root = DummyBlock();
        root.SubBlocks.Add(new CodeBlock { Type = CodeBlockType.Operation, Code = "foo();" });

        var ok = sut.Prepare(root);

        Assert.IsTrue(ok);
        Assert.IsTrue(sut.Externals.ContainsKey("foo"));
        Assert.AreSame(existing, sut.Externals["foo"]);
        var result = sut.Externals["foo"](null!);
        Assert.IsTrue(called);
        Assert.AreEqual(123, result);
    }

    [TestMethod]
    public void Prepare_Ignores_Case_For_Function_Declarations()
    {
        var sut = new PasInterpreter();
        var root = DummyBlock();

        root.SubBlocks.Add(new CodeBlock { Type = CodeBlockType.Operation, Code = "foo();" });
        root.SubBlocks.Add(new CodeBlock { Type = CodeBlockType.Function, Name = "Foo" }); // different case

        var ok = sut.Prepare(root);

        Assert.IsTrue(ok);
        Assert.IsFalse(sut.Externals.ContainsKey("foo"));
    }
}
