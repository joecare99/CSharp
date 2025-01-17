using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Views.ValueConverter.Tests
{
    [TestClass()]
    public class Xaml2ElementConverterTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        Xaml2ElementConverter testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        [TestInitialize()]
        public void Init() {
            testClass = new Xaml2ElementConverter();
        }
        [TestMethod()]
        [DataRow(null,"null")]
        [DataRow("<Button />","")]
        public void ConvertTest(string sAct,string sExp)
        {
            var result=testClass.Convert(sAct,typeof(object),null,System.Globalization.CultureInfo.CurrentCulture);
            if (result == null ) 
                Assert.AreEqual(sExp,"null");
                else
                Assert.AreEqual(sExp,result.GetType().Name);
        }

        [TestMethod()]
        public void ConvertBackTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => testClass.ConvertBack(null!, typeof(string), null!, System.Globalization.CultureInfo.CurrentCulture));
        }
    }
}