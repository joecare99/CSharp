// ***********************************************************************
// Assembly         : DemoLibrary
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-17-2022
// ***********************************************************************
// <copyright file="DataAccess.cs" company="DemoLibrary">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using DemoLibrary.Models;
using System;
using System.Collections.Generic;

namespace DemoLibrary {
	/// <summary>
	/// Class DataAccess.
	/// </summary>
	public static class DataAccess {
		/// <summary>
		/// The random
		/// </summary>
		static Random rnd = new Random();
		/// <summary>
		/// The street addresses
		/// </summary>
		static string[] streetAddresses = { "101 State Street", "425 Oak Avenue", "7 Wallace Way", "928 Eclesia Place", "123 Winbur House", "543 Venture Drive", "29 Main Avenue" };
		/// <summary>
		/// The cities
		/// </summary>
		static string[] cities = { "Springfield", "Wilshire", "Alexandria", "Franklin", "Clinton", "Fairview", "Boulder", "Denver", "Evangeline", "Georgetown", "Halifax", "Iconia", "Madison" };
		/// <summary>
		/// The states
		/// </summary>
		static string[] states = { "AZ", "CA", "DL", "FL", "GA", "IL", "OK", "PA", "TX", "VA", "WA", "WI" };
		/// <summary>
		/// The zip codes
		/// </summary>
		static string[] zipCodes = { "14121", "08904", "84732", "23410", "60095", "90210", "10456", "60618", "00926", "08701", "90280", "92335", "79936"  };

		/// <summary>
		/// The first names
		/// </summary>
		static string[] firstNames = { "Andrew", "Bob", "Carla", "Dany", "Earl", "Frank", "Georina", "Henry", "Inez", "John", "Karl", "Lenny", "Monique", "Norbert", "Oscar", "Paula", "Richard" };
		/// <summary>
		/// The last names
		/// </summary>
		static string[] lastNames = { "Smith", "Jones", "Garcia", "Hernandez", "Miller", "Santiago", "Thomas", "Lee", "Taylor", "Widmark" };
		/// <summary>
		/// The alive statuses
		/// </summary>
		static bool[] aliveStatuses = { true, false };
		/// <summary>
		/// The low end date
		/// </summary>
		static DateTime lowEndDate = new DateTime(1943, 1, 1);
		/// <summary>
		/// The days from low date
		/// </summary>
		static int daysFromLowDate = (DateTime.Today - lowEndDate).Days;

		//		public DataAccess() {
		//			daysFromLowDate = (DateTime.Today - lowEndDate).Days;
		//		}

		/// <summary>
		/// Gets the people.
		/// </summary>
		/// <param name="total">The total.</param>
		/// <returns>List&lt;PersonModel&gt;.</returns>
		static public List<PersonModel> GetPeople(int total = 10) {
			var output = new List<PersonModel>();
			for (int i = 0; i < total; i++) {
				output.Add(GetPerson(i + 1));
			}
			return output;
		}

		/// <summary>
		/// Gets the person.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>PersonModel.</returns>
		static public PersonModel GetPerson(int id = 1) {
			var output = new PersonModel();
			output.PersonId = id;
			output.FirstNames = GetRandomItem(firstNames);
			output.LastName = GetRandomItem(lastNames);
			output.IsAlive = GetRandomItem(aliveStatuses);
			output.DateOfBirth = GetRandomDate();
			output.Age = GetAgeInYears(output.DateOfBirth);
			output.AccountBalance = ((decimal)rnd.Next(1, 1000000) / 100);

			int addressCount = rnd.Next(1, 5);
			for (int i = 0; i < addressCount; i++) {
				output.Addresses.Add(GetAddress((id - 1) * 5 + i + 1));
			}
			return output;
		}

		/// <summary>
		/// Gets the address.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>AddressModel.</returns>
		static private AddressModel GetAddress(int id = 1) {
			var output = new AddressModel();
			output.AdressId = id;
			output.StreetAddress = GetRandomItem(streetAddresses);
			output.City = GetRandomItem(cities);
			output.State = GetRandomItem(states);
			output.ZipCode = GetRandomItem(zipCodes);
			return output;
		}

		/// <summary>
		/// Gets the random item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Items">The items.</param>
		/// <returns>T.</returns>
		static private T GetRandomItem<T>(T[] Items) => Items[rnd.Next(0,Items.Length)];
		/// <summary>
		/// Gets the random date.
		/// </summary>
		/// <returns>DateTime.</returns>
		static private DateTime GetRandomDate() => lowEndDate.AddDays(rnd.Next(daysFromLowDate));
		/// <summary>
		/// Gets the age in years.
		/// </summary>
		/// <param name="birthday">The birthday.</param>
		/// <returns>System.Int32.</returns>
		static private int GetAgeInYears(DateTime birthday) {
			int age = DateTime.Now.Year - birthday.Year;
			return DateTime.Now < birthday.AddYears(age) ? age - 1 : age;
		}
	}
}
