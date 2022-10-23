// ***********************************************************************
// Assembly         : DemoLibraryTests
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-26-2022
// ***********************************************************************
// <copyright file="AddressModelTests.cs" company="DemoLibraryTests">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DemoLibrary.Models;

namespace DemoLibraryTests.Models
{
	/// <summary>
	/// Defines test class AddressModelTests.
	/// </summary>
	[TestClass()]
	public class AddressModelTests {
		/// <summary>
		/// The c exp to string1
		/// </summary>
		private string cExpToString1="1, 4, 2 3, 5";

		/// <summary>
		/// Converts to stringtest.
		/// </summary>
		/// <param name="aStreet">a street.</param>
		/// <param name="aCity">a city.</param>
		/// <param name="aState">a state.</param>
		/// <param name="aZip">a zip.</param>
		/// <param name="aCountry">a.</param>
		/// <param name="ExpToString">The exp to string.</param>
		[DataTestMethod()]
		[DataRow("1", "2", "3", "4", "5", "1, 2, 3 4, 5")]
		[DataRow("123 test street","Los Angelos","WI","90210","", "123 test street, Los Angelos, WI 90210")]
        [DataRow("321 ocean drive", "Santa Monica", "CA", "90901", "USA", "321 ocean drive, Santa Monica, CA 90901, USA")]
        public void ToStringTest(string aStreet, string aCity,string aState, string aZip, string aCountry,string ExpToString) {
			var model = new AddressModel() {
				StreetAddress = aStreet,
				State = aState,
				ZipCode = aZip,
				City = aCity,
				Country = aCountry,
			};
			Assert.AreEqual(ExpToString,model.ToString());
		}

		/// <summary>
		/// Defines the test method ToStringTest1.
		/// </summary>
		[TestMethod()]
		public void ToStringTest1() {
			var model = new AddressModel()
			{
				StreetAddress = "1",
				State = "2",
				ZipCode = "3",
				City = "4",
				Country = "5"
			};
			Assert.AreEqual(cExpToString1, model.ToString());
		}
	}
}
