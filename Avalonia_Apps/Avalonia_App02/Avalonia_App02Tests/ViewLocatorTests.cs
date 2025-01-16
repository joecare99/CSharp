using Avalonia.Controls.Templates;
using System.Reflection;
using Avalonia_App02.Models.Interfaces;
using NSubstitute;
using Avalonia_App02.ViewModels;
using Avalonia_App02.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Avalonia_App02.Models;
using Avalonia.Controls;
using System.Data;

namespace Avalonia_App02.Tests
{
    [TestClass()]
    public class ViewLocatorTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private ViewLocator testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        public class TestViewModel: ViewModelBase, ISomeTemplateViewModel
        {
            public string Greeting { get; set; } = "Hello World !";

            public string Title => "TestTitle";

            public DateTime Now => new DateTime(2025,01,31);

            public IRelayCommand HomeCommand => throw new NotImplementedException();

            public IRelayCommand ConfigCommand => throw new NotImplementedException();

            public IRelayCommand ProcessCommand => throw new NotImplementedException();

            public IRelayCommand ActionsCommand => throw new NotImplementedException();

            public IRelayCommand MacrosCommand => throw new NotImplementedException();

            public IRelayCommand ReportsCommand => throw new NotImplementedException();

            public IRelayCommand HistoryCommand => throw new NotImplementedException();
        }


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
        [DataRow("null","")]
        [DataRow("Avalonia_App02.Models.SomeTemplateModel, Avalonia_App02","")]
        [DataRow("Avalonia_App02.Tests.ViewLocatorTests+TestViewModel, Avalonia_App02Tests","TextBlock")]
        [DataRow("Avalonia.Controls.Button, Avalonia.Controls", "TextBlock")]
        public void BuildTest(string sAct,string sExp)
        {
            //Debug.WriteLine(typeof(TestViewModel).AssemblyQualifiedName);
            //Debug.WriteLine(typeof(Button).AssemblyQualifiedName);
            //Debug.WriteLine(typeof(SomeTemplateViewModel).AssemblyQualifiedName);
            //Debug.WriteLine(typeof(SomeTemplateModel).AssemblyQualifiedName);
            // Arrange
            Type? tAct = Type.GetType(sAct);
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
            object? result = null;
            if (sAct.EndsWith(nameof(Avalonia_App02)))
            {
                Assert.ThrowsException<MissingMethodException>(() => testClass.Build(obj));
                tAct = null;    
            }
            else
                result = testClass.Build(obj);

            // Assert
            if (tAct != null)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(sExp, result!.GetType().Name);
            }
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