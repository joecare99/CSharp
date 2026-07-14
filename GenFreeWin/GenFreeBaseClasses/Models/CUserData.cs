using GenFree.Interfaces.Model;

namespace GenFree.Models;

public class CUserData : IUserData
{
    public string Owner { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string Place { get; set; }
    public string Staat { get; set; }
    public string mail { get; set; }
    public string tel { get; set; }
}