// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-10-2022
// ***********************************************************************
// <copyright file="ComparerExample.cs" company="HP Inc.">
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
    /// Class ComparerExample.
    /// </summary>
    public static class ComparerExample
    {
        /// <summary>
        /// The boxes
        /// </summary>
        private static List<Box> Boxes;
        /// <summary>
        /// Main procedure of Comparer-example.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void ComparerExampleMain(string[] args)
        {

            const string Title = "Comparer<T>";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            ShowSortWithLengthFirstComparer();

            ShowSortwithDefaultComparer();

            ShowLengthFirstComparer();
        }

        /// <summary>
        /// Creates the test data.
        /// </summary>
        private static void CreateTestData()
        {
            Boxes?.Clear();
            Boxes = new List<Box>()
            {
                new Box(4, 20, 14),
                new Box(12, 12, 12),
                new Box(8, 20, 10),
                new Box(6, 10, 2),
                new Box(2, 8, 4),
                new Box(2, 6, 8),
                new Box(4, 12, 20),
                new Box(18, 10, 4),
                new Box(24, 4, 18),
                new Box(10, 4, 16),
                new Box(10, 2, 10),
                new Box(6, 18, 2),
                new Box(8, 12, 4),
                new Box(12, 10, 8),
                new Box(14, 6, 6),
                new Box(16, 6, 16),
                new Box(2, 8, 12),
                new Box(4, 24, 8),
                new Box(8, 6, 20),
                new Box(18, 18, 12)
            };
        }

        /// <summary>
        /// Shows the sort with length first comparer.
        /// </summary>
        public static void ShowSortWithLengthFirstComparer()
        {
            // Sort by an Comparer<T> implementation that sorts
            // first by the length.
            Console.WriteLine(Constants.Constants.Header2, "Show Sort with BoxLengthFirst - comparer");
            CreateTestData();

            Boxes.Sort(new BoxLengthFirst());

            Console.WriteLine("H - L - W");
            Console.WriteLine("==========");
            foreach (Box bx in Boxes)
            {
                Console.WriteLine("{0}\t{1}\t{2}",
                    bx.Height.ToString(), bx.Length.ToString(),
                    bx.Width.ToString());
            }
        }

        /// <summary>
        /// Shows the sortwith default comparer.
        /// </summary>
        public static void ShowSortwithDefaultComparer()
        {

            Console.WriteLine(Constants.Constants.Header2, "Show Sort with default - comparer");
            CreateTestData();

            Console.WriteLine();
            Console.WriteLine("H - L - W");
            Console.WriteLine("==========");

            // Get the default comparer that 
            // sorts first by the height.
            Comparer<Box> defComp = Comparer<Box>.Default;

            // Calling Boxes.Sort() with no parameter
            // is the same as calling Boxs.Sort(defComp)
            // because they are both using the default comparer.
            Boxes.Sort();

            foreach (Box bx in Boxes)
            {
                Console.WriteLine("{0}\t{1}\t{2}",
                    bx.Height.ToString(), bx.Length.ToString(),
                    bx.Width.ToString());
            }
        }

        /// <summary>
        /// Shows the length first comparer.
        /// </summary>
        public static void ShowLengthFirstComparer()
        {
            // This explicit interface implementation
            // compares first by the length.
            // Returns -1 because the length of BoxA
            // is less than the length of BoxB.
            Console.WriteLine(Constants.Constants.Header2, "Show use of explicit - comparer");
            BoxLengthFirst LengthFirst = new BoxLengthFirst();

            Comparer<Box> bc = (Comparer<Box>)LengthFirst;

            Box BoxA = new Box(2, 6, 8);
            Box BoxB = new Box(10, 12, 14);
            int x = LengthFirst.Compare(BoxA, BoxB);
            Console.WriteLine();
            Console.WriteLine(x.ToString());
        }
    }

    /// <summary>
    /// Class BoxLengthFirst.
    /// Implements the <see cref="System.Collections.Generic.Comparer{TestStatements.Collection.Generic.Box}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Comparer{TestStatements.Collection.Generic.Box}" />
    public class BoxLengthFirst : Comparer<Box>
    {
        // Compares by Length, Height, and Width.
        /// <summary>
        /// Vergleicht beim Überschreiben in einer abgeleiteten Klasse zwei Objekte gleichen Typs und gibt über den zurückgegebenen Wert an, ob das eine Objekt kleiner, größer oder gleich dem anderen Objekt ist.
        /// </summary>
        /// <param name="x">Das erste zu vergleichende Objekt.</param>
        /// <param name="y">Das zweite zu vergleichende Objekt.</param>
        /// <returns>Eine ganze Zahl mit Vorzeichen, die die relativen Werte von <paramref name="x" /> und <paramref name="y" /> angibt, wie in der folgenden Tabelle veranschaulicht.
        /// Wert
        /// Bedeutung
        /// Kleiner als 0 (null)
        /// <paramref name="x" /> ist kleiner als <paramref name="y" />.
        /// Zero
        /// <paramref name="x" /> ist gleich <paramref name="y" />.
        /// Größer als 0 (null)
        /// <paramref name="x" /> ist größer als <paramref name="y" />.</returns>
        public override int Compare(Box x, Box y)
        {
            if (x.Length.CompareTo(y.Length) != 0)
            {
                return x.Length.CompareTo(y.Length);
            }
            else if (x.Height.CompareTo(y.Height) != 0)
            {
                return x.Height.CompareTo(y.Height);
            }
            else if (x.Width.CompareTo(y.Width) != 0)
            {
                return x.Width.CompareTo(y.Width);
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Class BoxComp.
    /// Implements the <see cref="System.Collections.Generic.IComparer{TestStatements.Collection.Generic.Box}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IComparer{TestStatements.Collection.Generic.Box}" />
    public class BoxComp : IComparer<Box>
    {
        /// <summary>
        /// Vergleicht zwei Objekte und gibt einen Wert zurück, der angibt, ob ein Wert niedriger, gleich oder größer als der andere Wert ist.
        /// </summary>
        /// <param name="x">Das erste zu vergleichende Objekt.</param>
        /// <param name="y">Das zweite zu vergleichende Objekt.</param>
        /// <returns>Eine ganze Zahl mit Vorzeichen, die die relativen Werte von <paramref name="x" /> und <paramref name="y" /> angibt, wie in der folgenden Tabelle veranschaulicht.
        /// Wert   Bedeutung   Kleiner als 0 (null)  <paramref name="x" /> ist kleiner als <paramref name="y" />.   0 (null)  <paramref name="x" /> ist gleich <paramref name="y" />.   Größer als 0 (null)  <paramref name="x" /> ist größer als <paramref name="y" />.</returns>
        public int Compare(Box x, Box y)
        {
            if (x.Height.CompareTo(y.Height) != 0)
            {
                return x.Height.CompareTo(y.Height);
            }
            else if (x.Length.CompareTo(y.Length) != 0)
            {
                return x.Length.CompareTo(y.Length);
            }
            else if (x.Width.CompareTo(y.Width) != 0)
            {
                return x.Width.CompareTo(y.Width);
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Class Box.
    /// Implements the <see cref="System.IComparable{TestStatements.Collection.Generic.Box}" />
    /// </summary>
    /// <seealso cref="System.IComparable{TestStatements.Collection.Generic.Box}" />
    public class Box : IComparable<Box>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Box" /> class.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="l">The l.</param>
        /// <param name="w">The w.</param>
        public Box(int h, int l, int w)
        {
            this.Height = h;
            this.Length = l;
            this.Width = w;
        }
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; private set; }
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length { get; private set; }
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; private set; }

        /// <summary>
        /// Vergleicht die aktuelle Instanz mit einem anderen Objekt vom selben Typ und gibt eine ganze Zahl zurück, die angibt, ob die aktuelle Instanz in der Sortierreihenfolge vor oder nach dem anderen Objekt oder an derselben Position auftritt.
        /// </summary>
        /// <param name="other">Ein Objekt, das mit dieser Instanz verglichen werden soll.</param>
        /// <returns>Ein Wert, der die relative Reihenfolge der verglichenen Objekte angibt. Der Rückgabewert hat folgende Bedeutung:
        /// Wert   Bedeutung   Kleiner als 0 (null)   Diese Instanz steht in der Sortierreihenfolge vor <paramref name="other" />.   0 (null)   Diese Instanz tritt in der Sortierreihenfolge an der gleichen Position wie <paramref name="other" /> auf.   Größer als 0 (null)   Diese Instanz folgt in der Sortierreihenfolge auf <paramref name="other" />.</returns>
        public int CompareTo(Box other)
        {
            // Compares Height, Length, and Width.
            if (this.Height.CompareTo(other.Height) != 0)
            {
                return this.Height.CompareTo(other.Height);
            }
            else if (this.Length.CompareTo(other.Length) != 0)
            {
                return this.Length.CompareTo(other.Length);
            }
            else if (this.Width.CompareTo(other.Width) != 0)
            {
                return this.Width.CompareTo(other.Width);
            }
            else
            {
                return 0;
            }
        }
    }
}
