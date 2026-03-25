namespace Trnsp.Show.Pas.Services
{
    public interface IFileService
    {
        string? OpenFileDialog(string title, string filter);
        string ReadAllText(string path);
    }
}
