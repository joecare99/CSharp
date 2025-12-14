using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using Trnsp.Show.Pas.Services;
using Trnsp.Show.Pas.ViewModels;
using TranspilerLib.Interfaces.Code;

namespace Trnsp.Show.Pas.Tests.ViewModels
{
    [TestClass]
    public class MainViewModelTests
    {
        private MainViewModel _testViewModel = null!;
        private IFileService _fileService = null!;
        private IPascalParserService _parserService = null!;

        [TestInitialize]
        public void Init()
        {
            _fileService = Substitute.For<IFileService>();
            _parserService = Substitute.For<IPascalParserService>();
            _testViewModel = new MainViewModel(_fileService, _parserService);
        }

        [TestMethod]
        public void SetupTest()
        {
            Assert.IsNotNull(_testViewModel);
            Assert.IsInstanceOfType(_testViewModel, typeof(MainViewModel));
            Assert.IsInstanceOfType(_testViewModel, typeof(ObservableObject));
        }

        [TestMethod]
        public void LoadFileCommand_ExecutesCorrectly()
        {
            // Arrange
            string filePath = "test.pas";
            string fileContent = "program Test; begin end.";
            var codeBlock = Substitute.For<ICodeBlock>();
            codeBlock.SubBlocks.Returns(new List<ICodeBlock>());

            _fileService.OpenFileDialog(Arg.Any<string>(), Arg.Any<string>()).Returns(filePath);
            _fileService.ReadAllText(filePath).Returns(fileContent);
            _parserService.Parse(fileContent).Returns(codeBlock);

            // Act
            _testViewModel.LoadFileCommand.Execute(null);

            // Assert
            _fileService.Received(1).OpenFileDialog(Arg.Any<string>(), Arg.Any<string>());
            _fileService.Received(1).ReadAllText(filePath);
            _parserService.Received(1).Parse(fileContent);
#pragma warning disable MSTEST0037 // Use Assert.HasCount
            Assert.AreEqual(1, _testViewModel.RootNodes.Count);
#pragma warning restore MSTEST0037
        }
    }
}
