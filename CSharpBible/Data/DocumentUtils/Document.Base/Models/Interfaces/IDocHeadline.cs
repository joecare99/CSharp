namespace Document.Base.Models.Interfaces;

public interface IDocHeadline: IDocContent
{
    int Level { get; }
}