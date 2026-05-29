using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RenderDemo.Pages
{
    public partial class ClippingPage : UserControl
    {
        public ClippingPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
