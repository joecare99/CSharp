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

namespace DemoLibrary
{
    /// <summary>
    /// Class DataAccess.
    /// </summary>
    public static class DataAccess
    {
        /// <summary>
        /// The random
        /// </summary>
        public static Func<int, int, int> GetNext { get; set; } = (mn, mx) => (_rnd ??= new Random()).Next(mn, mx);
        /// <summary>
        /// The street addresses
        /// </summary>
        static readonly string[] streetAddresses = { "101 State Street", "425 Oak Avenue", "7 Wallace Way", "928 Ecclesia Place", "123 Winbur House", "543 Venture Drive", "29 Main Avenue" };
        /// <summary>
        /// The cities
        /// </summary>
        static readonly string[] cities = { "Springfield", "Wilshire", "Alexandria", "Franklin", "Clinton", "Fairview", "Boulder", "Denver", "Evangeline", "Georgetown", "Halifax", "Iconia", "Madison" };
        /// <summary>
        /// The states
        /// </summary>
        static readonly string[] states = { "AZ", "CA", "DL", "FL", "GA", "IL", "OK", "PA", "TX", "VA", "WA", "WI" };
        /// <summary>
        /// The zip codes
        /// </summary>
        static readonly string[] zipCodes = { "14121", "08904", "84732", "23410", "60095", "90210", "10456", "60618", "00926", "08701", "90280", "92335", "79936" };

        /// <summary>
        /// The first names
        /// </summary>
        static readonly string[] firstNames = { "Andrew", "Bob", "Carla", "Dany", "Earl", "Frank", "Georgina", "Henry", "Inez", "John", "Karl", "Lenny", "Monique", "Norbert", "Oscar", "Paula", "Quentin", "Richard", "Steve", "Urban", "Victor", "Walter", "Xavier", "Yvonne", "Zaharias" };
        /// <summary>
        /// The last names
        /// </summary>
        static readonly string[] lastNames = { "Smith", "Jones", "Garcia", "Hernandez", "Miller", "Santiago", "Thomas", "Lee", "Taylor", "Widmark" };
        /// <summary>
        /// The last names
        /// </summary>
        static readonly string[] titles = { "", "", "", "", "", "Dr.", "Prof.", "Prof. Dr.", "Dr. med.", "Dipl.Ing.", "M.D.", "M.D." };
        /// <summary>
        /// The alive statuses
        /// </summary>
        static readonly bool[] aliveStatuses = { true, false };
        /// <summary>
        /// The low end date
        /// </summary>
        static readonly DateTime lowEndDate = new(1943, 1, 1);
        /// <summary>
        /// The days from low date
        /// </summary>
        static readonly int daysFromLowDate = (DateTime.Today - lowEndDate).Days;
        private static Random _rnd;

        //		public DataAccess() {
        //			daysFromLowDate = (DateTime.Today - lowEndDate).Days;
        //		}

        /// <summary>
        /// Gets the people.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <returns>List&lt;PersonModel&gt;.</returns>
        static public List<PersonModel> GetPeople(int total = 10)
        {
            var output = new List<PersonModel>();
            for (int i = 0; i < total; i++)
            {
                output.Add(GetPerson(i + 1));
            }
            return output;
        }

        /// <summary>
        /// Gets the person.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>PersonModel.</returns>
        static public PersonModel GetPerson(int id = 1)
        {
            var output = new PersonModel
            {
                PersonId = id,
                FirstNames = GetRandomItem(firstNames),
                LastName = GetRandomItem(lastNames),
                IsAlive = GetRandomItem(aliveStatuses),
                Title = GetRandomItem(titles),
                DateOfBirth = GetRandomDate(),
                AccountBalance = ((decimal)GetNext(1, 1000000) / 100),
            };
            output.Age = GetAgeInYears(output.DateOfBirth);

            int addressCount = GetNext(1, 5);
            for (int i = 0; i < addressCount; i++)
            {
                output.Addresses.Add(GetAddress((id - 1) * 5 + i + 1));
            }
            output.PrimaryAddress = output.Addresses[GetNext(0, addressCount - 1)];
            return output;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>AddressModel.</returns>
        static private AddressModel GetAddress(int id = 1)
        {
            var output = new AddressModel
            {
                AddressId = id,
                StreetAddress = GetRandomItem(streetAddresses),
                City = GetRandomItem(cities),
                State = GetRandomItem(states),
                ZipCode = GetRandomItem(zipCodes)
            };
            return output;
        }

        /// <summary>
        /// Gets the random item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Items">The items.</param>
        /// <returns>T.</returns>
        static private T GetRandomItem<T>(T[] Items) => Items[GetNext(0, Items.Length)];
        /// <summary>
        /// Gets the random date.
        /// </summary>
        /// <returns>DateTime.</returns>
        static private DateTime GetRandomDate() => lowEndDate.AddDays(GetNext(0, daysFromLowDate));
        /// <summary>
        /// Gets the age in years.
        /// </summary>
        /// <param name="birthday">The birthday.</param>
        /// <returns>System.Int32.</returns>
        static private int GetAgeInYears(DateTime birthday)
        {
            int age = DateTime.Now.Year - birthday.Year;
            return DateTime.Now < birthday.AddYears(age) ? age - 1 : age;
        }
    }
}
