using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCAMS.Core.Extensions.Tests
{
    [TestClass()]
    public class AsBoolExtensionTests
    {
        [DataTestMethod()]
        [DataRow("Null",null,false)]
        //Todo:        [DataRow("DBNull.value", System.DBNull.Value, false)]
        [DataRow("false", false, false)]
        [DataRow("true", true, true)]
        [DataRow("'false'", "false", false)]
        [DataRow("'true'", "true", true)]
        [DataRow("'False'", "False", false)]
        [DataRow("'True'", "True", true)]
        [DataRow("'FALSE'", "FALSE", false)]
        [DataRow("'TRUE'", "TRUE", true)]
        [DataRow("'NEIN'", "NEIN", false)]
        [DataRow("'JA'", "JA", false)] // ??
        [DataRow("Entfalserine", "Entfalserine", false)] // ?
        [DataRow("Trantruete", "Trantruete", true)] //?
        [DataRow("0", 0, false)] 
        [DataRow("1", 1, true)]
        [DataRow("2", 2, false)]
        [DataRow("-1", -1, true)] // !!
        public void AsBoolTest(string name, object o,bool xExp)
        {
            Assert.AreEqual(xExp,AsBoolExtension.AsBool(o));
        }
    }
}