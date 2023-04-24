// ***********************************************************************
// Assembly         : DemoLibraryTests
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-19-2022
// ***********************************************************************
// <copyright file="PersonModelTests.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DemoLibrary.Models;

namespace DemoLibraryTests.Models
{

	/// <summary>
	/// Defines test class PersonModelTests.
	/// </summary>
	[TestClass]
	public class PersonModelTests {

		/// <summary>
		/// Defines the test method TestPerson.
		/// </summary>
		[TestMethod()]
		public void TestPerson() {
			PersonModel model = new();
			Assert.IsNotNull(model);
		}

		/// <summary>
		/// Tests the person fullName.
		/// </summary>
		/// <param name="aFirstNames">a first names.</param>
		/// <param name="aLastName">a last name.</param>
		/// <param name="aTitle">a title.</param>
		/// <param name="expFullName">The exp fullName.</param>
		[DataTestMethod()]
		[DataRow("","","","")]
		[DataRow("", "1", "", "1")]
		[DataRow("1", "2", "", "1 2")]
		[DataRow("1", "2", "3", "3 1 2")]
		[DataRow("Peter", "Mustermann", "Dr.", "Dr. Peter Mustermann")]
		public void TestPersonFullName(string aFirstNames, string aLastName, string aTitle, string expFullName) {
			PersonModel model = new() { 
				FirstNames = aFirstNames,
				LastName = aLastName,
				PrimaryAddress=null,
				Title= aTitle				};
			Assert.AreEqual(expFullName, model.FullName);
		}
	}
}
