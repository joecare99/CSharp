using Gen_FreeWin.Services;
using GenFree.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gen_FreeWin.Tests.Services
{
    [TestClass]
    public class NamenSuchDocumentComposerTests
    {
        [TestMethod]
        public void ComposeBerufeHeading_ForOccupation_Singular_ReturnsOccupationHeading()
        {
            var composer = new NamenSuchDocumentComposer();

            var heading = composer.ComposeBerufeHeading(
                EEventArt.eA_300,
                1,
                "Beruf",
                "Titel",
                "Text70",
                "Text444",
                "Text445");

            Assert.IsNotNull(heading);
            Assert.AreEqual("Beruf: ", heading.Text);
            Assert.AreEqual(1, heading.LeadingNewlineCount);
            Assert.IsFalse(heading.TrimDocumentEndFirst);
        }

        [TestMethod]
        public void ComposeBerufeHeading_ForFamilyResidence_Plural_ReturnsTrimAndResetHeading()
        {
            var composer = new NamenSuchDocumentComposer();

            var heading = composer.ComposeBerufeHeading(
                EEventArt.eA_602,
                2,
                "Beruf",
                "Titel",
                "Text70",
                "Text444",
                "Text445");

            Assert.IsNotNull(heading);
            Assert.AreEqual("Wohnungen der Familie: ", heading.Text);
            Assert.AreEqual(2, heading.LeadingNewlineCount);
            Assert.IsTrue(heading.TrimDocumentEndFirst);
            Assert.IsTrue(heading.ResetContextUbgT1);
        }

        [TestMethod]
        public void ComposeHeidatEvent_ForDivorceWithIndentedOutput_ReturnsDivorceAndTrauzeugenMetadata()
        {
            var composer = new NamenSuchDocumentComposer();

            var eventDefinition = composer.ComposeHeidatEvent(
                EEventArt.eA_504,
                true,
                "Scheidung");

            Assert.IsNotNull(eventDefinition);
            Assert.AreEqual("Scheidung", eventDefinition.PrefixText);
            Assert.IsTrue(eventDefinition.LeadingNewline);
            Assert.IsTrue(eventDefinition.EnsureIndent);
            Assert.AreEqual(20, eventDefinition.IndentValue);
            Assert.IsTrue(eventDefinition.IsDivorceEvent);
            Assert.AreEqual("Zeugen", eventDefinition.WitnessLabel);
        }

        [TestMethod]
        public void ComposeHeidatEvent_ForMarriage_ReturnsTrauzeugenLabel()
        {
            var composer = new NamenSuchDocumentComposer();

            var eventDefinition = composer.ComposeHeidatEvent(
                EEventArt.eA_Marriage,
                false,
                "Heirat");

            Assert.IsNotNull(eventDefinition);
            Assert.IsFalse(eventDefinition.IsDivorceEvent);
            Assert.AreEqual("Trauzeugen", eventDefinition.WitnessLabel);
            Assert.IsFalse(eventDefinition.LeadingNewline);
            Assert.IsFalse(eventDefinition.EnsureIndent);
        }
    }
}
