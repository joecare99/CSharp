using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.ClassesAndObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.ClassesAndObjects.Tests
{
    /// <summary>
    /// Defines test class MembersTests.
    /// </summary>
    [TestClass()]
    public class MembersTests
    {
        private readonly string csExpectedConstString = "This is a constant string!";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            Members.FieldCount = 0;
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        [TestMethod()]
        public void SetupTest()
        {
            Assert.AreEqual(0, Members.FieldCount);
            Assert.AreEqual(0, nChanges);
        }

        /// <summary>
        /// Defines the test method ConstantTest.
        /// </summary>
        [TestMethod()]
        public void ConstantTest()
        {
            Assert.AreEqual(csExpectedConstString, Members.ConstString);
        }

        /// <summary>
        /// Defines the test method VariableTest.
        /// </summary>
        [TestMethod()]
        public void VariableTest()
        {
            Assert.AreEqual(0, Members.FieldCount);
            Members.FieldCount = 1;
            Assert.AreEqual(1, Members.FieldCount);
            Members.FieldCount += 1;
            Assert.AreEqual(2, Members.FieldCount);
            Members.FieldCount = int.MaxValue;
            Assert.AreEqual(int.MaxValue, Members.FieldCount);
            Members.FieldCount = int.MinValue;
            Assert.AreEqual(int.MinValue, Members.FieldCount);
            if (0 > Members.FieldCount)
            {
                Members.FieldCount = -Members.FieldCount;
            }
            Assert.AreEqual(int.MinValue, Members.FieldCount);
            Members.FieldCount = Members.FieldCount % 27;
            Assert.AreEqual(-11, Members.FieldCount);
            if (Members.FieldCount < 0)
            {
                Assert.AreEqual(-11, Members.FieldCount);
            }
        }

        /// <summary>
        /// Defines the test method aMethodTest.
        /// </summary>
        [TestMethod()]
        public void aMethodTest()
        {
            Members.aMethod();
            Assert.AreEqual(3, Members.FieldCount);
        }

        /// <summary>
        /// Defines the test method PropertyTest.
        /// </summary>
        [TestMethod()]
        public void PropertyTest()
        {
            Assert.AreEqual(0, Members.aProperty);
            Members.aProperty += 3;
            Assert.AreEqual(3, Members.FieldCount);
            Assert.AreEqual(3, Members.aProperty);
            Members.aProperty = int.MinValue;
            Assert.AreEqual(int.MinValue, Members.FieldCount);
            Assert.AreEqual(int.MinValue, Members.aProperty);
            Members.aProperty = -Members.aProperty;
            Assert.AreEqual(int.MinValue, Members.FieldCount);
            Assert.AreEqual(int.MinValue, Members.aProperty);
        }

        /// <summary>
        /// Defines the test method IndexerTest.
        /// </summary>
        [TestMethod()]
        public void IndexerTest()
        {
            Members aMember = new Members();
            Assert.AreEqual(0, aMember[0]);
            Assert.AreEqual(5, aMember[5]);
            aMember[5] = 2;
            Assert.AreEqual(-3, aMember[0]);
            Assert.AreEqual(2, aMember[5]);
        }

        private int nChanges;
        private void MemberChange(object sender, EventArgs e)
        {
            nChanges++;
        }

        /// <summary>
        /// Defines the test method DelegateTest.
        /// </summary>
        [TestMethod()]
        public void DelegateTest()
        {
            Members.OnChange += new EventHandler(MemberChange);
            PropertyTest();
            Assert.AreEqual(2, nChanges);
        }
    }
}