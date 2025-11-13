using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class Categorizer : ICategorizer
{
    public string ExtractCategory(string llmResponse)
    {
        // Einfacher Platzhalter; hier kann später JSON/XML Parsing ergänzt werden
        return "Uncategorized";
    }
}