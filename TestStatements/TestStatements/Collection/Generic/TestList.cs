// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="TestList.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Collection.Generic
{
    /// <summary>
    /// Class Part.
    /// Implements the <see cref="System.IEquatable{TestStatements.Collection.Generic.Part}" />
    /// </summary>
    /// <seealso cref="System.IEquatable{TestStatements.Collection.Generic.Part}" />
    public class Part : IEquatable<Part>
    {
        /// <summary>
        /// Gets or sets the name of the part.
        /// </summary>
        /// <value>The name of the part.</value>
        public string PartName { get; set; }

        /// <summary>
        /// Gets or sets the part identifier.
        /// </summary>
        /// <value>The part identifier.</value>
        public int PartId { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "ID: " + PartId + "   Name: " + PartName;
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Das Objekt, das mit dem aktuellen Objekt verglichen werden soll.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Part objAsPart = obj as Part;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return PartId;
        }
        /// <summary>
        /// Gibt an, ob das aktuelle Objekt gleich einem anderen Objekt des gleichen Typs ist.
        /// </summary>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        /// <returns><see langword="true" />, wenn das aktuelle Objekt gleich dem <paramref name="other" />-Parameter ist, andernfalls <see langword="false" />.</returns>
        public bool Equals(Part other)
        {
            if (other == null) return false;
            return (this.PartId.Equals(other.PartId));
        }
        // Should also override == and != operators.
    }

    /// <summary>
    /// Class TestList.
    /// </summary>
    public static class TestList
    {
        /// <summary>
        /// The parts
        /// </summary>
        private static List<Part> parts;
        /// <summary>
        /// Lists the main.
        /// </summary>
        public static void ListMain()
        {
            const string Title = "Show List<T>";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            CreateTestData();
            ShowList(parts);

            ShowContains();

            ShowInsert();

            ShowIndex();

            ShowRemove1();
            ShowRemove2();

            /*

             ID: 1234   Name: crank arm
             ID: 1334   Name: chain ring
             ID: 1434   Name: regular seat
             ID: 1444   Name: banana seat
             ID: 1534   Name: cassette
             ID: 1634   Name: shift lever

             Contains("1734"): False

             Insert(2, "1834")
             ID: 1234   Name: crank arm
             ID: 1334   Name: chain ring
             ID: 1834   Name: brake lever
             ID: 1434   Name: regular seat
             ID: 1444   Name: banana seat
             ID: 1534   Name: cassette
             ID: 1634   Name: shift lever

             Parts[3]: ID: 1434   Name: regular seat

             Remove("1534")

             ID: 1234   Name: crank arm
             ID: 1334   Name: chain ring
             ID: 1834   Name: brake lever
             ID: 1434   Name: regular seat
             ID: 1444   Name: banana seat
             ID: 1634   Name: shift lever

             RemoveAt(3)

             ID: 1234   Name: crank arm
             ID: 1334   Name: chain ring
             ID: 1834   Name: brake lever
             ID: 1444   Name: banana seat
             ID: 1634   Name: shift lever


         */

        }

        /// <summary>
        /// Creates the test data.
        /// </summary>
        private static void CreateTestData()
        {
            // Create a list of parts.
            parts?.Clear();
            parts = parts ?? new List<Part>();

            // Add parts to the list.
            parts.Add(new Part() { PartName = "crank arm", PartId = 1234 });
            parts.Add(new Part() { PartName = "chain ring", PartId = 1334 });
            parts.Add(new Part() { PartName = "regular seat", PartId = 1434 });
            parts.Add(new Part() { PartName = "banana seat", PartId = 1444 });
            parts.Add(new Part() { PartName = "cassette", PartId = 1534 });
            parts.Add(new Part() { PartName = "shift lever", PartId = 1634 });
        }

        /// <summary>
        /// Shows the list.
        /// </summary>
        /// <param name="parts">The parts.</param>
        private static void ShowList(List<Part> parts)
        {
            // Write out the parts in the list. This will call the overridden ToString method
            // in the Part class.
            Console.WriteLine();
            foreach (Part aPart in parts)
            {
                Console.WriteLine(aPart);
            }
        }

        /// <summary>
        /// Shows the contains.
        /// </summary>
        public static void ShowContains()
        {

            // Check the list for part #1734. This calls the IEquatable.Equals method
            // of the Part class, which checks the PartId for equality.
            const string Title = "Show Contains";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            Console.WriteLine("\nContains(\"1734\"): {0}",
                parts.Contains(new Part { PartId = 1734, PartName = "" }));
        }

        /// <summary>
        /// Shows the insert.
        /// </summary>
        public static void ShowInsert()
        {
            // Insert a new item at position 2.
            const string Title = "Show Insert";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            Console.WriteLine("\nInsert(2, \"1834\") (brake lever)");
            parts.Insert(2, new Part() { PartName = "brake lever", PartId = 1834 });

            ShowList(parts);        
        }

        /// <summary>
        /// Shows the index.
        /// </summary>
        public static void ShowIndex()
        {
            CreateTestData();
            const string Title = "Show Index";
            Console.WriteLine(Constants.Constants.Header2, Title);
            parts.Insert(2, new Part() { PartName = "brake lever", PartId = 1834 });
            Console.WriteLine("\nParts[3]: {0}", parts[3]);
        }

        /// <summary>
        /// Shows the remove1.
        /// </summary>
        public static void ShowRemove1()
        {
            CreateTestData();
            const string Title = "Show Remove";
            Console.WriteLine(Constants.Constants.Header2, Title);
            parts.Insert(2, new Part() { PartName = "brake lever", PartId = 1834 });
  
            Console.WriteLine("\nRemove(\"1534\") (cassette)");
            // This will remove part 1534 even though the PartName is different,
            // because the Equals method only checks PartId for equality.
            parts.Remove(new Part() { PartId = 1534, PartName = "cogs" });

            ShowList(parts);
        }

        /// <summary>
        /// Shows the remove2.
        /// </summary>
        public static void ShowRemove2()
        {
            CreateTestData();
            const string Title = "Show RemoveAt";
            Console.WriteLine(Constants.Constants.Header2, Title);
            parts.Insert(2, new Part() { PartName = "brake lever", PartId = 1834 });
            parts.RemoveAt(5);

            Console.WriteLine("\nRemoveAt(3) (regular seat)");
            // This will remove the part at index 3.
            parts.RemoveAt(3);

            ShowList(parts);
        }

    }

    /// <summary>
    /// Class DinosaurExample.
    /// </summary>
    public static class DinosaurExample
    {
        /// <summary>
        /// The dinosaurs
        /// </summary>
        private static List<string> dinosaurs;
        /// <summary>
        /// Lists the dinos.
        /// </summary>
        public static void ListDinos()
        {
            const string Title = "Dinosaur Example";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            dinosaurs = new List<string>();

            ShowStatus(dinosaurs);

            ShowCreateData();
            ShowContains();
            ShowInsert();
            ShowItemProperty();
            ShowRemove();
            ShowTrimExcess();
            ShowClear();

            /* This code example produces the following output:

            Capacity: 0

            Tyrannosaurus
            Amargasaurus
            Mamenchisaurus
            Deinonychus
            Compsognathus

            Capacity: 8
            Count: 5

            Contains("Deinonychus"): True

            Insert(2, "Compsognathus")

            Tyrannosaurus
            Amargasaurus
            Compsognathus
            Mamenchisaurus
            Deinonychus
            Compsognathus

            dinosaurs[3]: Mamenchisaurus

            Remove("Compsognathus")

            Tyrannosaurus
            Amargasaurus
            Mamenchisaurus
            Deinonychus
            Compsognathus

            TrimExcess()
            Capacity: 5
            Count: 5

            Clear()
            Capacity: 5
            Count: 0
             */
        }

        /// <summary>
        /// Shows the create data.
        /// </summary>
        public static void ShowCreateData()
        {
            const string Title = "Show Create (default) Data";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();

            ShowList(dinosaurs);
            ShowStatus(dinosaurs);
        }

        /// <summary>
        /// Shows the contains.
        /// </summary>
        public static void ShowContains()
        {
            const string Title = "Show Contains";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            Console.WriteLine("\nContains(\"Deinonychus\"): {0}",
                dinosaurs.Contains("Deinonychus"));
        }

        /// <summary>
        /// Shows the insert.
        /// </summary>
        public static void ShowInsert()
        {
            const string Title = "Show Insert";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            Console.WriteLine("\nInsert(2, \"Compsognathus\")");
            dinosaurs.Insert(2, "Compsognathus");

            ShowList(dinosaurs);
            ShowStatus(dinosaurs);
        }

        /// <summary>
        /// Shows the item property.
        /// </summary>
        public static void ShowItemProperty()
        {
            const string Title = "Show Item-Property";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            dinosaurs.Insert(2, "Compsognathus");
            // Shows accessing the list using the Item property.
            Console.WriteLine("\ndinosaurs[3]: {0}", dinosaurs[3]);
        }

        /// <summary>
        /// Shows the remove.
        /// </summary>
        public static void ShowRemove()
        {
            const string Title = "Show Remove";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            dinosaurs.Insert(2, "Compsognathus");

            Console.WriteLine("\nRemove(\"Compsognathus\")");
            dinosaurs.Remove("Compsognathus");

            ShowList(dinosaurs);
            ShowStatus(dinosaurs);
        }

        /// <summary>
        /// Shows the trim excess.
        /// </summary>
        public static void ShowTrimExcess()
        {
            const string Title = "Show TrimExcess";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            dinosaurs.Insert(2, "Compsognathus");
            dinosaurs.Remove("Compsognathus");

            dinosaurs.TrimExcess();
            Console.WriteLine("\nTrimExcess()");

            ShowList(dinosaurs);
            ShowStatus(dinosaurs);
        }

        /// <summary>
        /// Shows the clear.
        /// </summary>
        public static void ShowClear()
        {
            const string Title = "Show Clear";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();

            Console.WriteLine("\nClear()");
            dinosaurs.Clear();

            ShowList(dinosaurs);
            ShowStatus(dinosaurs);
        }

        /// <summary>
        /// Creates the test data.
        /// </summary>
        private static void CreateTestData()
        {
            dinosaurs?.Clear();
            dinosaurs = dinosaurs ?? new List<string>();
            dinosaurs.Add("Tyrannosaurus");
            dinosaurs.Add("Amargasaurus");
            dinosaurs.Add("Mamenchisaurus");
            dinosaurs.Add("Deinonychus");
            dinosaurs.Add("Compsognathus");
        }

        /// <summary>
        /// Shows the list.
        /// </summary>
        /// <param name="dinosaurs">The dinosaurs.</param>
        private static void ShowList(List<string> dinosaurs)
        {
            Console.WriteLine();
            foreach (string dinosaur in dinosaurs)
            {
                Console.WriteLine(dinosaur);
            }
        }

        /// <summary>
        /// Shows the status.
        /// </summary>
        /// <param name="dinosaurs">The dinosaurs.</param>
        private static void ShowStatus(List<string> dinosaurs)
        {
            if (null != dinosaurs)
            {
                Console.WriteLine("Capacity: {0}", dinosaurs?.Capacity);
                Console.WriteLine("Count: {0}", dinosaurs?.Count);
            }
            else
            {
                Console.WriteLine("Is null");
            }
        }
    }
}
