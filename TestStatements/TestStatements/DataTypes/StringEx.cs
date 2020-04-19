using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.DataTypes
{
    public static class StringEx
    {
        public static void AllTests()
        {
            StringEx1();
            StringEx2();
        }

        /// <summary>
        ///   <span data-ttu-id="ca35c-120">Durch Zuweisen eines Zeichenfolgenliterals zu einer <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a> Variable.</span>
        ///   <span data-ttu-id="ca35c-121">Dies ist die am häufigsten verwendete Methode zum Erstellen einer Zeichenfolge.</span>
        ///   <span data-ttu-id="ca35c-122">Im folgenden Beispiel wird die Zuweisung verwendet, um mehrere Zeichen folgen zu erstellen.</span>
        ///   <span data-ttu-id="ca35c-123">Beachten
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
        ///   <span data-ttu-id="ca35c-124">Durch Aufrufen eines <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a>-Klassenkonstruktors.</span>
        ///   <span data-ttu-id="ca35c-125">Im folgenden Beispiel werden Zeichen folgen durch Aufrufen mehrerer Klassenkonstruktoren instanziiert.</span>
        ///   <span data-ttu-id="ca35c-126">Beachten Sie, dass einige der Konstruktoren Zeiger auf Zeichen Arrays oder signierte Byte Arrays als Parameter enthalten.</span>
        ///   <span data-ttu-id="ca35c-127">Der Visual Basic unterstützt keine Aufrufe dieser Konstruktoren.</span>
        ///   <span data-ttu-id="ca35c-128">Ausführliche Informationen zu <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a>-Konstruktoren finden Sie in der <a class="xref" href="https://docs.microsoft.com/de-de/dotnet/api/system.string.-ctor?view=netframework-4.8" data-linktype="relative-path"><u><font color="#0066cc">String</font></u></a>-konstruktorzusammenfassung.</span>
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
    }
}
