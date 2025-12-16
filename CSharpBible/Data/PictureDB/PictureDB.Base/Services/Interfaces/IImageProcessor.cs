namespace PictureDB.Base.Services.Interfaces;

public interface IImageProcessor
{
    string ConvertToBase64(string filePath);
}