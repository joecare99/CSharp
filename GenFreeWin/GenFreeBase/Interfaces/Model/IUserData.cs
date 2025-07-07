namespace GenFree.Interfaces.Model;

public interface IUserData
{
    string Name { get; set; }
    string Staat { get; set; }
    string Place { get; set; }
    string Street { get; set; }
    string mail { get; set; }
    string tel { get; set; }
    string Owner { get; set; }
}