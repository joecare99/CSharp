namespace PluginBase.Interfaces;

public interface IUserInterface
{
    string Title { get; set; }
    bool ShowMessage(string message);
    void WriteLine(string v);
}
