using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VBUnObfusicator.Models.ICSCode;

namespace VBUnObfusicator.Models.Tests
{
    [TestClass]
    public class TokenDataTests
    {
        [TestMethod]
        public void TokenDataTest()
        {
            var t = (TokenData)("If",CodeBlockType.Instruction,2);
            Assert.AreEqual("If", t.Code);
            Assert.AreEqual(CodeBlockType.Instruction, t.type);
            Assert.AreEqual(2, t.Level);
        }

    }
}
