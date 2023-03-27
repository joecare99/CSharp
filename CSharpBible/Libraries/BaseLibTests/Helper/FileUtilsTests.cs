using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
	public class FileUtilsTests {
		internal string sUserData = "";
		private string sLocalTestPath = "";
        private Exception? eException;


		private void SaveTestFile(string sFilename, object aobj) {
			using (FileStream fs = File.OpenWrite(sFilename))
			using (StreamWriter sw = new StreamWriter(fs)) {
				sw.Write($"This is File: {sFilename}\r\nUserdata is {(aobj as FileUtilsTests).sUserData}");
			}
			if (eException != null) throw eException; 
		}

		public FileUtilsTests() {
		}

		[TestInitialize]
		public void Init() {
            sLocalTestPath = Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString());
			Directory.CreateDirectory(sLocalTestPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(sLocalTestPath,true);
        }

        [DataTestMethod()]
		[DataRow("", "", ".")]
		[DataRow("test.org", ".new", "test.new")]
		[DataRow("test", ".new", "test.new")]
		[DataRow("c:\\test.org\\test", ".new", "c:\\test.org\\test.new")]
		[DataRow("c:test.org", ".new", "c:test.new")]
		[DataRow("c:test", ".new", "c:test.new")]
		[DataRow("c:\\test.dir\\..\\test.org", ".new", "c:\\test.dir\\..\\test.new")]
		public void ChangeFileExtTest(string sFilename, string sNewExt, string sExpResult) {
			Assert.AreEqual(sExpResult, FileUtils.ChangeFileExt(sFilename, sNewExt));

		}

		[DataTestMethod()]
        [DataRow(0, "TestFile.any", false)]
        [DataRow(0, "TestFile.~new", false)]
        [DataRow(0, "TestFile.bak", false)]
        [DataRow(1, "TestFile.any", false)]
        [DataRow(1, "TestFile.~new", false)]
        [DataRow(1, "TestFile.bak", false)]
        [DataRow(2, "TestFile.any", false)]
        [DataRow(2, "TestFile.~new", false)]
        [DataRow(2, "TestFile.bak", true)]
        [DataRow(4, "TestFile.any", false)]
        [DataRow(4, "TestFile.~new", false)]
        [DataRow(4, "TestFile.bak", false)]
        public void SaveFileTest1(int iPreMode, string sFileName, bool _) {
			bool[] ePreMode = new bool[3];
			ePreMode[0] = iPreMode % 2 != 0;
			iPreMode /= 2;
			ePreMode[1] = iPreMode % 2 != 0;
			iPreMode /= 2;
			ePreMode[2] = iPreMode % 2 != 0;

			PrepareFiles(ePreMode, Path.Combine(sLocalTestPath,sFileName));
			sUserData = "Test 123";
            Assert.IsTrue(FileUtils.SaveFile(SaveTestFile, Path.Combine(sLocalTestPath, sFileName), this));
			Assert.IsTrue(File.Exists(Path.Combine(sLocalTestPath,sFileName)));

		}

        [DataTestMethod()]
        [DataRow(0, "TestFile.any", false)]
        [DataRow(0, "TestFile.~new", false)]
        [DataRow(0, "TestFile.bak", false)]
        [DataRow(1, "TestFile.any", false)]
        [DataRow(1, "TestFile.~new", false)]
        [DataRow(1, "TestFile.bak", false)]
        [DataRow(2, "TestFile.any", false)]
        [DataRow(2, "TestFile.~new", false)]
        [DataRow(2, "TestFile.bak", true)]
        [DataRow(4, "TestFile.any", false)]
        [DataRow(4, "TestFile.~new", false)]
        [DataRow(4, "TestFile.bak", false)]
        public void SaveFileTest2(int iPreMode, string sFileName, bool xExp)
        {
            bool[] ePreMode = new bool[3];
            ePreMode[0] = iPreMode % 2 != 0;
            iPreMode /= 2;
            ePreMode[1] = iPreMode % 2 != 0;
            iPreMode /= 2;
            ePreMode[2] = iPreMode % 2 != 0;

            PrepareFiles(ePreMode, Path.Combine(sLocalTestPath, sFileName));
            sUserData = "Test 123";
			eException = new FieldAccessException("Dummy Dummy");
            Assert.ThrowsException<FieldAccessException>(()=>FileUtils.SaveFile(SaveTestFile, Path.Combine(sLocalTestPath, sFileName), this));
            Assert.AreEqual(xExp, File.Exists(Path.Combine(sLocalTestPath, sFileName)));
        }

        private void PrepareFiles(bool[] ePreMode, string sFileName) {
			if (File.Exists(FileUtils.ChangeFileExt(sFileName, ".bak")))
				File.Delete(FileUtils.ChangeFileExt(sFileName, ".bak"));
			if (ePreMode[0]) {
				sUserData = "Backupfile";
				SaveTestFile(FileUtils.ChangeFileExt(sFileName, ".bak"), this);
			}
			if (File.Exists(sFileName))
				File.Delete(sFileName);
			if (ePreMode[1]) {
				sUserData = "Datafile";
				SaveTestFile(FileUtils.ChangeFileExt(sFileName, ".bak"), this);
			}
			if (File.Exists(FileUtils.ChangeFileExt(sFileName, ".~new")))
				File.Delete(FileUtils.ChangeFileExt(sFileName, ".~new"));
			if (ePreMode[2]) {
				sUserData = "NewFile";
				SaveTestFile(FileUtils.ChangeFileExt(sFileName, ".~new"), this);
			}
		}

		[DataTestMethod()]
		[DataRow("TestFile.dat", "")]
        [DataRow("TestFile2.dat", "12345678")]
        public void WriteStringToFileTest(string sFilename, string sData) {
			Assert.IsTrue(FileUtils.WriteStringToFile($"{sLocalTestPath}\\{sFilename}", sData));
			Assert.AreEqual(sData,File.ReadAllText(Path.Combine(sLocalTestPath, sFilename)));
		}

        [DataTestMethod()]
        [DataRow("TestFile.dat", "")]
        [DataRow("TestFile2.dat", "12345678")]
        public void ReadStringFromFileTest(string sFilename, string sData) {
			File.WriteAllText(Path.Combine(sLocalTestPath, sFilename), sData);
            Assert.AreEqual(sData, FileUtils.ReadStringFromFile(Path.Combine(sLocalTestPath, sFilename)));
        }
    }
}
