using Microsoft.VisualStudio.TestTools.UnitTesting;
using TranspilerLib.Models.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Models.Scanner;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Data;

namespace TranspilerLib.Models.Interpreter.Tests;

[TestClass()]
public class IECInterpreterTests
{
    private static (ICodeBlock root, ICodeBlock pre) BuildLinearBlocks(params ICodeBlock[] blocks)
    {
        var root = new CodeBlock { Code = "ROOT", Type = CodeBlockType.Block };
        foreach (var b in blocks)
            b.Parent = root;
        return (root, blocks.First());
    }

    private static ICodeBlock Block(string code, CodeBlockType type) => new CodeBlock { Code = code, Type = type };

    [TestMethod()]
    public void Interpret_AssignmentChain_Works()
    {
        // Arrange: pre -> BEGIN -> a=b -> c=rw_ABS(b)
        var pre = Block("DECL", CodeBlockType.Declaration);
        var begin = Block("BEGIN", CodeBlockType.Label);
        var assign1 = Block("a = b", CodeBlockType.Assignment);
        var assign2 = Block("c = rw_ABS(b)", CodeBlockType.Assignment);
        var (root, preForExec) = BuildLinearBlocks(pre, begin, assign1, assign2);

        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object> { ["a"] = 0, ["b"] = -2 };

        // Act
        var result = sut.Interpret(preForExec, parameters);

        // Assert
        Assert.IsNull(result);
        Assert.AreEqual(parameters["b"], parameters["a"]);
        Assert.AreEqual(2, parameters["c"]);
    }

    [TestMethod()]
    public void Interpret_FunctionCall_WithMultipleArgs_Works()
    {
        // Arrange: pre -> BEGIN -> d=rw_CONCAT(a,b)
        var pre = Block("DECL", CodeBlockType.Declaration);
        var begin = Block("BEGIN", CodeBlockType.Label);
        var assign = Block("d = rw_CONCAT(a,b)", CodeBlockType.Assignment);
        var (root, preForExec) = BuildLinearBlocks(pre, begin, assign);

        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object> { ["a"] = "A", ["b"] = "B" };

        // Act
        sut.Interpret(preForExec, parameters);

        // Assert
        Assert.AreEqual("AB", parameters["d"]);
    }

    [TestMethod()]
    public void Interpret_Throws_WhenBeginMissing()
    {
        // Arrange: pre -> stmt (no BEGIN)
        var pre = Block("DECL", CodeBlockType.Declaration);
        var stmt = Block("a = b", CodeBlockType.Assignment);
        var (root, preForExec) = BuildLinearBlocks(pre, stmt);

        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object> { ["a"] = 0, ["b"] = 1 };

        // Act + Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => sut.Interpret(preForExec, parameters));
    }

    [TestMethod()]
    public void Interpret_Throws_OnUnsupportedBlockType()
    {
        // Arrange: pre -> BEGIN -> unsupported(Operation)
        var pre = Block("DECL", CodeBlockType.Declaration);
        var begin = Block("BEGIN", CodeBlockType.Label);
        var op = Block("a + b", CodeBlockType.Operation);
        var (root, preForExec) = BuildLinearBlocks(pre, begin, op);

        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object> { ["a"] = 1, ["b"] = 2 };

        // Act + Assert
        Assert.ThrowsExactly<NotImplementedException>(() => sut.Interpret(preForExec, parameters));
    }

    [TestMethod()]
    public void Interpret_Throws_OnInvalidAssignmentSyntax()
    {
        // Arrange: pre -> BEGIN -> invalid assignment
        var pre = Block("DECL", CodeBlockType.Declaration);
        var begin = Block("BEGIN", CodeBlockType.Label);
        var badAssign = Block("a = b = c", CodeBlockType.Assignment);
        var (root, preForExec) = BuildLinearBlocks(pre, begin, badAssign);

        var sut = new IECInterpreter(root);
        var parameters = new Dictionary<string, object> { ["a"] = 0, ["b"] = 1, ["c"] = 2 };

        // Act + Assert
        Assert.ThrowsExactly<FormatException>(() => sut.Interpret(preForExec, parameters));
    }

    [TestMethod()]
    [DataRow(IECResWords.rw_ABS, 1, true, 1)]
    [DataRow(IECResWords.rw_ABS, -1, true, 1)]
    [DataRow(IECResWords.rw_ABS, 0, true, 0)]
    [DataRow(IECResWords.rw_ABS, -1.1, true, 1.1)]
    [DataRow(IECResWords.rw_ACOS, 0.0, true, Math.PI/2)]
    [DataRow(IECResWords.rw_ACOS, 0.5, true, Math.PI/3)]
    [DataRow(IECResWords.rw_ASIN, 1e-9d, true, 0.0)]
    [DataRow(IECResWords.rw_ASIN, 0.5, true, Math.PI/6)]
    [DataRow(IECResWords.rw_ATAN, 1e-9d, true, 0.0)]
    [DataRow(IECResWords.rw_ATAN, 0.5, true,  0.463647609)]
 //   [DataRow(IECResWords.rw_ATAN2, new object[] { 0.0, 1.0 }, true, 1.1)]
    [DataRow(IECResWords.rw_CONCAT, new object[] { "a", "b" }, true, "ab")]
    [DataRow(IECResWords.rw_CONCAT, new object[] { "a", "b", "c" }, true, "abc")]
    [DataRow(IECResWords.rw_COS, 1e-9d, true, 1.0)]
    [DataRow(IECResWords.rw_COS, Math.PI, true, -1.0)]
    [DataRow(IECResWords.rw_DIV, new object[] { 5, 2 }, true, new object[] { 2, 1 })]
    [DataRow(IECResWords.rw_DIV, new object[] { 5, 3 }, true, new object[] { 1, 2 })]
    [DataRow(IECResWords.rw_EXP, 1e-9d, true, 1.0)]
    [DataRow(IECResWords.rw_EXP, 1d, true, Math.E)]
    [DataRow(IECResWords.rw_INT, 1.1, true, 1d)]
    [DataRow(IECResWords.rw_INT, -1.1, true, -2d)]
    [DataRow(IECResWords.rw_LEN, "abc", true, 3)]
    [DataRow(IECResWords.rw_LN, 1d, true, 0.0)]
    [DataRow(IECResWords.rw_LN, Math.E, true, 1.0)]
    [DataRow(IECResWords.rw_LOG, 1d, true, 0.0)]
    [DataRow(IECResWords.rw_LOG, 10d, true, 1.0)]
    [DataRow(IECResWords.rw_MOD, new object[] { 5, 2 }, true, 1)]
    [DataRow(IECResWords.rw_MOD, new object[] { 5, 3 }, true, 2)]
    [DataRow(IECResWords.rw_SIN, 1e-9d, true, 0.0)]
    [DataRow(IECResWords.rw_SIN, Math.PI, true, 0.0)]
    [DataRow(IECResWords.rw_SQRT, 1e-20d, true, 0.0)]
    [DataRow(IECResWords.rw_SQRT, 4d, true, 2.0)]
    [DataRow(IECResWords.rw_TO_STRING, 1, true, "1")]
    [DataRow(IECResWords.rw_TO_STRING, 1.1, true, "1,1")]
    [DataRow(IECResWords.rw_TAN, 1e-9d, true, 0.0)]
    [DataRow(IECResWords.rw_TAN, Math.PI, true, 0.0)]
    [DataRow(IECResWords.rw_TRUNC, 1.1d, true, 1.0)]
    [DataRow(IECResWords.rw_TRUNC, -1.1d, true, -1.0)]
    public void SystemfunctionsTest(Enum eAct, object value,bool xExp,object exp)
    {
        Assert.AreEqual(xExp, IECInterpreter.systemfunctions.TryGetValue(eAct,out var methods));
        if (value is object[] values)
        {
            var m = methods!.FirstOrDefault(m => (values.Count() == m?.GetParameters().Count()) && m.GetParameters().First().ParameterType.IsAssignableFrom(values[0].GetType()));
            if (m!.ReturnType.IsAssignableTo(typeof(double)))
                Assert.AreEqual((double)exp, (double)(m?.Invoke(null, values) ?? throw new InvalidOperationException("Method returned null")), 1e-7d);
            else if (m.ReturnType.IsAssignableTo(typeof((int, int))) && exp is object[] aexp)
                Assert.AreEqual((aexp[0], aexp[1]), ((int,int))(m?.Invoke(null, values) ?? throw new InvalidOperationException("Method returned null")));
            else
                Assert.AreEqual(exp, m?.Invoke(null, values));
        }
        else
        {
           // if (value is int i && exp is int ) value = (double)i;
            var m = methods!.First(m => m?.GetParameters().First().ParameterType.IsAssignableFrom(value.GetType())??false);
            if (m.ReturnType.IsAssignableTo(typeof(double)))
                Assert.AreEqual((double)exp, (double)(m?.Invoke(null, [value]) ?? throw new InvalidOperationException("Method returned null")), 1e-7);
            else
                Assert.AreEqual(exp, m?.Invoke(null, [value]));
        }
    }

}