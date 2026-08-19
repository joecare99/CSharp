using System.ComponentModel;
using System.Drawing;

namespace CSFreeVision.Base
{
    public interface IView : IComponent
    {
        int Top { get; set; }
        int Left { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Point Origin { get; set; }
        Group Parent { get; set; }
        TCanvas Canvas { get; set; }
    }
}