using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVM.ViewModel.Tests
{
    [TestClass]
    public class GetResultTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        GetResult testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init() { 
            testClass = new();
        }

        [TestMethod]
        public void SetupTest() {
            testClass.Should().NotBeNull();
            testClass.Should().BeOfType<GetResult>();
            testClass.Should().BeAssignableTo<IGetResult>();
            testClass.Count.Should().Be(0);
        }

        [TestMethod]
        public void RegisterTest()
        {
            testClass.Register("Test1",(o)=>$"Test1({string.Join(", ",o)})");
            testClass.Count.Should().Be(1);
            testClass.Register("Test2", (o) => $"Test2({string.Join(", ", o)})");
            testClass.Count.Should().Be(2);
            testClass.Register("Test3", (o) => null);
            testClass.Count.Should().Be(3);
            testClass.Register("Test3", (o) => $"Test3({string.Join(", ", o)})");
            testClass.Count.Should().Be(3);
        }

        [DataTestMethod]
        [DataRow("Test1", new[] {"Hello" }, "Test1(Hello)")]
        [DataRow("Test1", new[] { "Hello", "World" }, "Test1(Hello, World)")]
        [DataRow("Test1", new[] { "Hello", "World","!" }, "Test1(Hello, World, !)")]
        public void Test1(string _, object[] param, string? sExp) {
            testClass.Get(param).Should().BeNull();
            RegisterTest();
            testClass.Get(param).Should().Be(sExp);
        }

        [DataTestMethod]
        [DataRow("Test3", new[] { "Hello" }, "Test3(Hello)")]
        [DataRow("Test2", new[] { "Hello", "World" }, "Test2(Hello, World)")]
        [DataRow("Test1", new[] { "Hello", "new", "World" }, "Test1(Hello, new, World)")]
        public void GetTest(string name, object[] param, string? sExp)
        {
            testClass.Get(param,name).Should().BeNull();
            RegisterTest();
            testClass.Get(param,name).Should().Be(sExp);
        }
    }
}
