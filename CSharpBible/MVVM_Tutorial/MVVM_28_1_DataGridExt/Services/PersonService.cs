using MVVM_28_1_DataGridExt.Models;
using System;
using System.Collections.Generic;

namespace MVVM_28_1_DataGridExt.Services
{
    public class PersonService
    {
        /// <summary>
        /// The random
        /// </summary>
        public static Func<int, int, int> GetNext { get; set; } = (mn, mx) => (_rnd ??= new Random()).Next(mn, mx);
        private static Random _rnd;
        /// <summary>
        /// The first names
        /// </summary>
        static readonly string[] firstNames = { "Andrew", "Bob", "Carla", "Dany", "Earl", "Frank", "Georgina", "Henry", "Inez", "John", "Karl", "Lenny", "Monique", "Norbert", "Oscar", "Paula", "Quentin", "Richard", "Steve", "Theodor", "Urban", "Victor", "Walter", "Xavier", "Yvonne", "Zaharias" };
        /// <summary>
        /// The last names
        /// </summary>
        static readonly string[] lastNames = { "Smith", "Jones", "Garcia", "Hernandez", "Miller", "Santiago", "Thomas", "Lee", "Taylor", "Widmark" };
        /// <summary>
        /// The last names
        /// </summary>
        static readonly string[] titles = { "", "", "", "", "", "Dr.", "Prof.", "Prof. Dr.", "Dr. med.", "Dipl.Ing.", "M.D.", "M.D." };

        /// <summary>
        /// The low end date
        /// </summary>
        static readonly DateTime lowEndDate = new(1953, 1, 1);
        /// <summary>
        /// The days from low date
        /// </summary>
        static readonly int daysFromLowDate = (DateTime.Today - lowEndDate).Days-7000;

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

        public IEnumerable<Person> GetPersons()
        {
            var deps = GetDepartments();
            var _id = 5; 
            return new Person[]
            {
                    new(){ FirstName="Max", LastName="Muster", Email="max@muster.com", Id=1, Department = deps[0] },
                    new(){ FirstName="Susi", LastName="Müller", Email="susi@muster.com", Id=2, Department = deps[1], Birthday= new DateTime(1980,1,1) },
                    new(){ FirstName="Dave", LastName="Dev", Email="dev.dave@muster.com", Id=3, Department = deps[3], Birthday= new DateTime(1988,5,2) },
                    new(){ FirstName="Herbert", LastName="Bossinger", Email="ceo@muster.com", Id=4, Department = deps[2], Birthday= new DateTime(1999,7,7) },
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
                    RandomPerson(ref _id,deps),
            };

            static Person RandomPerson(ref int _id, Department[] deps)
            {
                string fn, ln;
                return new() { 
                    FirstName =(fn= GetRandomItem(firstNames)), 
                    LastName = (ln=GetRandomItem(lastNames)), 
                    Email =  $"{fn.ToLower()}.{ln.ToLower()}@muster.com", 
                    Id = _id++, 
                    Department = GetRandomItem(deps), 
                    Birthday = GetRandomDate() };
            }
        }
        public Department[] GetDepartments() 
            => new Department[]{
                new(){Id = 1},
                new(){Id = 2},
                new(){Id = 3},
                new(){Id = 4} }
                ;
    }
}
