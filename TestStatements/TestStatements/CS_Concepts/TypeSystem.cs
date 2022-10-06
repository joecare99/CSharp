using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.CS_Concepts
{
    /// <summary>
    ///   <h2>Typen, Variablen und Werte</h2>
    ///   <para>C# ist eine stark typisierte Sprache. Jede Variable und jede Konstante verfügt über einen Typ, genau wie jeder Ausdruck, der zu einem Wert ausgewertet wird. Jede Methodensignatur gibt für jeden Eingabeparameter und den Rückgabewert einen Typ an. In der .NET-Klassenbibliothek werden integrierte numerische Typen und komplexe Typen definiert, die viele logische Konstrukte darstellen, z.B. das Dateisystem, Netzwerkverbindungen, Auflistungen und Arrays von Objekten sowie Datumsangaben. In einem typischen C#-Programm werden Typen aus der Klassenbibliothek sowie benutzerdefinierte Typen verwendet, die die Konzepte für das Problemfeld des Programms modellieren.</para>
    ///   <para>Folgende Informationen können in einem Typ gespeichert sein:</para>
    ///   <list type="bullet">
    ///     <item>    Der Speicherplatz, den eine Variable des Typs erfordert</item>
    ///     <item>    Die maximalen und minimalen Werte, die diese darstellen kann</item>
    ///     <item>    Die enthaltenen Member (Methoden, Felder, Ereignisse usw.)</item>
    ///     <item>    Der Basistyp, von dem geerbt wird</item>
    ///     <item>    Die Position, an der der Arbeitsspeicher für Variablen zur Laufzeit belegt wird</item>
    ///     <item>    Die Arten von zulässigen Vorgängen</item>
    ///   </list>
    /// </summary>
    public static class TypeSystem
    {

        public static void All()
        {
            UseOfTypes();
            DelareVariables();
            ValueTypes1();
            ValueTypes2();
            ValueTypes3();
            ValueTypes4();

        }

        /// <summary>
        ///   <span data-ttu-id="b84c8-116">Der Compiler verwendet Typinformationen, um sicherzustellen, dass alle im Code ausgeführten Vorgänge <em>typsicher</em> sind.</span>
        ///   <span data-ttu-id="b84c8-117">Wenn Sie z.B. eine Variable vom Typ <a href="https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/builtin-types/integral-numeric-types" data-linktype="relative-path"><u><font color="#0066cc">int</font></u></a> deklarieren, können Sie mit dem Compiler die Variable für Additions- und Subtraktionsvorgänge verwenden.</span>
        ///   <span data-ttu-id="b84c8-118">Wenn Sie dieselben Vorgänge für eine Variable vom Typ <a href="https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/builtin-types/bool" data-linktype="relative-path"><u><font color="#0066cc">bool</font></u></a> ausführen möchten, generiert der Compiler einen Fehler, wie im folgenden Beispiel dargestellt:</span>
        /// </summary>
        /// <remarks>
        /// C- und C++-Entwickler sollten beachten, dass in C# bool nicht in int konvertiert werden kann.
        /// </remarks>
        /// <example>
        ///   <code>int a = 5;             
        /// int b = a + 2; //OKbool test = true;
        ///
        /// // Error. Operator '+' cannot be applied to operands of type 'int' and 'bool'.
        /// int c = a + test;</code>
        /// </example>
        public static void UseOfTypes()
        {
            int a = 5;
            Console.WriteLine($"{a.GetType()} a = {a}");
            int b = a + 2; //OK
            Console.WriteLine($"{b.GetType()} b = {b}");

            bool test = true;
            Console.WriteLine($"{test.GetType()} test = {test}");

#if Error
            // Error. Operator '+' cannot be applied to operands of type 'int' and 'bool'.
            int c = a + test;
#endif
        }

        public static void DelareVariables()
        {
            // Declaration only:
            float temperature;
            string name;
            MyClass myClass;

            // Declaration with initializers (four examples):
            char firstLetter = 'C';
            Console.WriteLine($"{firstLetter.GetType()} firstLetter = {firstLetter}");
            var limit = 3;
            int[] source = { 0, 1, 2, 3, 4, 5 };
            var query = from item in source
                        where item <= limit
                        select item;
            Console.WriteLine($"{query.GetType()} query = {query}");
            foreach (var item in query)
            {
                Console.WriteLine($"{item.GetType()} item = {item}");
            }

            foreach (var item in Enumerable.Range(1, 10).Select(x => x * x )) 
            {
                Console.WriteLine($"{item.GetType()} item = {item}");
            }
        }

        public static string GetName(int ID)
        {
            if (ID < names.Length)
                return names[ID];
            else
                return String.Empty;
        }
        private static string[] names = { "Spencer", "Sally", "Doug" };

        public static void ValueTypes1()
        {
            // Static method on type byte.
            byte b = byte.MaxValue;
            Console.WriteLine($"{b.GetType()} b = {b}");
        }

        public static void ValueTypes2()
        {
            byte num = 0xA;
            Console.WriteLine($"{num.GetType()} num = {num}");

            int i = 5;
            Console.WriteLine($"{i.GetType()} i = {i}");

            char c = 'Z';
            Console.WriteLine($"{c.GetType()} c = {c}");
        }

        public struct Coords
        {
            public int x, y;

            public Coords(int p1, int p2)
            {
                x = p1;
                y = p2;
            }
            public override string ToString() => $"{"{"}{x}, {y}{"}"}";
            
        }

        public static void ValueTypes3()
        {
            Coords c = new Coords() { x = 2, y = 3 };
            Console.WriteLine($"{c.GetType()} c = {c}");
        }

        public enum FileMode
        {
            CreateNew = 1,
            Min = CreateNew,
            Create = 2,
            Open = 3,
            OpenOrCreate = 4,
            Truncate = 5,
            Append = 6,
            Max = Append
        }
        static IEnumerable<FileMode> FileModes()
        {
            for (var i = FileMode.Min; i <= FileMode.Max; i++)
                yield return i;
        }

        public static void ValueTypes4()
        {
            foreach (var item in FileModes())
            {
                Console.WriteLine($"{item.GetType()} item = {item}");
            }
        }
        private class MyClass
        {
        }

    }
}
