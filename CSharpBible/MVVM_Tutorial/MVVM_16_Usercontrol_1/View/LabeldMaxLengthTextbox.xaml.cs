using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVVM_16_Usercontrol_1.View
{
    /// <summary>
    /// Interaktionslogik für LabeldMaxLengthTextbox.xaml
    /// </summary>
    public partial class LabeldMaxLengthTextbox : UserControl
    {
        public static readonly DependencyProperty _CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(object), typeof(LabeldMaxLengthTextbox),
                new FrameworkPropertyMetadata((object)null, (PropertyChangedCallback)OnCaptionChanged));
  
        public static readonly DependencyProperty _TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(object), typeof(LabeldMaxLengthTextbox),
                new FrameworkPropertyMetadata((object)null, (PropertyChangedCallback)OnTextChanged));
        private string _text;

        private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((LabeldMaxLengthTextbox)d).OnCaptionChanged(e.OldValue, e.NewValue); // Übergang in die Instanz

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((LabeldMaxLengthTextbox)d).OnTextChanged(e.OldValue, e.NewValue); // Übergang in die Instanz

        private void OnCaptionChanged(object old, object newValue)
        {
            if (newValue is string @s)
                lblCaption.Content = $"{@s}";
        }

        private void OnTextChanged(object old, object newValue)
        {
            if (newValue is string @s && @s != _Text)
                _Text=@s;
        }

        public LabeldMaxLengthTextbox()
        {
            InitializeComponent();
        }

        public object Caption { get; set; } = "";
        public object Text { 
            get; 
            set; }
        public string _Text
        {
            get => _text;
            set { _text = value;
                if (Text is Binding b)
                    ;
                else SetValue(_TextProperty, value);
            }
        }

        public int MaxLength { get; set; } = 50;

    }
}
