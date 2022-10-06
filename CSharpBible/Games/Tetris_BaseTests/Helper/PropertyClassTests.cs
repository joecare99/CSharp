using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris_Base.Helper;

namespace Tetris_Base.Helper.Tests {
	/// <summary>
	/// Defines test class PropertyClassTests.
	/// </summary>
	[TestClass()]
	public class PropertyClassTests {
		/// <summary>
		/// Sets the property test.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="oExp">The o exp.</param>
		/// <param name="xExp">if set to <c>true</c> [x exp].</param>
		/// <param name="value">The value.</param>
		/// <param name="data">The data.</param>
		[DataTestMethod()]
		[DataRow("Null", null, false, null, null)]
		[DataRow("int,Null", null, true, null, 1)]
		[DataRow("Null,int", 1, true, 1, null)]
		[DataRow("int,int =", 1, false, 1, 1)]
		[DataRow("int,int !=", 2, true, 2, 1)]
		[DataRow("float,Null", null, true, null, 1.0)]
		[DataRow("Null,float", 1.0, true, 1.0, null)]
		[DataRow("float,float =", 1.0, false, 1.0, 1.0)]
		[DataRow("float,float !=", 2.0, true, 2.0, 1.0)]
		[DataRow("object[],Null", null, true, null, new object[] { 1, 2, 3 })]
		//        [DataRow("Null,class", new object[] { 1, 2, 3 }, true, new object[] { 1, 2, 3 }, null)]
		//        [DataRow("class,class =", new object[] { 1, 2, 3 }, false, new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 })]
		//        [DataRow("class,class !=", 2.0, true, new object[] { 3, 2, 1 }, new object[] { 1, 2, 3 })]
		[DataRow("string,Null", null, true, null, "Hallo")]
		[DataRow("Null,string", "Hallo", true, "Hallo", null)]
		[DataRow("string,string =", "Hallo", false, "Hallo", "Hallo")]
		[DataRow("string,string !=", "Welt", true, "Welt", "Hallo")]

		public void SetPropertyTest(string Name, object oExp, bool xExp, object value, object data) {
			var _data = data;
			var flag = false;
			Assert.AreEqual(xExp, PropertyClass.SetProperty(ref _data, value, (o, n) => {
				flag = true;
				Assert.AreEqual(_data, n, $"{Name}.data");
				Assert.AreEqual(data, o, $"{Name}.old");
				Assert.AreEqual(value, n, $"{Name}.new");
			}));
			Assert.AreEqual(xExp, flag);
			Assert.AreEqual(oExp, _data);
		}

		/// <summary>
		/// Sets the property p test.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="oExp">The o exp.</param>
		/// <param name="xExp">if set to <c>true</c> [x exp].</param>
		/// <param name="value">The value.</param>
		/// <param name="data">The data.</param>
		[DataTestMethod()]
		[DataRow("Null", null, false, null, null)]
		[DataRow("int,Null", null, true, null, 1)]
		[DataRow("Null,int", 1, true, 1, null)]
		[DataRow("int,int =", 1, false, 1, 1)]
		[DataRow("int,int !=", 2, true, 2, 1)]
		[DataRow("float,Null", null, true, null, 1.0)]
		[DataRow("Null,float", 1.0, true, 1.0, null)]
		[DataRow("float,float =", 1.0, false, 1.0, 1.0)]
		[DataRow("float,float !=", 2.0, true, 2.0, 1.0)]
		[DataRow("object[],Null", null, true, null, new object[] { 1, 2, 3 })]
		//        [DataRow("Null,class", new object[] { 1, 2, 3 }, true, new object[] { 1, 2, 3 }, null)]
		//        [DataRow("class,class =", new object[] { 1, 2, 3 }, false, new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 })]
		//        [DataRow("class,class !=", 2.0, true, new object[] { 3, 2, 1 }, new object[] { 1, 2, 3 })]
		[DataRow("string,Null", null, true, null, "Hallo")]
		[DataRow("Null,string", "Hallo", true, "Hallo", null)]
		[DataRow("string,string =", "Hallo", false, "Hallo", "Hallo")]
		[DataRow("string,string !=", "Welt", true, "Welt", "Hallo")]

		public void SetPropertyPTest(string Name, object oExp, bool xExp, object value, object data) {
			var _data = data;
			var flag = false;
			Assert.AreEqual(xExp, PropertyClass.SetPropertyP(ref _data, value, (o, n) => {
				flag = true;
				Assert.AreEqual(_data, o, $"{Name}.data");
				Assert.AreEqual(data, o, $"{Name}.old");
				Assert.AreEqual(value, n, $"{Name}.new");
			}));
			Assert.AreEqual(xExp, flag);
			Assert.AreEqual(oExp, _data);
		}

	}
}
