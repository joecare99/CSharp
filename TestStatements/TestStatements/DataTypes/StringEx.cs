// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="StringEx.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.IO;

namespace TestStatements.DataTypes
{
    /// <summary>
    /// Class StringEx.
    /// </summary>
    public static class StringEx
    {
        /// <summary>
        /// Alls the tests.
        /// </summary>
        public static void AllTests()
        {
            StringEx1();
            StringEx2();
            StringEx3();
            StringEx4();
            StringEx5();
            UnicodeEx1();
            StringSurogarteEx1();
        }

        /// <summary>
        /// <span data-ttu-id="ca35c-120">Durch Zuweisen eines Zeichenfolgenliterals zu einer <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a> Variable.</span>
        /// <span data-ttu-id="ca35c-121">Dies ist die am häufigsten verwendete Methode zum Erstellen einer Zeichenfolge.</span>
        /// <span data-ttu-id="ca35c-122">Im folgenden Beispiel wird die Zuweisung verwendet, um mehrere Zeichen folgen zu erstellen.</span>
        /// <span data-ttu-id="ca35c-123">Beachten
        /// Sie, C#dass in, da der umgekehrte Schrägstrich (\) ein Escapezeichen
        /// ist, literale umgekehrte Schrägstriche in einer Zeichenfolge mit
        /// Escapezeichen versehen oder die gesamte Zeichenfolge @-quoted werden
        /// muss.</span>
        /// </summary>
        public static void StringEx1()
        {
            string string1 = "This is a string created by assignment.";
            Console.WriteLine(string1);
            string string2a = "The path is C:\\PublicDocuments\\Report1.doc";
            Console.WriteLine(string2a);
            string string2b = @"The path is C:\PublicDocuments\Report1.doc";
            Console.WriteLine(string2b);
            // The example displays the following output:
            //       This is a string created by assignment.
            //       The path is C:\PublicDocuments\Report1.doc
            //       The path is C:\PublicDocuments\Report1.doc      

        }

        /// <summary>
        /// <span data-ttu-id="ca35c-124">Durch Aufrufen eines <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a>-Klassenkonstruktors.</span>
        /// <span data-ttu-id="ca35c-125">Im folgenden Beispiel werden Zeichen folgen durch Aufrufen mehrerer Klassenkonstruktoren instanziiert.</span>
        /// <span data-ttu-id="ca35c-126">Beachten Sie, dass einige der Konstruktoren Zeiger auf Zeichen Arrays oder signierte Byte Arrays als Parameter enthalten.</span>
        /// <span data-ttu-id="ca35c-127">Der Visual Basic unterstützt keine Aufrufe dieser Konstruktoren.</span>
        /// <span data-ttu-id="ca35c-128">Ausführliche Informationen zu <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a>-Konstruktoren finden Sie in der <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string.-ctor?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a>-konstruktorzusammenfassung.</span>
        /// </summary>
        public static void StringEx2()
        {
            char[] chars = { 'w', 'o', 'r', 'd' };
            sbyte[] bytes = { 0x41, 0x42, 0x43, 0x44, 0x45, 0x00 };

            // Create a string from a character array.
            string string1 = new string(chars);
            Console.WriteLine(string1);

            // Create a string that consists of a character repeated 20 times.
            string string2 = new string('c', 20);
            Console.WriteLine(string2);

#if unsafe
            string stringFromBytes = null;
            string stringFromChars = null;
            unsafe
            {
                fixed (sbyte* pbytes = bytes)
                {
                    // Create a string from a pointer to a signed byte array.
                    stringFromBytes = new string(pbytes);
                }
                fixed (char* pchars = chars)
                {
                    // Create a string from a pointer to a character array.
                    stringFromChars = new string(pchars);
                }
            }

            Console.WriteLine(stringFromBytes);
            Console.WriteLine(stringFromChars);
#endif
            // The example displays the following output:
            //       word
            //       cccccccccccccccccccc
            //       ABCDE
            //       word  

        }

        /// <summary>
        /// Strings the ex3.
        /// </summary>
        public static void StringEx3()
        {
            string string1 = "Today is " + DateTime.Now.ToString("D") + ".";
            Console.WriteLine(string1);

            string string2 = "This is one sentence. " + "This is a second. ";
            string2 += "This is a third sentence.";
            Console.WriteLine(string2);
            // The example displays output like the following:
            //    Today is Tuesday, July 06, 2011.
            //    This is one sentence. This is a second. This is a third sentence.

        }

        /// <summary>
        /// Strings the ex4.
        /// </summary>
        public static void StringEx4()
        {
            string sentence = "This sentence has five words.";
            // Extract the second word.
            int startPosition = sentence.IndexOf(" ") + 1;
            string word2 = sentence.Substring(startPosition,
                                              sentence.IndexOf(" ", startPosition) - startPosition);
            Console.WriteLine("Second word: " + word2);
            // The example displays the following output:
            //       Second word: sentence
        }

        /// <summary>
        /// Strings the ex5.
        /// </summary>
        public static void StringEx5()
        {
            DateTime dateAndTime = new DateTime(2011, 7, 6, 7, 32, 0);
            double temperature = 68.3;
            string result = String.Format("At {0:t} on {0:D}, the temperature was {1:F1} degrees Fahrenheit.",
                                          dateAndTime, temperature);
            Console.WriteLine(result);
            // The example displays the following output:
            //       At 7:32 AM on Wednesday, July 06, 2011, the temperature was 68.3 degrees Fahrenheit.
        }

        /// <summary>
        /// Unicodes the ex1.
        /// </summary>
        public static void UnicodeEx1()
        {
            StreamWriter sw = new StreamWriter(@".\graphemes.txt");
            string grapheme = "\u0061\u0308";
            sw.WriteLine(grapheme);

            string singleChar = "\u00e4";
            sw.WriteLine(singleChar);

            sw.WriteLine("{0} = {1} (Culture-sensitive): {2}", grapheme, singleChar,
                         String.Equals(grapheme, singleChar,
                                       StringComparison.CurrentCulture));
            sw.WriteLine("{0} = {1} (Ordinal): {2}", grapheme, singleChar,
                         String.Equals(grapheme, singleChar,
                                       StringComparison.Ordinal));
            sw.WriteLine("{0} = {1} (Normalized Ordinal): {2}", grapheme, singleChar,
                         String.Equals(grapheme.Normalize(),
                                       singleChar.Normalize(),
                                       StringComparison.Ordinal));
            sw.Close();
        }
        /// <summary>
        /// Strings the surogarte ex1.
        /// </summary>
        public static void StringSurogarteEx1()
        {
            string surrogate = "\uD800\uDC03";
            for (int ctr = 0; ctr < surrogate.Length; ctr++)
                Console.Write("U+{0:X2} ", Convert.ToUInt16(surrogate[ctr]));

            Console.WriteLine();
            Console.WriteLine("   Is Surrogate Pair: {0}",
                              Char.IsSurrogatePair(surrogate[0], surrogate[1]));
            // The example displays the following output:
            //       U+D800 U+DC03
            //          Is Surrogate Pair: True

        }
    }
}
