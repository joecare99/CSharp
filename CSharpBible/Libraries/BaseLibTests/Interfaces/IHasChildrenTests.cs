using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BaseLib_netTests.Interfaces
{
#pragma warning disable CS8634 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der class-Einschränkung.
    class TestIHCClass : IHasChildren<object?>
#pragma warning restore CS8634 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der class-Einschränkung.
    {
        private readonly List<object?> _children=new();
        public bool AddItem(object? value)
        {
            _children.Add(value);
            return true;
        }

        public IEnumerable<object?> GetItems()
        {
            return _children;
        }

        public void NotifyChildChange(object? childObject, object oldVal, object newVal, [CallerMemberName] string prop = "")
        {
            throw new NotImplementedException();
        }

        public bool RemoveItem(object? value)
        {
            return _children.Remove(value);
        }
    }

    [TestClass]
    public class IHasChildrenTests
    {
#pragma warning disable CS8634 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der class-Einschränkung.
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private IHasChildren<object?> _class;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#pragma warning restore CS8634 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der class-Einschränkung.

        [TestInitialize]
        public void Init()
        {
            _class = new TestIHCClass();
            _class.AddItem("Test");
            _class.AddItem(5);
            _class.AddItem(7.2d);
            _class.AddItem(null);
        }

#if NET5_0_OR_GREATER
        [TestMethod]
        [DataRow(0,"Test")]
        [DataRow(1, 5)]
        [DataRow(2, 7.2d)]
        [DataRow(3, null)]
        public void ItemsTest(int ix,object? oExp) {
            Assert.AreEqual(oExp, _class.Items.ToList()[ix]);  
        }
#endif

        [TestMethod]
        [DataRow(0, "Test")]
        [DataRow(1, 5)]
        [DataRow(2, 7.2d)]
        [DataRow(3, null)]
        public void GetItemsTest(int ix, object? oExp)
        {
            Assert.AreEqual(oExp, _class.GetItems().ToList()[ix]);
        }


    }
}
