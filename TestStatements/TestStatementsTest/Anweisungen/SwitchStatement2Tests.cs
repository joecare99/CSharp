using Microsoft.CodeAnalysis.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class SwitchStatement2Tests:ConsoleTestsBase
    {
        static IEnumerable<object[]> ShapeTestData => new[] { 
            new object[] { new Square(10) ,40d,"Information about square:\r\n   Length of a side: 10\r\n   Area: 100"},
            new object[] { new Rectangle(5, 7),24d, "Information about rectangle:\r\n   Dimensions: 5 x 7\r\n   Area: 35"},
            new object[] { (Shape?)null! ,null!,"An uninitialized shape"},
            new object[] { (Shape?)null! ,null!,"An uninitialized shape"},
            new object[] { new Square(0),0d, "The shape: Square with no dimensions"},
            new object[] { new Rectangle(8, 8),32d, "Information about square rectangle:\r\n   Length of a side: 8\r\n   Area: 64"},
            new object[] { new Circle(3), Math.PI*6d, "A Circle shape"},
        };
        [DataTestMethod()]
        [DynamicData(nameof(ShapeTestData))]
        public void ShowShapeInfoTest(Shape sh,double _,string sExp)
        {

            AssertConsoleOutput(sExp, ()=>SwitchStatement2.ShowShapeInfo(sh));
        }

        [DataTestMethod()]
        [DynamicData(nameof(ShapeTestData))]
        public void ShapeCircumfenceTest(Shape sh, double? fExp, string _)
        {

            Assert.AreEqual(fExp, sh?.Circumference);
        }

        [DataTestMethod()]
    
        public void SwitchExample21Test()
        {
            AssertConsoleOutput(@"Information about square:
   Length of a side: 10
   Area: 100
Information about rectangle:
   Dimensions: 5 x 7
   Area: 35
An uninitialized shape
The shape: Square with no dimensions
Information about square rectangle:
   Length of a side: 8
   Area: 64
A Circle shape", SwitchStatement2.SwitchExample21);
        }

    }
}