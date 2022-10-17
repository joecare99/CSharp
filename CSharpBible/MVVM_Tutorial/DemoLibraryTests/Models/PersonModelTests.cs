// ***********************************************************************
// Assembly         : DemoLibraryTests
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-19-2022
// ***********************************************************************
// <copyright file="PersonModelTests.cs" company="DemoLibraryTests">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DemoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibraryTests.Models {

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
			PersonModel model = new PersonModel();
		}

		/// <summary>
		/// Tests the person fullname.
		/// </summary>
		/// <param name="aFirstNames">a first names.</param>
		/// <param name="aLastName">a last name.</param>
		/// <param name="aTitle">a title.</param>
		/// <param name="expFullname">The exp fullname.</param>
		[DataTestMethod()]
		[DataRow("","","","")]
		[DataRow("", "1", "", "1")]
		[DataRow("1", "2", "", "1 2")]
		[DataRow("1", "2", "3", "3 1 2")]
		[DataRow("Peter", "Mustermann", "Dr.", "Dr. Peter Mustermann")]
		public void TestPersonFullname(string aFirstNames, string aLastName, string aTitle, string expFullname) {
			PersonModel model = new PersonModel() { 
				FirstNames = aFirstNames,
				LastName = aLastName,
				Title= aTitle				};
			Assert.AreEqual(expFullname, model.FullName);
		}
	}
}
