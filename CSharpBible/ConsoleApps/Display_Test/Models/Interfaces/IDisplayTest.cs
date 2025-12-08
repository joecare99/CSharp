using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;

namespace DisplayTest.Models.Interfaces;

public interface IDisplayTest
{
    void DisplayTest1(IRandom random);
    void DisplayTest2();
    void DisplayTest3(IRandom random);
    IConsole? console { get; set; }
}
