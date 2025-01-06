namespace TranspilerLib.Data.Tests
{
    [TestClass]
    public class TokenDataTests
    {
        [TestMethod]
        public void TokenDataTest()
        {
            var t = (TokenData)("If",CodeBlockType.Operation,2);
            Assert.AreEqual("If", t.Code);
            Assert.AreEqual(CodeBlockType.Operation, t.type);
            Assert.AreEqual(2, t.Level);
        }

    }
}
