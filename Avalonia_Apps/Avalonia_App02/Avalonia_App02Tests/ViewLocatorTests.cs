using Avalonia.Controls.Templates;
using System.Reflection;
using Avalonia_App02.Models.Interfaces;
using NSubstitute;

namespace Avalonia_App02.Tests
{
    [TestClass()]
    public class ViewLocatorTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private ViewLocator testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

        [TestInitialize()]
        public void TestInitialize()
        {
            testClass = new ViewLocator();
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(ViewLocator));
            Assert.IsInstanceOfType(testClass, typeof(IDataTemplate));
        }

        [TestMethod()]
        [DataRow("Avalonia_App02.ViewModels.TemplateViewModel")]
        [DataRow("Avalonia_App02.Models.TemplateModel")]
        [DataRow("System.DateTime")]
        public void BuildTest(string sAct)
        {
            // Arrange
            Type tAct = Assembly.GetAssembly(typeof(App)).GetType(sAct);
            object? obj = null;
            if (tAct != null)
            {
                if (tAct.GetConstructors().First().GetParameters().Count() <= 1)
                    obj = Activator.CreateInstance(tAct);
                else
                    obj = Activator.CreateInstance(tAct, [
                        null,
                        Substitute.For<ISysTime>(),
                        Substitute.For<ICyclTimer>()
                    ]);
            }

            // Act
            var result = testClass.Build(obj);

            // Assert
            if (tAct != null)
                Assert.IsNotNull(result);
            else
                Assert.IsNull(result);
        }

        [TestMethod()]
        public void MatchTest()
        {
            // Arrange

            // Act
            var result = testClass.Match("Test");

            // Assert
            Assert.IsNotNull(result);
        }
    }
}