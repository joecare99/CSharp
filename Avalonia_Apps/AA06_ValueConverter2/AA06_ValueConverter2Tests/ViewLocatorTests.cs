﻿using Avalonia.Controls.Templates;
using System.Reflection;
using AA06_ValueConverter2.Models.Interfaces;
using NSubstitute;
using AA06_ValueConverter2.ViewModels;
using AA06_ValueConverter2.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using AA06_ValueConverter2.Models;
using Avalonia.Controls;
using System.Data;

namespace AA06_ValueConverter2.Tests
{
    [TestClass()]
    public class ViewLocatorTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private ViewLocator testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        public class TestViewModel: ViewModelBase, IValueConverterViewModel
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

            public double InputValue { get ; set ; }

            public double ResultValue => throw new NotImplementedException();
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
        [DataRow("AA06_ValueConverter2.Models.ValueConverterModel, AA06_ValueConverter2","")]
        [DataRow("AA06_ValueConverter2.Tests.ViewLocatorTests+TestViewModel, AA06_ValueConverter2Tests","TextBlock")]
        [DataRow("Avalonia.Controls.Button, Avalonia.Controls", "TextBlock")]
        public void BuildTest(string sAct,string sExp)
        {
            //Debug.WriteLine(typeof(TestViewModel).AssemblyQualifiedName);
            //Debug.WriteLine(typeof(Button).AssemblyQualifiedName);
            //Debug.WriteLine(typeof(ValueConverterViewModel).AssemblyQualifiedName);
            //Debug.WriteLine(typeof(ValueConverterModel).AssemblyQualifiedName);
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
            if (sAct.EndsWith(nameof(AA06_ValueConverter2)))
            {
                Assert.ThrowsExactly<MissingMethodException>(() => testClass.Build(obj));
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
