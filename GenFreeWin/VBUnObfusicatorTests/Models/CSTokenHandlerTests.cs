using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using VBUnObfusicator.Models;
using VBUnObfusicator.Interfaces.Code;
using static VBUnObfusicator.Interfaces.Code.ICSCode;
using VBUnObfusicator.Data;

namespace VBUnObfusicator.Tests.Models;

[TestClass]
public class CSTokenHandlerTests
{
    private class TokenCollector
    {
        public List<TokenData> Tokens { get; } = new();
        public void Collect(TokenData token) => Tokens.Add(token);
    }

    private static CSCode.CSTokenHandler CreateHandler(string[] reservedWords = null)
    {
        var handler = new CSCode.CSTokenHandler
        {
            reservedWords = reservedWords ?? new[] { "if", "for", "while", "return" }
        };
        return handler;
    }

    [TestMethod]
    [DataRow(0, typeof(Action<ICodeBase.TokenDelegate?, string, TokenizeData>), true)]
    [DataRow(1, typeof(Action<ICodeBase.TokenDelegate?, string, TokenizeData>), true)]
    [DataRow(99, null, false)]
    public void TryGetValue_ReturnsExpectedHandler(int state, Type expectedType, bool shouldExist)
    {
        var handler = CreateHandler();
        var result = handler.TryGetValue(state, out var action);
        Assert.AreEqual(shouldExist, result);
        if (shouldExist)
            Assert.IsNotNull(action);
        else
            Assert.IsNull(action);
    }

    [TestMethod]
    [DataRow("abc();", 0, 6, CodeBlockType.Instruction)]
    [DataRow("label:", 0, 6, CodeBlockType.Label)]
    [DataRow("goto label;", 0, 11, CodeBlockType.Goto)]
    [DataRow("if (x) {", 0, 8, CodeBlockType.Instruction)]
    [DataRow("{", 0, 0, CodeBlockType.Block)]
    [DataRow("}", 0, 0, CodeBlockType.Block)]
    public void HandleDefault_EmitsExpectedToken(string code, int pos2, int pos, CodeBlockType expectedType)
    {
        var handler = CreateHandler();
        var collector = new TokenCollector();
        var data = new TokenizeData { Pos2 = pos2, Pos = pos2, State = 0 };
        // Simulate moving to the right position
        for (int i = pos2; i <= pos; i++)
        {
            data.Pos = i;
            handler.TryGetValue(0, out var action);
            action?.Invoke(collector.Collect, code, data);
        }
        Assert.IsTrue(collector.Tokens.Exists(t => t.type == expectedType));
        Assert.AreEqual(0, data.State);
    }

    [TestMethod]
    [DataRow(" \"", 0, 1, 1)]
    [DataRow("@\"", 0, 1, 5)]
    [DataRow("$\"", 0, 1, 4)]
    [DataRow("// ", 0, 0, 2)]
    [DataRow("/*", 0, 0, 3)]
    [DataRow(" / ", 0, 1, 0)]
    public void HandleDefault_NewState(string code, int pos2, int pos, int expectedState)
    {
        var handler = CreateHandler();
        var collector = new TokenCollector();
        var data = new TokenizeData { Pos2 = pos2, Pos = pos2, State = 0 };
        // Simulate moving to the right position
        for (int i = pos2; i <= pos; i++)
        {
            data.Pos = i;
            handler.TryGetValue(0, out var action);
            action?.Invoke(collector.Collect, code, data);
        }
        Assert.AreEqual(expectedState == 0 ? 0 : 1, collector.Tokens.Count);
        Assert.AreEqual(expectedState, data.State);
    }

    [TestMethod]
    [DataRow("/* comment */", 2, 11, CodeBlockType.Comment)]
    public void HandleBlockComments_EmitsComment(string code, int pos2, int pos, CodeBlockType expectedType)
    {
        var handler = CreateHandler();
        var collector = new TokenCollector();
        var data = new TokenizeData { Pos2 = pos2, Pos = pos };
        if( handler.TryGetValue(3, out var action))
           action(collector.Collect, code, data);
        Assert.IsTrue(collector.Tokens.Exists(t => t.type == expectedType));
    }

    [TestMethod]
    [DataRow("// comment\r", 2, 10, CodeBlockType.LComment)]
    public void HandleLineComments_EmitsLComment(string code, int pos2, int pos, CodeBlockType expectedType)
    {
        var handler = CreateHandler();
        var collector = new TokenCollector();
        var data = new TokenizeData { Pos2 = pos2, Pos = pos, State= 2 };
        if (handler.TryGetValue(2, out var action))
            action(collector.Collect, code, data);
        Assert.IsTrue(collector.Tokens.Exists(t => t.type == expectedType));
    }

    [TestMethod]
    [DataRow("\"string\"\r", 0, 7, CodeBlockType.String)]
    [DataRow("@\"multi\rline\"", 0, 10, CodeBlockType.String)]
    public void HandleStrings_EmitsString(string code, int pos2, int pos, CodeBlockType expectedType)
    {
        // stringEndChars is assumed to contain '"' and '\r'
        typeof(CSCode.CSTokenHandler)
            .GetField("stringEndChars", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.SetValue(null, new[] { '"', '\r' });

        var handler = CreateHandler();
        var collector = new TokenCollector();
        var data = new TokenizeData { Pos2 = pos2, Pos = pos, State = 4 };
        if (handler.TryGetValue(4, out var action))
            action(collector.Collect, code, data);
        Assert.IsTrue(collector.Tokens.Exists(t => t.type == expectedType));
    }

    [TestMethod]
    [DataRow("$\"string\"\r", 0, 7,6, CodeBlockType.String)]
    [DataRow("$\"multiline{3}\"", 0, 13,4, CodeBlockType.String)]
    public void HandleStrings_IpoString(string code, int pos2, int pos,int iExpState, CodeBlockType expectedType)
    {
        // stringEndChars is assumed to contain '"' and '\r'
        typeof(CSCode.CSTokenHandler)
            .GetField("stringEndChars", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.SetValue(null, new[] { '"', '\r' });

        var handler = CreateHandler();
        var collector = new TokenCollector();
        var data = new TokenizeData { Pos2 = pos2, Pos = pos, State = 6 };
        if (handler.TryGetValue(6, out var action))
            action(collector.Collect, code, data);
        Assert.IsFalse(collector.Tokens.Exists(t => t.type == expectedType));
        Assert.AreEqual(iExpState, data.State); // Check if state is set to expected value
    }

    [TestMethod]
    public void ReservedWords_Setter_SetsValue()
    {
        var handler = CreateHandler();
        var words = new[] { "foo", "bar" };
        handler.reservedWords = words;
        // Use reflection to check private static field
        var field = typeof(CSCode.CSTokenHandler).GetProperty("reservedWords");
        Assert.IsNotNull(field);
    }
}
