using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using WaveFunCollapse.Models.Data;

namespace WaveFunCollapse.Models.Interfaces;

public interface IWFCModel
{
    void DisplayTest1(IRandom random);
    void DisplayTest2();
    void DisplayTest3(IRandom random);
    void ShowImage2(string filename);
    void ShowImage1(string filename);
    IReadOnlyList<TileData> AnalyseImage(string filename);

    IConsole? console { get; set; }
    Func<ConsoleColor[]>? D1SBuffer { get; set; }
    Func<ConsoleColor[]>? D2SBuffer { get; set; }
    Func<ConsoleColor[]>? D3SBuffer { get; set; }
    Action<int, int, ConsoleColor>? D1PutPixel { get; set; }
    Action<int, int, byte, byte, byte>? D1PutPixelC { get; set; }
    Action<int, int, byte, byte, byte>? D2PutPixelC { get; set; }
    Action? DUpdate { get; set; }
    Func<string, Bitmap?>? LoadImage { get; set; }
}
