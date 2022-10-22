// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SortedListExample.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace TestStatements.Collection.Generic
{
    /// <summary>
    /// Class SortedListExample.
    /// </summary>
    public static class SortedListExample
    {
        /// <summary>
        /// The open with
        /// </summary>
        private static SortedList<string, string> openWith;

        /// <summary>
        /// Sorteds the list main.
        /// </summary>
        public static void SortedListMain()
        {
            const string Title = "SortedList<TKey,TValue>";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            TestAddExisting();
            TestIndexr();
            ShowTryGetValue();
            ShowContainsKey();
            ShowForEach();

            ShowValues1();
            ShowValues2();

            ShowKeys1();
            ShowKeys2();

            ShowRemove();
        }

        /// <summary>
        /// Shows the values1.
        /// </summary>
        public static void ShowValues1()
        {
            // To get the values alone, use the Values property.
            const string Title = "Show Values - list";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();
            openWith["doc"] = "winword.exe";
            openWith.Add("ht", "hypertrm.exe");

            IList<string> ilistValues = openWith.Values;

            // The elements of the list are strongly typed with the 
            // type that was specified for the SorteList values.
            Console.WriteLine();
            foreach (string s in ilistValues)
            {
                Console.WriteLine("Value = {0}", s);
            }
        }

        /// <summary>
        /// Shows the values2.
        /// </summary>
        public static void ShowValues2()
        {
            // The Values property is an efficient way to retrieve
            // values by index.
            const string Title = "Show Values - index";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();
            openWith["doc"] = "winword.exe";

            Console.WriteLine("\nIndexed retrieval using the Values " +
                "property: Values[2] = {0}", openWith.Values[2]);
        }

        /// <summary>
        /// Shows the keys1.
        /// </summary>
        public static void ShowKeys1()
        {
            // To get the keys alone, use the Keys property.
            const string Title = "Show Keys - list";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();
            openWith["doc"] = "winword.exe";
            openWith.Add("ht", "hypertrm.exe");

            IList<string> ilistKeys = openWith.Keys;

            // The elements of the list are strongly typed with the 
            // type that was specified for the SortedList keys.
            Console.WriteLine();
            foreach (string s in ilistKeys)
            {
                Console.WriteLine("Key = {0}", s);
            }
        }

        /// <summary>
        /// Shows the keys2.
        /// </summary>
        public static void ShowKeys2()
        {
            // The Keys property is an efficient way to retrieve
            // keys by index.
            const string Title = "Show Keys - index";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();
            openWith["doc"] = "winword.exe";

            Console.WriteLine("\nIndexed retrieval using the Keys " +
                "property: Keys[2] = {0}", openWith.Keys[2]);
        }

        /// <summary>
        /// Shows the remove.
        /// </summary>
        public static void ShowRemove()
        {
            // Use the Remove method to remove a key/value pair.
            const string Title = "Show Remove ";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();
            openWith["doc"] = "winword.exe";

            Console.WriteLine("\nRemove(\"doc\")");
            openWith.Remove("doc");

            if (!openWith.ContainsKey("doc"))
            {
                Console.WriteLine("Key \"doc\" is not found.");
            }
        }

        /// <summary>
        /// Shows for each.
        /// </summary>
        public static void ShowForEach()
        {
            // When you use foreach to enumerate list elements,
            // the elements are retrieved as KeyValuePair objects.
            const string Title = "Show ForEach";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();
            openWith["doc"] = "winword.exe";
            openWith.Add("ht", "hypertrm.exe");

            Console.WriteLine();
            foreach (KeyValuePair<string, string> kvp in openWith)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                    kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Shows the contains key.
        /// </summary>
        public static void ShowContainsKey()
        {
            const string Title = "Show ContainsKey";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();

            // ContainsKey can be used to test keys before inserting 
            // them.
            if (!openWith.ContainsKey("ht"))
            {
                openWith.Add("ht", "hypertrm.exe");
                Console.WriteLine("Value added for key = \"ht\": {0}",
                    openWith["ht"]);
            }
        }

        /// <summary>
        /// Shows the try get value.
        /// </summary>
        public static void ShowTryGetValue()
        {
            const string Title = "Show TryGetValue";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();

            // When a program often has to try keys that turn out not to
            // be in the list, TryGetValue can be a more efficient 
            // way to retrieve values.
            string value = "";
            if (openWith.TryGetValue("tif", out value))
            {
                Console.WriteLine("For key = \"tif\", value = {0}.", value);
            }
            else
            {
                Console.WriteLine("Key = \"tif\" is not found.");
            }
        }

        /// <summary>
        /// Tests the indexr.
        /// </summary>
        public static void TestIndexr()
        {
            const string Title = "Use Index to access SortedList";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();

            // The Item property is another name for the indexer, so you 
            // can omit its name when accessing elements. 
            Console.WriteLine("For key = \"rtf\", value = {0}.",
                openWith["rtf"]);

            // The indexer can be used to change the value associated
            // with a key.
            openWith["rtf"] = "winword.exe";
            Console.WriteLine("For key = \"rtf\", value = {0}.",
                openWith["rtf"]);

            // If a key does not exist, setting the indexer for that key
            // adds a new key/value pair.
            openWith["doc"] = "winword.exe";

            // The indexer throws an exception if the requested key is
            // not in the list.
            try
            {
                Console.WriteLine("For key = \"tif\", value = {0}.",
                    openWith["tif"]);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Key = \"tif\" is not found.");
            }
        }

        /// <summary>
        /// Tests the add existing.
        /// </summary>
        public static void TestAddExisting()
        {
            const string Title = "Add Existing Value to SortedList";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestSL();

            // The Add method throws an exception if the new key is 
            // already in the list.
            try
            {
                openWith.Add("txt", "winword.exe");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("An element with Key = \"txt\" already exists.");
            }
        }

        /// <summary>
        /// Creates the test sl.
        /// </summary>
        private static void CreateTestSL()
        {
            // Create a new sorted list of strings, with string
            // keys.
            openWith?.Clear();
            openWith =
                new SortedList<string, string>();

            // Add some elements to the list. There are no 
            // duplicate keys, but some of the values are duplicates.
            openWith.Add("txt", "notepad.exe");
            openWith.Add("bmp", "paint.exe");
            openWith.Add("dib", "paint.exe");
            openWith.Add("rtf", "wordpad.exe");
        }
    }

    /* This code example produces the following output:

    An element with Key = "txt" already exists.
    For key = "rtf", value = wordpad.exe.
    For key = "rtf", value = winword.exe.
    Key = "tif" is not found.
    Key = "tif" is not found.
    Value added for key = "ht": hypertrm.exe

    Key = bmp, Value = paint.exe
    Key = dib, Value = paint.exe
    Key = doc, Value = winword.exe
    Key = ht, Value = hypertrm.exe
    Key = rtf, Value = winword.exe
    Key = txt, Value = notepad.exe

    Value = paint.exe
    Value = paint.exe
    Value = winword.exe
    Value = hypertrm.exe
    Value = winword.exe
    Value = notepad.exe

    Indexed retrieval using the Values property: Values[2] = winword.exe

    Key = bmp
    Key = dib
    Key = doc
    Key = ht
    Key = rtf
    Key = txt

    Indexed retrieval using the Keys property: Keys[2] = doc

    Remove("doc")
    Key "doc" is not found.
     */
}
