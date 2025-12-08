using GenFree.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace gen_plusTests.Gen_FreeWin
{
    [TestClass]
    public class DataModuleBaseTests : TestBase
    {
        private string _workingDir = "";

        [TestInitialize]
        public void Init()
        {
            // get a temporary working directory
             _workingDir = Path.Combine( Path.GetTempPath(),Guid.NewGuid().ToString());
            Directory.CreateDirectory(_workingDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // delete the temporary working directory
            Directory.Delete(_workingDir, true);
        }
        [TestMethod]
        public void TestSetUp()
        {
            Assert.IsTrue(Directory.Exists(_workingDir));
        }
        [TestMethod]
        public void CreateMandantTest()
        {
            DataModul.DataOpen(Path.Combine(_workingDir, "test.mdb"));
        }
    }
}
