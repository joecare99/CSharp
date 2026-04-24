using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using Trnsp.Show.Pas.ViewModels;

namespace Trnsp.Show.Pas.Tests.ViewModels
{
    [TestClass]
    public class CodeBlockNodeTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var codeBlock = Substitute.For<ICodeBlock>();
            codeBlock.Name.Returns("TestBlock");
            codeBlock.Type.Returns(CodeBlockType.Operation);
            codeBlock.Code.Returns("x := 1;");
            codeBlock.SubBlocks.Returns(new List<ICodeBlock>());

            // Act
            var node = new CodeBlockNode(codeBlock);

            // Assert
            Assert.AreEqual("TestBlock", node.Name);
            Assert.AreEqual(CodeBlockType.Operation, node.Type);
            Assert.AreEqual("x := 1;", node.Code);
#pragma warning disable MSTEST0037 // Use Assert.IsEmpty
            Assert.AreEqual(0, node.Children.Count);
#pragma warning restore MSTEST0037
        }

        [TestMethod]
        public void Constructor_CreatesChildrenRecursively()
        {
            // Arrange
            var childBlock = Substitute.For<ICodeBlock>();
            childBlock.Name.Returns("Child");
            childBlock.SubBlocks.Returns(new List<ICodeBlock>());

            var parentBlock = Substitute.For<ICodeBlock>();
            parentBlock.Name.Returns("Parent");
            parentBlock.SubBlocks.Returns(new List<ICodeBlock> { childBlock });

            // Act
            var node = new CodeBlockNode(parentBlock);

            // Assert
#pragma warning disable MSTEST0037 // Use Assert.HasCount
            Assert.AreEqual(1, node.Children.Count);
#pragma warning restore MSTEST0037
            Assert.AreEqual("Child", node.Children.First().Name);
        }
    }
}
