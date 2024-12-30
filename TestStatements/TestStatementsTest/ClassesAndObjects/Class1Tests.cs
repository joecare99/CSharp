using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.ClassesAndObjects.Tests
{
    [TestClass]
    public class Class123Tests:ConsoleTestsBase
    {
        [TestMethod]
        public void Class1Test()
        {
            var c = new Class1();
            Assert.IsNotNull(c);
            Assert.AreEqual("I am Class1", c.ToString());
        }

        [TestMethod]
        public void Class2Test()
        {
            var c = new Class2();
            Assert.IsNotNull(c);
            Assert.AreEqual("I am Class2", c.ToString());
            AssertConsoleOutput("Quack !", c.Quack);
        }

        [TestMethod]
        public void Class3Test()
        {
            var c = new Class3();
            Assert.IsNotNull(c);
            Assert.AreEqual("I am Class3", c.ToString());
            AssertConsoleOutput("Moving ...", c.Move);
        }

        [TestMethod]
        public void Class4Test()
        {
            var c = new Class4();
            Assert.IsNotNull(c);
            Assert.AreEqual("I am Class4", c.ToString());
            AssertConsoleOutput("Moving ...", c.Move);
            AssertConsoleOutput("Rolling ...", c.Roll);
        }

        [TestMethod]
        public void Class5Test()
        {
            var c = new Class5();
            Assert.IsNotNull(c);
            Assert.AreEqual("I am Class5", c.ToString());
            AssertConsoleOutput("Moving ...", c.Move);
            AssertConsoleOutput("Rolling ...", c.Roll);
            AssertConsoleOutput("Quack !", c.Quack);
        }

        [TestMethod]
        public void InterfaceTestTest()
        {
            AssertConsoleOutput(@"I am Class1
not able to Quack ...
Not able to move ...
Not able to roll ...
=============================
I am Class2
It can Quack ...
Quack !
Not able to move ...
Not able to roll ...
=============================
I am Class3
not able to Quack ...
It can move ...
Moving ...
Not able to roll ...
=============================
I am Class4
not able to Quack ...
It can move ...
Moving ...
It can roll ...
Rolling ...
=============================
I am Class5
It can Quack ...
Quack !
It can move ...
Moving ...
It can roll ...
Rolling ...
=============================", InterfaceTest.Run);
        }

    }
}
