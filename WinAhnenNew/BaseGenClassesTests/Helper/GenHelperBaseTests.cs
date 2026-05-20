using System;
using System.Collections.Generic;
using BaseGenClasses.Helper;

namespace BaseGenClasses.Helper.Tests;

[TestClass]
public class GenHelperBaseTests
{
    [TestMethod]
    [DataRow("I12", "0012")]
    [DataRow("F9", "0009")]
    [DataRow("AB123", "AB0123")]
    [DataRow("ABCD", "ABCD")]
    [DataRow("", "")]
    public void NormalCitRef_NormalizesLikePascal(string input, string expected)
    {
        var helper = new TestGenHelperBase();

        string result = helper.CallNormalCitRef(input);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Citation_Setter_ResetsCitationReferenceCache()
    {
        var helper = new TestGenHelperBase();
        helper.SetCitationRefn("cached");

        helper.Citation = new List<string> { "source" };

        Assert.AreEqual(string.Empty, helper.GetCitationRefn());
    }

    [TestMethod]
    public void FireEvent_DispatchesKnownParserEvent()
    {
        var helper = new TestGenHelperBase();

        helper.FireEvent(this, ["ParserIndiName", "Anna", "I42", "7"]);

        Assert.AreEqual("IndiName", helper.LastCall?.MethodName);
        Assert.AreEqual("Anna", helper.LastCall?.Text);
        Assert.AreEqual("I42", helper.LastCall?.Reference);
        Assert.AreEqual(7, helper.LastCall?.SubType);
    }

    [TestMethod]
    public void FireEvent_IgnoresInvalidPayload()
    {
        var helper = new TestGenHelperBase();

        helper.FireEvent(this, ["ParserIndiName", "Anna", "I42"]);
        helper.FireEvent(this, ["ParserIndiName", "Anna", "I42", "invalid"]);

        Assert.IsNull(helper.LastCall);
    }

    [TestMethod]
    public void Warning_RaisesConfiguredMessageCallback()
    {
        var helper = new TestGenHelperBase();
        MessageCall? call = null;
        helper.OnHlpMessage = (sender, type, text, reference, mode) => call = new MessageCall(sender, type, text, reference, mode);

        helper.CallWarning("warn", "I7", 2);

        Assert.IsNotNull(call);
        Assert.AreSame(helper, call.Sender);
        Assert.AreEqual(EventType.Warning, call.Type);
        Assert.AreEqual("warn", call.Text);
        Assert.AreEqual("I7", call.Reference);
        Assert.AreEqual(2, call.Mode);
    }

    [TestMethod]
    public void Error_RaisesConfiguredMessageCallback()
    {
        var helper = new TestGenHelperBase();
        MessageCall? call = null;
        helper.OnHlpMessage = (sender, type, text, reference, mode) => call = new MessageCall(sender, type, text, reference, mode);

        helper.CallError("err", "F3", 5);

        Assert.IsNotNull(call);
        Assert.AreSame(helper, call.Sender);
        Assert.AreEqual(EventType.Error, call.Type);
        Assert.AreEqual("err", call.Text);
        Assert.AreEqual("F3", call.Reference);
        Assert.AreEqual(5, call.Mode);
    }

    private sealed class TestGenHelperBase : GenHelperBase
    {
        public HelperCall? LastCall { get; private set; }

        public string CallNormalCitRef(string value) => NormalCitRef(value);

        public void CallWarning(string text, string reference, int mode) => Warning(text, reference, mode);

        public void CallError(string text, string reference, int mode) => Error(text, reference, mode);

        public string GetCitationRefn() => FCitRefn;

        public void SetCitationRefn(string value) => FCitRefn = value;

        public override void Clear() => Record(nameof(Clear), string.Empty, string.Empty, 0);

        public override void StartFamily(object sender, string text, string reference, int subType) => Record(nameof(StartFamily), text, reference, subType);

        public override void StartIndiv(object sender, string text, string reference, int subType) => Record(nameof(StartIndiv), text, reference, subType);

        public override void FamilyIndiv(object sender, string text, string reference, int subType) => Record(nameof(FamilyIndiv), text, reference, subType);

        public override void FamilyType(object sender, string text, string reference, int subType) => Record(nameof(FamilyType), text, reference, subType);

        public override void FamilyDate(object sender, string text, string reference, int subType) => Record(nameof(FamilyDate), text, reference, subType);

        public override void FamilyData(object sender, string text, string reference, int subType) => Record(nameof(FamilyData), text, reference, subType);

        public override void FamilyPlace(object sender, string text, string reference, int subType) => Record(nameof(FamilyPlace), text, reference, subType);

        public override void IndiData(object sender, string text, string reference, int subType) => Record(nameof(IndiData), text, reference, subType);

        public override void IndiDate(object sender, string text, string reference, int subType) => Record(nameof(IndiDate), text, reference, subType);

        public override void IndiName(object sender, string text, string reference, int subType) => Record(nameof(IndiName), text, reference, subType);

        public override void IndiPlace(object sender, string text, string reference, int subType) => Record(nameof(IndiPlace), text, reference, subType);

        public override void IndiRef(object sender, string text, string reference, int subType) => Record(nameof(IndiRef), text, reference, subType);

        public override void IndiOccu(object sender, string text, string reference, int subType) => Record(nameof(IndiOccu), text, reference, subType);

        public override void IndiRel(object sender, string text, string reference, int subType) => Record(nameof(IndiRel), text, reference, subType);

        public override void EndOfEntry(object sender, string text, string reference, int subType) => Record(nameof(EndOfEntry), text, reference, subType);

        public override void CreateNewHeader(string filename) => Record(nameof(CreateNewHeader), filename, string.Empty, 0);

        public override void SaveToFile(string filename) => Record(nameof(SaveToFile), filename, string.Empty, 0);

        private void Record(string methodName, string text, string reference, int subType)
        {
            LastCall = new HelperCall(methodName, text, reference, subType);
        }
    }

    private sealed record HelperCall(string MethodName, string Text, string Reference, int SubType);

    private sealed record MessageCall(object Sender, EventType Type, string Text, string Reference, int Mode);
}
