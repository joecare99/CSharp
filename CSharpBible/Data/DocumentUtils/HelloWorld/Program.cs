using Document.Base.Models.Interfaces;
using Document.Odf.Models;

namespace HelloWorld;

public class Program
{
    const string cOutputFile = "HelloWorld.odt";

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

        userDocument = OdfTextDocument.CreateUserDocument();
    }

    static void Run()
    {
        userDocument.AddParagraph(cStylename).TextContent = "Hello World!";
        userDocument.SaveTo(cOutputPath);
        Console.WriteLine("Document saved to: " + cOutputPath);
    }

    static void Main(string[] args)
    {
        Init(args);
        Run();
    }
}
