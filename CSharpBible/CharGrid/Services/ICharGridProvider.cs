namespace CSharpBible.CharGrid.Services;

public interface ICharGridProvider
{
    int Rows { get; }
    int Columns { get; }
    char GetChar(int row, int column);
    void SetChar(int row, int column, char value);
}
