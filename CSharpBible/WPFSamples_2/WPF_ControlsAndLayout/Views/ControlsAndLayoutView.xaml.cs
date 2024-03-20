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

        private void HandleSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
        }

        protected void HandleTextChanged(object sender, TextChangedEventArgs me)
        {

            try
            {
                if (RealTimeUpdate) ParseCurrentBuffer(TextBox1.Text);
                TextBox1.Foreground = Brushes.Black;
                ErrorText.Text = "";
            }

            catch (XamlParseException xpe)
            {
                TextBox1.Foreground = Brushes.Red;
                TextBox1.TextWrapping = TextWrapping.Wrap;
                ErrorText.Text = xpe.Message;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void ParseCurrentBuffer(string str)
        {

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(str);
            sw.Flush();
            ms.Flush();
            ms.Position = 0;

            var content = XamlReader.Load(ms);
            if (content != null)
            {
                cc.Children.Clear();
                cc.Children.Add((UIElement)content);
            }

        }

    }
}
