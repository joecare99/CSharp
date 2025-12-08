namespace PictureDB.Base.Services.Interfaces;

public interface IEvaluator
{
    int ExtractScore(string llmResponse);
}