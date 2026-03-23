using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JCAMS.Core.Tests
{
    [TestClass()]
    public class SSimpleCipherTests
    {
        [DataTestMethod()]
        [DataRow("Null", "", null)]
        [DataRow("empty", "", "")]
        [DataRow("Hello World", "Hello World", "9uWdWUpsF3YqFx3Br5HehA==")]
        [DataRow("Ratzopaltuff", "Ratzopaltuff", "um7lW5sPeEbPqyrSFrTQYg==")]
        [DataRow("Lorem ipsum ...", TestData.cLoremIpsum, "tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh52MZ+OV4PczZt69mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391ufLqgJrGeJCzw+M5JyYq4WXK4qcWp9PyGhwzr9waHUi3ZNQVj2otsnnDoJkGKxw1yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIsK+/TaUejzSHIV4l7xDxrL/Mt/5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkFZV1qaoQPtEgcVj6YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZIj8r8RxtlPZHWfcwMyzYCifMMFNT/BlLi0srvPAMMStVK37MqKDmvNmQurKmQVzaJinu7OOOk2ayePy+0uVzfoGdxgGA8ISlPJHYm3dxoMxRv3Ldhlthip7sDJWaYplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0UVPF7M0XqGxDcytpki9ZUOLGb2J/Ypi9Ly+ZX+LYs=")]
        public void SimpelDecryptTest(string name, string sExp, string sCypher)
        {
            Assert.AreEqual(sExp,SSimpleCipher.SimpleDecrypt(sCypher),$"Test: {name}");
        }

        [DataTestMethod()]
        [DataRow("Null",null,"")]
        [DataRow("empty", "", "")]
        [DataRow("Hello World", "Hello World", "9uWdWUpsF3YqFx3Br5HehA==")]
        [DataRow("Ratzopaltuff", "Ratzopaltuff", "um7lW5sPeEbPqyrSFrTQYg==")]

        [DataRow("Lorem ipsum ...", TestData.cLoremIpsum, "tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh52MZ+OV4PczZt69mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391ufLqgJrGeJCzw+M5JyYq4WXK4qcWp9PyGhwzr9waHUi3ZNQVj2otsnnDoJkGKxw1yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIsK+/TaUejzSHIV4l7xDxrL/Mt/5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkFZV1qaoQPtEgcVj6YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZIj8r8RxtlPZHWfcwMyzYCifMMFNT/BlLi0srvPAMMStVK37MqKDmvNmQurKmQVzaJinu7OOOk2ayePy+0uVzfoGdxgGA8ISlPJHYm3dxoMxRv3Ldhlthip7sDJWaYplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0UVPF7M0XqGxDcytpki9ZUOLGb2J/Ypi9Ly+ZX+LYs=")]
        public void SimpelEncryptTest(string name, string sMessage, string sExp)
        {
            Assert.AreEqual(sExp, SSimpleCipher.SimpleEncrypt(sMessage), $"Test: {name}");
        }
    }
}