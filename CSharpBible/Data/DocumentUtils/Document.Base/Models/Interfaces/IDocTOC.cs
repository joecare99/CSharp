namespace Document.Base.Models.Interfaces;

public interface IDocTOC: IDocContent
{
    void RebuildFrom(IDocSection root);
}