using Document.Base.Models.Interfaces;
using Document.Odf;
using System.Windows;

namespace OdtBoldTextExample;

public class Program
{
    const string cOutputFile = "BoldText.fodt";

    static string cOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), cOutputFile);

    const string cStylename = "Standard";

    static IUserDocument userDocument;

    static void Init(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine("Output file path set to: " + args[0]);
            cOutputPath = args[0];
        }

        userDocument = new OdfTextDocument();
    }

    static void Run()
    {
        var p = userDocument.AddParagraph(cStylename);
        p.TextContent = "A";
        p.AddSpan("bold", [FontWeights.Bold]);
        p.AddSpan(" text", []);
        userDocument.SaveTo(cOutputPath);
        Console.WriteLine("Document saved to: " + cOutputPath);
    }

    static void Main(string[] args)
    {
        Init(args);
        Run();
    }
}
