using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trnsp.Show.Pas.Services;

namespace Trnsp.Show.Pas.Tests.Services
{
    [TestClass]
    public class FileServiceTests
    {
        [TestMethod]
        public void FileService_Implements_IFileService()
        {
            var service = new FileService();
            Assert.IsInstanceOfType(service, typeof(IFileService));
        }
    }
}
