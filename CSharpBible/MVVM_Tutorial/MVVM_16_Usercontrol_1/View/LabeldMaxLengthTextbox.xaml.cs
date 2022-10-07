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
        public LabeldMaxLengthTextbox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Caption { get; set; } = "";
        public string Text { get; set; } = "";
        public int MaxLength { get; set; } = 50;
    }
}
