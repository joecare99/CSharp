using System.IO;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using WPF_ControlsAndLayout.ViewModels;

namespace WPF_ControlsAndLayout.Views
{
    /// <summary>
    /// Interaktionslogik für ControlsAndLayoutView.xaml
    /// </summary>
    public partial class ControlsAndLayoutView
    {
        public ControlsAndLayoutView()
        {
            InitializeComponent();

            if (DataContext is ControlsAndLayoutViewModel vm)
            {
                vm.ParseCurrentBuffer = ParseCurrentBuffer;
            }
        }

        public bool RealTimeUpdate = true;

        private void ParseCurrentBuffer(string str)
        {

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(str);
            sw.Flush();
            ms.Flush();
            ms.Position = 0;

            var content = XamlReader.Load(ms);
            if (content is UIElement uie)
            {
                cc.Children.Clear();
                cc.Children.Add(uie);
            }

        }

    }
}
