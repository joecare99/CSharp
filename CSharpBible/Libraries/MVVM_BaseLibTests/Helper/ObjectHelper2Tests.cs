using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Helper.Tests
{
    [TestClass()]
    public class ObjectHelper2Tests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private ControlArray<int?> _testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        [TestInitialize]
        public void Init() { 
            _testClass = new ControlArray<int?>();
        }

        [TestMethod()]
        public void SetIndexTest()
        {
            // Act
            _testClass.SetIndex(8, 4);
            _testClass.SetIndex(2, 1);
             // Assert
            Assert.AreEqual(2, _testClass.Count);
            Assert.AreEqual(8, _testClass[5]);
            Assert.AreEqual(2, _testClass[2]);
        }

        [TestMethod()]
        public void GetIndexTest()
        {
            // Arrange
            _testClass[5] = 8;
            _testClass[1] = null!;
            _testClass[2] = 2;

            // Assert
            Assert.AreEqual(4,_testClass.GetIndex(8));
            Assert.AreEqual(1,_testClass.GetIndex(2));
            Assert.AreEqual(-1,_testClass.GetIndex(1));
        }
    }
}