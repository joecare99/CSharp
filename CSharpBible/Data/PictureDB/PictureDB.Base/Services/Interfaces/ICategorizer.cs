namespace PictureDB.Base.Services.Interfaces;

public interface ICategorizer
{
    string ExtractCategory(string llmResponse);
}