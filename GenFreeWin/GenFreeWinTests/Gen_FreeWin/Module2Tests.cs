using System;
using GenFreeWin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenFreeWin.Tests
{
    [TestClass]
    public class Module2Tests
    {
        [TestMethod]
        public void Koelner_Phonetic_ReturnsExpectedCode_ForKnownInput()
        {
            Assert.AreEqual("657000", Module2.Koelner_Phonetic("Müller"));
        }

        [TestMethod]
        public void Koelner_Phonetic_NormalizesUmlautsAndSharpS()
        {
            Assert.AreEqual(
                Module2.Koelner_Phonetic("Straße"),
                Module2.Koelner_Phonetic("Strasse"));
        }

        [TestMethod]
        public void Koelner_Phonetic_ReturnsSixCharacterCode_ForEmptyInput()
        {
            Assert.AreEqual("000000", Module2.Koelner_Phonetic(string.Empty));
        }

        [TestMethod]
        public void Koelner_Phonetic_ThrowsForNullInput()
        {
            Assert.ThrowsExactly<NullReferenceException>(() => Module2.Koelner_Phonetic(null!));
        }
    }
}
