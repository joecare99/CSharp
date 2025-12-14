using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trnsp.Show.Pas.Services;
using TranspilerLib.Interfaces.Code;

namespace Trnsp.Show.Pas.Tests.Services
{
    [TestClass]
    public class PascalParserServiceTests
    {
        [TestMethod]
        public void PascalParserService_Implements_IPascalParserService()
        {
            var service = new PascalParserService();
            Assert.IsInstanceOfType(service, typeof(IPascalParserService));
        }

        [TestMethod]
        public void Parse_ReturnsCodeBlock()
        {
            // Arrange
            var service = new PascalParserService();
            string code = "program Test; begin end.";

            // Act
            var result = service.Parse(code);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ICodeBlock));
        }
    }
}
