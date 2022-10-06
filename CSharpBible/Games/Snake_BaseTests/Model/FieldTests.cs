using BaseLib.Helper;
using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Base.Model.Tests
{
    /// <summary>
    /// Class TestItem.
    /// Implements the <see cref="IPlacedObject" />
    /// Implements the <see cref="System.IEquatable{Snake_Base.Model.Tests.TestItem}" />
    /// Implements the <see cref="IParentedObject" />
    /// </summary>
    /// <seealso cref="IPlacedObject" />
    /// <seealso cref="System.IEquatable{Snake_Base.Model.Tests.TestItem}" />
    /// <seealso cref="IParentedObject" />
    internal class TestItem : IPlacedObject, IEquatable<TestItem>, IParentedObject
    {
        /// <summary>
        /// The parent
        /// </summary>
        internal object? _parent;
        /// <summary>
        /// The place
        /// </summary>
        internal Point _place;
        private Point _oldplace;
        /// <summary>
        /// The name
        /// </summary>
        public string Name="";

        /// <summary>
        /// Occurs when [log operation].
        /// </summary>
        internal static event Action<string,TestItem, object?, object?, string>? logOperation;
        /// <summary>
        /// Occurs when [on place change].
        /// </summary>
        public event EventHandler<(Point oP, Point nP)> OnPlaceChange;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(TestItem? other)
        {
            return other != null && other.Name == Name;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
        public object? GetParent() => _parent;
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="CallerMember">The caller member.</param>
        public void SetParent(object? value, [CallerMemberName] string CallerMember = "")
        {
            if (_parent == value) return;
            var _old = _parent;
            _parent = value;
            LogOperation("New Parent", _old, _parent, CallerMember);
        }


        /// <summary>
        /// Gets the place.
        /// </summary>
        /// <returns>Point.</returns>
        public Point GetPlace() => _place;
        /// <summary>
        /// Sets the place.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="Name">The name.</param>
        public void SetPlace(Point value, [CallerMemberName] string Name = "") => Property.SetProperty(ref _place, value, 
            (s, o, n) => { _oldplace = o;OnPlaceChange?.Invoke(this, (o, n)); logOperation?.Invoke("Place changed", this, o, n, s); }, Name);
        /// <summary>
        /// Gets the old place.
        /// </summary>
        /// <returns>Point.</returns>
        public Point GetOldPlace() => _oldplace;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"TestItem({Name},{GetPlace()})";
        }

        private void LogOperation(string v, object? old, object? parent, string callerMember) => logOperation?.Invoke(v,this, old, parent, callerMember);
    }

    /// <summary>
    /// Class TestParent.
    /// Implements the <see cref="BaseLib.Interfaces.IHasChildren{Snake_Base.Model.Field{Snake_Base.Model.Tests.TestItem}}" />
    /// Implements the <see cref="IPlacedObject" />
    /// </summary>
    /// <seealso cref="BaseLib.Interfaces.IHasChildren{Snake_Base.Model.Field{Snake_Base.Model.Tests.TestItem}}" />
    /// <seealso cref="IPlacedObject" />
    internal class TestParent : IHasChildren<Field<TestItem>>, IPlacedObject
    {
        /// <summary>
        /// The place
        /// </summary>
        internal Point _place;
        private Point _oldplace;
        /// <summary>
        /// The items
        /// </summary>
        public List<Field<TestItem>> _items = new List<Field<TestItem>>();

        /// <summary>
        /// The log operation
        /// </summary>
        internal static Action<string, TestParent, object?, object?, string>? logOperation;
        /// <summary>
        /// The name
        /// </summary>
        public string Name="";

        /// <summary>
        /// Occurs when [on place change].
        /// </summary>
        public event EventHandler<(Point oP, Point nP)> OnPlaceChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestParent"/> class.
        /// </summary>
        public TestParent() : this("", Point.Empty) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TestParent"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="place">The place.</param>
        public TestParent(string name,Point place)
        {
            _oldplace = 
            _place = place;
            Name = name;
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AddItem(Field<TestItem> value)
        {
            _items.Add(value);
            LogOperation("Items.Add", null, value);
            return true;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>IEnumerable&lt;Field&lt;TestItem&gt;&gt;.</returns>
        public IEnumerable<Field<TestItem>> GetItems() => _items;
        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool RemoveItem(Field<TestItem> value)
        {
            _items.Remove(value);
            LogOperation("Items.Remove", value, null);
            return true;
        }

        /// <summary>
        /// Gets the old place.
        /// </summary>
        /// <returns>Point.</returns>
        public Point GetOldPlace() => _oldplace;
        /// <summary>
        /// Gets the place.
        /// </summary>
        /// <returns>Point.</returns>
        public Point GetPlace() => _place;
        /// <summary>
        /// Sets the place.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="Name">The name.</param>
        public void SetPlace(Point value, [CallerMemberName] string Name = "") => Property.SetProperty(ref _place, value,
            (s, o, n) => { _oldplace = o; OnPlaceChange?.Invoke(this, (n, o)); logOperation?.Invoke("Place changed", this, o, n, s); }, Name);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"TestParent({Name},{GetPlace()})";
        }

        private void LogOperation(string v, object? old, object? parent,[CallerMemberName] string callerMember="") => logOperation?.Invoke(v, this, old, parent, callerMember);

        /// <summary>
        /// Notifies the child change.
        /// </summary>
        /// <param name="childObject">The child object.</param>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        /// <param name="prop">The property.</param>
        public void NotifyChildChange(Field<TestItem> childObject, object oldVal, object newVal, [CallerMemberName] string prop = "")
        {
            
        }
    }

    /// <summary>
    /// Defines test class FieldTests.
    /// </summary>
    [TestClass()]
    public class FieldTests 
    {
        #region Properties
        private Field<TestItem> testField;
        private string ResultData = "";

        #region ExprectedTestData
        private const string cExpResultEmpty = "";
        private const string cExpResult1 = "Place changed: TestItem(Apple,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Apple,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:0)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:1)\to:\tn:TestItem(Apple,{X=7,Y=5})\tc:Items.Add\r\n";
        private const string cExpResult2 = "Place changed: TestItem(Apple,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Apple,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:0)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:1)\to:\tn:TestItem(Apple,{X=7,Y=5})\tc:Items.Add\r\nPlace changed: TestItem(Pear,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Pear,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:1)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:2)\to:\tn:TestItem(Pear,{X=7,Y=5})\tc:Items.Add\r\n";
        private const string cExpResult7 = "Place changed: TestItem(Apple,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Apple,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:0)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:1)\to:\tn:TestItem(Apple,{X=7,Y=5})\tc:Items.Add\r\nPlace changed: TestItem(Pear,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Pear,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:1)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:2)\to:\tn:TestItem(Pear,{X=7,Y=5})\tc:Items.Add\r\nPlace changed: TestItem(Cherry,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Cherry,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:2)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:3)\to:\tn:TestItem(Cherry,{X=7,Y=5})\tc:Items.Add\r\nPlace changed: TestItem(Rasberry,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Rasberry,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:3)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:4)\to:\tn:TestItem(Rasberry,{X=7,Y=5})\tc:Items.Add\r\nPlace changed: TestItem(Strewberry,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Strewberry,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:4)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:5)\to:\tn:TestItem(Strewberry,{X=7,Y=5})\tc:Items.Add\r\nPlace changed: TestItem(Cranberry,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Cranberry,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:5)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:6)\to:\tn:TestItem(Cranberry,{X=7,Y=5})\tc:Items.Add\r\nPlace changed: TestItem(Bird,{X=7,Y=5})\to:{X=0,Y=0}\tn:{X=7,Y=5}\tc:Place\r\nNew Parent: TestItem(Bird,{X=7,Y=5})\to:\tn:Field({X=7,Y=5},I:6)\tc:Parent\r\nDataChange: Field({X=7,Y=5},I:7)\to:\tn:TestItem(Bird,{X=7,Y=5})\tc:Items.Add\r\n";
        private const string cExpResRemApple = "DataChange: Field({X=7,Y=5},I:0)\to:TestItem(Apple,{X=0,Y=0})\tn:\tc:Items.Remove\r\n";
        private const string cExpResRemApple1 = "DataChange: Field({X=7,Y=5},I:1)\to:TestItem(Apple,{X=0,Y=0})\tn:\tc:Items.Remove\r\n";
        private const string cExpResRemPear1 = "DataChange: Field({X=7,Y=5},I:1)\to:TestItem(Pear,{X=0,Y=0})\tn:\tc:Items.Remove\r\n";
        private readonly string cExpResult = cExpResult1;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Tests the initialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            testField = new Field<TestItem>();
            testField.SetPlace(new Point(7, 5));
            testField.DataChangeEvent += TfDataChangedEvent;
            TestItem.logOperation += LogOperation;
            TestParent.logOperation = LogOperation;
            ResultData = cExpResultEmpty;
        }

        /// <summary>
        /// Defines the test method AddItemTest.
        /// </summary>
        [TestMethod()]
        public void AddItemTest()
        {
            var item = new TestItem() { Name = "Apple" };
            Assert.IsTrue(testField.AddItem(item));
            Assert.AreEqual(testField.GetPlace(), item.GetPlace());
            Assert.AreEqual(testField, item.GetParent());
            Assert.IsTrue(testField.Items.Contains(item));
            Assert.IsFalse(testField.AddItem(item));
            Assert.AreEqual(cExpResult, ResultData);
        }

        /// <summary>
        /// Adds the item test2.
        /// </summary>
        /// <param name="sItems">The s items.</param>
        /// <param name="cExpr">The c expr.</param>
        /// <param name="sExpResult">The s exp result.</param>
        [DataTestMethod()]
        [DataRow("Apple", new bool[] { true }, new string[] { cExpResult1 })]
        [DataRow("Apple;Pear", new bool[] { true, true }, new string[] { cExpResult2 })]
        [DataRow("Apple;Apple", new bool[] { true, false }, new string[] { cExpResult1 })]
        public void AddItemTest2(string sItems, bool[] cExpr, string[] sExpResult)
        {
            var i = 0;
            foreach (var sItem in sItems.Split(';'))
            {
                var item = new TestItem() { Name = sItem };
                Assert.AreEqual(cExpr[i], testField.AddItem(item));
                Assert.IsTrue(testField.Items.Contains(item));
                if (cExpr[i])
                {
                    Assert.AreEqual(testField.GetPlace(), item.GetPlace());
                    Assert.AreEqual(testField, item.GetParent());
                }
                else
                {
                    Assert.AreEqual(Point.Empty, item.GetPlace());
                    Assert.AreEqual(null, item.GetParent());
                }
                Assert.IsFalse(testField.AddItem(item));
                i++;
            }
            Assert.AreEqual(sExpResult[0], ResultData);
        }

        /// <summary>
        /// Dummies the data test.
        /// </summary>
        /// <param name="sData">The s data.</param>
        /// <param name="iExpCount">The i exp count.</param>
        /// <param name="sExpResult">The s exp result.</param>
        [DataTestMethod()]
        [DataRow(cExpResultEmpty, 0, new string[] { cExpResultEmpty })]
        [DataRow("Apple",1, new string[] { cExpResult1 })]
        [DataRow("Apple;Apple", 1, new string[] { cExpResult1 })]
        [DataRow("Apple;Pear", 2, new string[] { cExpResult2 })]
        [DataRow("Apple;Pear;Apple", 2, new string[] { cExpResult2 })]
        [DataRow("Apple;Pear;Pear", 2, new string[] { cExpResult2 })]
        [DataRow("Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", 7, new string[] { cExpResult7 })]
        public void DummyDataTest(string sData,int iExpCount, string[] sExpResult)
        {
            GenerateDummyData(sData,false);
            Assert.AreEqual(iExpCount, testField.Items?.Count ?? 0);
            Assert.AreEqual(sExpResult[0], ResultData);
        }

        /// <summary>
        /// Removes the item test.
        /// </summary>
        /// <param name="sData">The s data.</param>
        /// <param name="sRemove">The s remove.</param>
        /// <param name="axExpSuc">The ax exp suc.</param>
        /// <param name="iExpCount">The i exp count.</param>
        /// <param name="sExpResult">The s exp result.</param>
        [DataTestMethod()]
        [DataRow(cExpResultEmpty, "Apple",new bool[] { false }, 0, new string[] { cExpResultEmpty })]
        [DataRow("Apple", "Apple",new bool[] { true }, 0, new string[] { cExpResRemApple })]
        [DataRow("Apple", "Pear",new bool[] { false }, 1, new string[] { cExpResultEmpty })]
        [DataRow("Apple", "Apple;Apple", new bool[] { true, false }, 0, new string[] { cExpResRemApple })]
        [DataRow("Apple", "Apple;Pear", new bool[] { true, false }, 0, new string[] { cExpResRemApple })]
        [DataRow("Apple", "Pear;Apple", new bool[] { false, true }, 0, new string[] { cExpResRemApple })]
        [DataRow("Apple;Pear","Apple", new bool[] { true }, 1, new string[] { cExpResRemApple1 })]
        [DataRow("Apple;Pear", "Pear", new bool[] { true }, 1, new string[] { cExpResRemPear1 })]
        [DataRow("Apple;Pear", "Pear;Apple", new bool[] { true,true }, 0, new string[] { "DataChange: Field({X=7,Y=5},I:1)\to:TestItem(Pear,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:0)\to:TestItem(Apple,{X=0,Y=0})\tn:\tc:Items.Remove\r\n" })]
        [DataRow("Apple;Pear", "Apple;Pear", new bool[] { true, true }, 0, new string[] { "DataChange: Field({X=7,Y=5},I:1)\to:TestItem(Apple,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:0)\to:TestItem(Pear,{X=0,Y=0})\tn:\tc:Items.Remove\r\n" })]
        [DataRow("Apple;Pear", "Rasberry", new bool[] { false }, 2, new string[] { cExpResultEmpty })]
        [DataRow("Apple;Pear;Pear", cExpResultEmpty, new bool[] { false }, 2, new string[] { cExpResultEmpty })]
        [DataRow("Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", "Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", new bool[] { true, true,true,true,true,true,true }, 0, new string[] { "DataChange: Field({X=7,Y=5},I:6)\to:TestItem(Apple,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:5)\to:TestItem(Pear,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:4)\to:TestItem(Cherry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:3)\to:TestItem(Rasberry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:2)\to:TestItem(Strewberry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:1)\to:TestItem(Cranberry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:0)\to:TestItem(Bird,{X=0,Y=0})\tn:\tc:Items.Remove\r\n" })]
        [DataRow("Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", "Bird;Cranberry;Strewberry;Rasberry;Cherry;Pear;Apple", new bool[] { true, true, true, true, true, true, true }, 0, new string[] { "DataChange: Field({X=7,Y=5},I:6)\to:TestItem(Bird,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:5)\to:TestItem(Cranberry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:4)\to:TestItem(Strewberry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:3)\to:TestItem(Rasberry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:2)\to:TestItem(Cherry,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:1)\to:TestItem(Pear,{X=0,Y=0})\tn:\tc:Items.Remove\r\nDataChange: Field({X=7,Y=5},I:0)\to:TestItem(Apple,{X=0,Y=0})\tn:\tc:Items.Remove\r\n" })]
        public void RemoveItemTest(string sData,string sRemove, bool[] axExpSuc,int iExpCount, string[] sExpResult)
        {
            GenerateDummyData(sData);
            var i = 0;
            foreach (var item in sRemove.Split(';'))
            {
                Assert.AreEqual(axExpSuc[i++], testField.RemoveItem(string.IsNullOrEmpty(item)?null: new TestItem() { Name = item }));
            }
            Assert.AreEqual(iExpCount, testField.Items?.Count ?? 0);
            Assert.AreEqual(sExpResult[0], ResultData);
        }

        /// <summary>
        /// Gets the place test.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="p">The p.</param>
        [DataTestMethod()]
        [DataRow("0,9",new int[] { 0, 9 })]
        [DataRow("0,0", new int[] { 0, 0 })]
        [DataRow("0,-1", new int[] { 0, -1 })]
        [DataRow("-1,0", new int[] { -1, 0 })]
        [DataRow("-5,-5", new int[] { -5, -5 })]
        public void GetPlaceTest(string Name,int[] p)
        {
            Point pp = Point.Empty; 
            var _Field = new Field<object>(pp=new Point(p[0], p[1]));
            Assert.AreEqual(pp, _Field.GetPlace());
        }

        /// <summary>
        /// Sets the place test1.
        /// </summary>
        /// <param name="sData">The s data.</param>
        /// <param name="p">The p.</param>
        /// <param name="sExpResult">The s exp result.</param>
        [DataTestMethod()]
        [DataRow("", new int[] { 3, 2 }, new string[] { "DataChange: Field({X=3,Y=2},I:0)\to:{X=7,Y=5}\tn:{X=3,Y=2}\tc:SetPlaceTest1\r\n" })]
        [DataRow("Apple", new int[] { 0, 9 }, new string[] { "Place changed: TestItem(Apple,{X=0,Y=9})\to:{X=7,Y=5}\tn:{X=0,Y=9}\tc:Place\r\nDataChange: Field({X=0,Y=9},I:1)\to:{X=7,Y=5}\tn:{X=0,Y=9}\tc:SetPlaceTest1\r\n" })]
        [DataRow("Apple;Pear", new int[] { 0, 0 }, new string[] { "Place changed: TestItem(Apple,{X=0,Y=0})\to:{X=7,Y=5}\tn:{X=0,Y=0}\tc:Place\r\nPlace changed: TestItem(Pear,{X=0,Y=0})\to:{X=7,Y=5}\tn:{X=0,Y=0}\tc:Place\r\nDataChange: Field({X=0,Y=0},I:2)\to:{X=7,Y=5}\tn:{X=0,Y=0}\tc:SetPlaceTest1\r\n" })]
        [DataRow("Pear;Apple", new int[] { 0, -1 }, new string[] { "Place changed: TestItem(Pear,{X=0,Y=-1})\to:{X=7,Y=5}\tn:{X=0,Y=-1}\tc:Place\r\nPlace changed: TestItem(Apple,{X=0,Y=-1})\to:{X=7,Y=5}\tn:{X=0,Y=-1}\tc:Place\r\nDataChange: Field({X=0,Y=-1},I:2)\to:{X=7,Y=5}\tn:{X=0,Y=-1}\tc:SetPlaceTest1\r\n" })]
        [DataRow("Pear;Apple;Cherry", new int[] { -1, 0 }, new string[] { "Place changed: TestItem(Pear,{X=-1,Y=0})\to:{X=7,Y=5}\tn:{X=-1,Y=0}\tc:Place\r\nPlace changed: TestItem(Apple,{X=-1,Y=0})\to:{X=7,Y=5}\tn:{X=-1,Y=0}\tc:Place\r\nPlace changed: TestItem(Cherry,{X=-1,Y=0})\to:{X=7,Y=5}\tn:{X=-1,Y=0}\tc:Place\r\nDataChange: Field({X=-1,Y=0},I:3)\to:{X=7,Y=5}\tn:{X=-1,Y=0}\tc:SetPlaceTest1\r\n"})]
        [DataRow("Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", new int[] { -5, -5 }, new string[] { "Place changed: TestItem(Apple,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Pear,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Cherry,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Rasberry,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Strewberry,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Cranberry,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Bird,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nDataChange: Field({X=-5,Y=-5},I:7)\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:SetPlaceTest1\r\n" })]
        public void SetPlaceTest1(string sData, int[] p, string[] sExpResult)
        {
            GenerateDummyData(sData);
            Point pp = Point.Empty;
            Point pop = testField.GetPlace();
            foreach (var item in testField.Items)
            {
                Assert.AreEqual(pop, item.GetPlace());
                Assert.AreEqual(pp, item.GetOldPlace());
            }
            testField.SetPlace(pp = new Point(p[0], p[1]));
            Assert.AreEqual(pp, testField.GetPlace());
            Assert.AreEqual(pop, testField.GetOldPlace());
            foreach (var item in testField.Items)
            {
                Assert.AreEqual(pp, item.GetPlace());
                Assert.AreEqual(pop, item.GetOldPlace());
            }
            Assert.AreEqual(sExpResult[0], ResultData);
        }

        /// <summary>
        /// Gets the items test.
        /// </summary>
        /// <param name="sData">The s data.</param>
        /// <param name="iExpCount">The i exp count.</param>
        /// <param name="o">The o.</param>
        [DataTestMethod()]
        [DataRow("", 0, new string[] { cExpResultEmpty })]
        [DataRow("Apple", 1, new string[] { cExpResultEmpty })]
        [DataRow("Apple;Pear", 2, new string[] { cExpResultEmpty })]
        [DataRow("Pear;Apple", 2, new string[] { cExpResultEmpty })]
        [DataRow("Apple;Pear;Pear", 2, new string[] { cExpResultEmpty })]
        [DataRow("Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", 7, new string[] { cExpResultEmpty })]
        public void GetItemsTest(string sData, int iExpCount,params object[] o)
        {
            GenerateDummyData(sData);
            Assert.AreEqual(iExpCount, testField.Items?.Count ?? 0);
        }

        /// <summary>
        /// Sets the parent test.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sData">The s data.</param>
        /// <param name="p">The p.</param>
        /// <param name="sExpResult">The s exp result.</param>
        [DataTestMethod()]
        [DataRow("0,9", "", new int[] { 0, 9 }, new string[] { "DataChange: Field({X=0,Y=9},I:0)\to:{X=7,Y=5}\tn:{X=0,Y=9}\tc:Place\r\nDataChange: Field({X=0,Y=9},I:0)\to:\tn:TestParent(0,9,{X=0,Y=9})\tc:SetParentTest\r\n" })]
        [DataRow("0,0", "Apple", new int[] { 0, 0 }, new string[] { "Place changed: TestItem(Apple,{X=0,Y=0})\to:{X=7,Y=5}\tn:{X=0,Y=0}\tc:Place\r\nDataChange: Field({X=0,Y=0},I:1)\to:{X=7,Y=5}\tn:{X=0,Y=0}\tc:Place\r\nDataChange: Field({X=0,Y=0},I:1)\to:\tn:TestParent(0,0,{X=0,Y=0})\tc:SetParentTest\r\n" })]
        [DataRow("0,-1", "Apple;Pear", new int[] { 0, -1 }, new string[] { "Place changed: TestItem(Apple,{X=0,Y=-1})\to:{X=7,Y=5}\tn:{X=0,Y=-1}\tc:Place\r\nPlace changed: TestItem(Pear,{X=0,Y=-1})\to:{X=7,Y=5}\tn:{X=0,Y=-1}\tc:Place\r\nDataChange: Field({X=0,Y=-1},I:2)\to:{X=7,Y=5}\tn:{X=0,Y=-1}\tc:Place\r\nDataChange: Field({X=0,Y=-1},I:2)\to:\tn:TestParent(0,-1,{X=0,Y=-1})\tc:SetParentTest\r\n" })]
        [DataRow("-1,0", "Pear;Apple", new int[] { -1, 0 }, new string[] { "Place changed: TestItem(Pear,{X=-1,Y=0})\to:{X=7,Y=5}\tn:{X=-1,Y=0}\tc:Place\r\nPlace changed: TestItem(Apple,{X=-1,Y=0})\to:{X=7,Y=5}\tn:{X=-1,Y=0}\tc:Place\r\nDataChange: Field({X=-1,Y=0},I:2)\to:{X=7,Y=5}\tn:{X=-1,Y=0}\tc:Place\r\nDataChange: Field({X=-1,Y=0},I:2)\to:\tn:TestParent(-1,0,{X=-1,Y=0})\tc:SetParentTest\r\n" })]
        [DataRow("-5,-5", "Apple;Pear;Cherry", new int[] { -5, -5 }, new string[] { "Place changed: TestItem(Apple,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Pear,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nPlace changed: TestItem(Cherry,{X=-5,Y=-5})\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nDataChange: Field({X=-5,Y=-5},I:3)\to:{X=7,Y=5}\tn:{X=-5,Y=-5}\tc:Place\r\nDataChange: Field({X=-5,Y=-5},I:3)\to:\tn:TestParent(-5,-5,{X=-5,Y=-5})\tc:SetParentTest\r\n" })]
        [DataRow("-6,-7", "Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", new int[] { -6, -7 }, new string[] { "Place changed: TestItem(Apple,{X=-6,Y=-7})\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nPlace changed: TestItem(Pear,{X=-6,Y=-7})\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nPlace changed: TestItem(Cherry,{X=-6,Y=-7})\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nPlace changed: TestItem(Rasberry,{X=-6,Y=-7})\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nPlace changed: TestItem(Strewberry,{X=-6,Y=-7})\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nPlace changed: TestItem(Cranberry,{X=-6,Y=-7})\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nPlace changed: TestItem(Bird,{X=-6,Y=-7})\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nDataChange: Field({X=-6,Y=-7},I:7)\to:{X=7,Y=5}\tn:{X=-6,Y=-7}\tc:Place\r\nDataChange: Field({X=-6,Y=-7},I:7)\to:\tn:TestParent(-6,-7,{X=-6,Y=-7})\tc:SetParentTest\r\n" })]
        public void SetParentTest(string name, string sData, int[] p, string[] sExpResult)
        {
            GenerateDummyData(sData);
            Point pp = Point.Empty;
            Point pop = testField.GetPlace();
            var _parent = new TestParent(name, pp = new Point(p[0], p[1]));
            Assert.AreEqual(pp, _parent.GetPlace(),"Parent.Place");
            testField.SetParent(_parent);
            Assert.AreEqual(pp, testField.GetPlace(),"Place");
            Assert.AreEqual(pop, testField.GetOldPlace(),"OldPlace");
            foreach (var item in testField.Items)
            {
                Assert.AreEqual(pp, item.GetPlace(),"item.Place");
                Assert.AreEqual(pop, item.GetOldPlace(),"item.OldPlace");
            }
            Assert.AreEqual(sExpResult[0], ResultData);
        }

        /// <summary>
        /// Gets the parent test.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sData">The s data.</param>
        /// <param name="p">The p.</param>
        [DataTestMethod()]
        [DataRow("0,9", "", new int[] { 0, 9 })]
        [DataRow("0,0", "Apple", new int[] { 0, 0 })]
        [DataRow("0,-1", "Apple;Pear", new int[] { 0, -1 })]
        [DataRow("-1,0", "Pear;Apple", new int[] { -1, 0 })]
        [DataRow("-5,-5", "Apple;Pear;Cherry", new int[] { -5, -5 })]
        [DataRow("-6,-7", "Apple;Pear;Cherry;Rasberry;Strewberry;Cranberry;Bird", new int[] { -6, -7 })]
        public void GetParentTest(string name, string sData, int[] p)
        {
            Point pp = Point.Empty;
            var _parent = new TestParent(name,pp = new Point(p[0], p[1]));
            var _field = new Field<TestItem>(Point.Empty, _parent); 
            Assert.AreEqual(_parent, _field.GetParent());
            Assert.AreEqual(pp, _field.GetPlace());
        }

        #region Private Methods
        private void GenerateDummyData(string sData, bool clear = true)
        {
            if (String.IsNullOrEmpty(sData)) return;
            foreach (var sItem in sData.Split(';'))
                testField.AddItem(new TestItem() { Name = sItem });
            if (clear)
                ResultData = cExpResultEmpty;
        }

        private void TfDataChangedEvent(object? sender, (string sender, object? oldVal, object? newVal) e)
        {
            ResultData += $"DataChange: {sender}\to:{e.oldVal}\tn:{e.newVal}\tc:{e.sender}\r\n";
        }

        private void LogOperation(string arg1, object arg2, object? arg3, object? arg4, string arg5)
        {
            ResultData += $"{arg1}: {arg2}\to:{arg3}\tn:{arg4}\tc:{arg5}\r\n";
        }
        #endregion
#endregion
    }
}