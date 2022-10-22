using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorials.HelloWorld
{
    /// <summary>Ausführen des ersten C#-Programms</summary>
    public static class HelloWorldClass
    {
        /// <summary>
        /// Führen Sie den folgenden Code im interaktiven Fenster aus. Wählen Sie die Schaltfläche Enter focus mode (Fokusmodus aktivieren) aus. Geben Sie anschließend den folgenden Codeblock in das interaktive Fenster ein, und wählen Sie Ausführen aus:
        /// </summary>
        /// <remarks>
        ///   <para>Herzlichen Glückwunsch! Sie haben Ihr erstes C#-Programm ausgeführt. Hierbei handelt es sich um ein einfaches Programm, das die Meldung „Hello World!“ ausgibt. Diese Meldung wird anhand der Console.WriteLine-Methode ausgegeben. Der Console-Typ stellt das Konsolenfenster dar. WriteLine ist eine Methode des Console-Typs, die eine Textzeile in dieser Textkonsole ausgibt.</para>
        ///   <para>Fahren wir fort, und sehen wir uns das einmal genauer an. In der restlichen Lektion wird die Arbeit mit dem string-Typ erklärt, der Text in C# darstellt. Wie der Console-Typ weist der string-Typ bestimmte Methoden auf. Bei den string-Methoden geht es um Text.</para>
        /// </remarks>
        public static void HelloWorld()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
