using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Collection.Generic
{
    public static class DictionaryExample
    {
        private static Dictionary<string, string> openWith;
        public static void DictionaryExampleMain()
        {
            const string Title = "Dictionary<TKey,TValue>";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            CreateTestData();

            ShowDict(openWith);

            TryAddExisting();

            ShowIndex1();

            ShowIndex2();

            ShowIndex3();

            ShowIndex4();

            ShowTryGetValue();

            ShowContainsKey();

            ShowDict(openWith);

            ShowValueCollection();

            ShowKeyCollection();

            ShowRemove();
        }

        public static void TryAddExisting()
        {
            // The Add method throws an exception if the new key is 
            // already in the dictionary.
            const string Title = "Show Try to add Existing";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();

            try
            {
                openWith.Add("txt", "winword.exe");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("An element with Key = \"txt\" already exists.");
            }
        }

        public static void ShowIndex1()
        {
            // The Item property is another name for the indexer, so you 
            // can omit its name when accessing elements. 
            const string Title = "Show Index (get value)";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();

            Console.WriteLine("For key = \"rtf\", value = {0}.",
                openWith["rtf"]);
        }

        public static void ShowIndex2()
        {
            // The indexer can be used to change the value associated
            // with a key.
            const string Title = "Show Index (change value)";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();

            openWith["rtf"] = "winword.exe";
            Console.WriteLine("For key = \"rtf\", value = {0}.",
                openWith["rtf"]);

            ShowDict(openWith);
        }

        public static void ShowIndex3()
        {
            // If a key does not exist, setting the indexer for that key
            // adds a new key/value pair.
            const string Title = "Show Index (add value)";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            openWith["rtf"] = "winword.exe";

            openWith["doc"] = "winword.exe";
            ShowDict(openWith);
        }

        public static void ShowIndex4()
        {
            // The indexer throws an exception if the requested key is
            // not in the dictionary.
            const string Title = "Show Index (get exception)";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            openWith["rtf"] = "winword.exe";
            openWith["doc"] = "winword.exe";

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

        public static void ShowTryGetValue()
        {
            // When a program often has to try keys that turn out not to
            // be in the dictionary, TryGetValue can be a more efficient 
            // way to retrieve values.
            const string Title = "Show TryGetValue";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            openWith["rtf"] = "winword.exe";
            openWith["doc"] = "winword.exe";

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

        public static void ShowContainsKey()
        {
            // ContainsKey can be used to test keys before inserting 
            // them.
            const string Title = "Show ContainsKey";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            openWith["rtf"] = "winword.exe";
            openWith["doc"] = "winword.exe";

            if (!openWith.ContainsKey("ht"))
            {
                openWith.Add("ht", "hypertrm.exe");
                Console.WriteLine("Value added for key = \"ht\": {0}",
                    openWith["ht"]);
            }
        }

        public static void ShowValueCollection()
        {
            // To get the values alone, use the Values property.
            const string Title = "Show ValueCollection";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            openWith["rtf"] = "winword.exe";
            openWith["doc"] = "winword.exe";
            openWith.Add("ht", "hypertrm.exe");

            Dictionary<string, string>.ValueCollection valueColl =
                openWith.Values;

            // The elements of the ValueCollection are strongly typed
            // with the type that was specified for dictionary values.
            Console.WriteLine();
            foreach (string s in valueColl)
            {
                Console.WriteLine("Value = {0}", s);
            }
        }

        public static void ShowKeyCollection()
        {
            // To get the keys alone, use the Keys property.
            const string Title = "Show KeyCollection";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            openWith["rtf"] = "winword.exe";
            openWith["doc"] = "winword.exe";
            openWith.Add("ht", "hypertrm.exe");

            Dictionary<string, string>.KeyCollection keyColl =
                openWith.Keys;

            // The elements of the KeyCollection are strongly typed
            // with the type that was specified for dictionary keys.
            Console.WriteLine();
            foreach (string s in keyColl)
            {
                Console.WriteLine("Key = {0}", s);
            }
        }

        public static void ShowRemove()
        {
            // Use the Remove method to remove a key/value pair.
            const string Title = "Show Remove";
            Console.WriteLine(Constants.Constants.Header2, Title);
            CreateTestData();
            openWith["rtf"] = "winword.exe";
            openWith["doc"] = "winword.exe";
            openWith.Add("ht", "hypertrm.exe");

            Console.WriteLine("\nRemove(\"doc\")");
            openWith.Remove("doc");

            if (!openWith.ContainsKey("doc"))
            {
                Console.WriteLine("Key \"doc\" is not found.");
            }

            ShowDict(openWith);
        }

        private static void ShowDict(Dictionary<string, string> aDict)
        {
            // When you use foreach to enumerate dictionary elements,
            // the elements are retrieved as KeyValuePair objects.
            Console.WriteLine();
            foreach (KeyValuePair<string, string> kvp in aDict)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                    kvp.Key, kvp.Value);
            }
            ShowStatus(aDict);

        }

        private static void ShowStatus(Dictionary<string,string> aDict)
        {
            if (null != aDict)
            {
                Console.WriteLine("Count: {0}", aDict?.Count);
            }
            else
            {
                Console.WriteLine("Is null");
            }
        }

        private static void CreateTestData()
        {
            // Create a new dictionary of strings, with string keys.
            //
            openWith?.Clear();
            openWith = openWith ?? new Dictionary<string, string>();

            // Add some elements to the dictionary. There are no 
            // duplicate keys, but some of the values are duplicates.
            openWith.Add("txt", "notepad.exe");
            openWith.Add("bmp", "paint.exe");
            openWith.Add("dib", "paint.exe");
            openWith.Add("rtf", "wordpad.exe");
        }

        /* This code example produces the following output:

        An element with Key = "txt" already exists.
        For key = "rtf", value = wordpad.exe.
        For key = "rtf", value = winword.exe.
        Key = "tif" is not found.
        Key = "tif" is not found.
        Value added for key = "ht": hypertrm.exe

        Key = txt, Value = notepad.exe
        Key = bmp, Value = paint.exe
        Key = dib, Value = paint.exe
        Key = rtf, Value = winword.exe
        Key = doc, Value = winword.exe
        Key = ht, Value = hypertrm.exe

        Value = notepad.exe
        Value = paint.exe
        Value = paint.exe
        Value = winword.exe
        Value = winword.exe
        Value = hypertrm.exe

        Key = txt
        Key = bmp
        Key = dib
        Key = rtf
        Key = doc
        Key = ht

        Remove("doc")
        Key "doc" is not found.
        */
    }
}
