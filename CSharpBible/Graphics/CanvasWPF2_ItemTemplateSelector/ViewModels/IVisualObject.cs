using MVVM.ViewModel;
using System.Windows;

namespace CanvasWPF2_ItemTemplateSelector.ViewModel
{
    public interface IVisualObject
    {
        double X { get; set; }
        double Y { get; set; }
        double Dx { get; set; }
        double Dy { get; set; }
        int sType { get; set; }
        DelegateCommand<object> MouseHover { get; set; }
        Point point { get; set; }

        void Step();
    }
}