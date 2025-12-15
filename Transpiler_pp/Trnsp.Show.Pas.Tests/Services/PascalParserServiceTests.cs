using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TranspilerLib.Interfaces.Code;
using Trnsp.Show.Pas.Services;

namespace Trnsp.Show.Pas.Tests.Services
{
    [TestClass]
    public class PascalParserServiceTests
    {
        [TestMethod]
        public void PascalParserService_Implements_IPascalParserService()
        {
            var service = new PascalParserService(Substitute.For<ICodeBase>());
            Assert.IsInstanceOfType(service, typeof(IPascalParserService));
        }

        [TestMethod]
        public void Parse_ReturnsCodeBlock()
        {
            // Arrange
            var parser = Substitute.For<ICodeBase>();
            var expectedBlock = Substitute.For<ICodeBlock>();
            parser.Parse().Returns(expectedBlock);
            var service = new PascalParserService(parser);
            string code = "program Test; begin end.";

            // Act
            var result = service.Parse(code);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedBlock, result);
            parser.Received().OriginalCode = code;
        }
    }
}
