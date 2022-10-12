﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace JCAMS.Core.Logging.Tests
{
    [TestClass()]
    public class TLoggingTests
    {
        string DebugResult = "";
        private readonly string cExpLogTest = "L:c:\\JCAMSstartup.txt,'Exception;TestException;;',(LogTest)\r\nL:c:\\JCAMSstartup.txt,'\n',()\r\n";
        private readonly string cExpLogTest0_1 = "L:c:\\JCAMSstartup.txt,'Exception;TestException;;',(LogTest0_1)\r\nL:c:\\JCAMSstartup.txt,'\n',()\r\n";
        private readonly string cExpLogTest0 = "";
        private readonly string cExpLogTest1 = "L:c:\\JCAMSstartup.txt,'LogTest1: {0}, {1}, {2}',(4321;1234;3rd Param)\r\nL:c:\\JCAMSstartup.txt,'\n',()\r\n";
        private readonly string cExpLogTest2 = "";
        private readonly string cExpLogTest2_1 = "";
        private readonly string cExpLogTest2_2 = "L:c:\\JCAMSstartup.txt,'LogTest2: {0}, {1}, {2}',(4321;1234;3rd Param)\r\nL:c:\\JCAMSstartup.txt,'\n',()\r\n";

        [TestInitialize()]
        public void Init()
        {
            TLogging.DoWriteLog = LogDoWrite;
            DebugResult = "";
        }

        private bool LogDoWrite(object oDest, string sText, object[] oParam)
        {
            string sParam = "";
            if (oParam != null)
                foreach (object arg4 in oParam.ToList())
                    sParam += $";{arg4}";
            DebugResult += $"L:{oDest},'{sText}',({sParam.TrimStart(';')})"+Environment.NewLine;
            return true; // todo:
        }

        [TestMethod()]
        public void CreateProtocolTest()
        {         
            Assert.Fail();
        }

        [TestMethod()]
        public void DebugPrintTest()
        {
            TLogging.DebugPrint("Hello World!");
        }

        [TestMethod()]
        public void DebugPrintTest1()
        {
            TLogging.DebugPrint("{0} {1}!","Ciao","Bella");
        }

        [TestMethod()]
        public void DeleteStartupLogTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LogTest()
        {
            TLogging.xTriggerError = false;
            TLogging.Log(new Exception("TestException"));
            Assert.AreEqual(cExpLogTest,DebugResult);
        }

        [TestMethod()]
        public void LogTest0_1()
        {
            TLogging.xTriggerError = true;
            TLogging.Log(new Exception("TestException"));
            Assert.AreEqual(cExpLogTest0_1, DebugResult);
        }

        [TestMethod()]
        public void LogTest1()
        {
            TLogging.xTriggerError = false;
            TLogging.Log("LogTest1: {0}, {1}, {2}",4321,1234,"3rd Param");
            Assert.AreEqual(cExpLogTest1, DebugResult);
        }

        [TestMethod()]
        public void LogTest2()
        {
            TLogging.xTriggerError = false;
            TLogging.Log(ELogTopic.Debug, "LogTest2: {0}, {1}, {2}", 4321, 1234, "3rd Param");
            Assert.AreEqual(cExpLogTest2, DebugResult);
        }

        [TestMethod()]
        public void LogTest2_1()
        {
            TLogging.xTriggerError = false;
            TLogging.xAppIsStarting = false;
            TLogging.Log(ELogTopic.Debug, "LogTest2: {0}, {1}, {2}", 4321, 1234, "3rd Param");
            Assert.AreEqual(cExpLogTest2_1, DebugResult);
        }

        [TestMethod()]
        public void LogTest2_2()
        {
            TLogging.xTriggerError = false;
            TLogging.xAppIsStarting = false;
            TLogging.Log(ELogTopic.Always, "LogTest2: {0}, {1}, {2}", 4321, 1234, "3rd Param");
            Assert.AreEqual(cExpLogTest2_2, DebugResult);
        }

        [TestMethod()]
        public void LogTest3()
        {
            Assert.Fail();
        }
    }
}