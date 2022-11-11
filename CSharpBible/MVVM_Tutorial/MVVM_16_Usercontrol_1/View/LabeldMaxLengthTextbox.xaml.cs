using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MVVM_16_Usercontrol_1.View
{
    /// <summary>
    /// Interaktionslogik für LabeldMaxLengthTextbox.xaml
    /// </summary>
    public partial class LabeldMaxLengthTextbox : UserControl
    {
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(LabeldMaxLengthTextbox.Caption), typeof(string), typeof(LabeldMaxLengthTextbox),
                new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.None));
  
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(LabeldMaxLengthTextbox.Text), typeof(string), typeof(LabeldMaxLengthTextbox),
                new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(LabeldMaxLengthTextbox.Command), typeof(IRelayCommand), typeof(LabeldMaxLengthTextbox),
                new FrameworkPropertyMetadata(default(IRelayCommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public LabeldMaxLengthTextbox()
        {
            InitializeComponent();
        }

        public object Caption { get; set ; } = "";
        
        public string Text { 
            get => (string)GetValue(TextProperty); 
            set => SetValue(TextProperty,value); 
        }

        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public int MaxLength { get; set; } = 50;

    }
}
