using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSharpBible.Calc32.NonVisual;

namespace Calc32WPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CalculatorClass calculatorClass1 = new CalculatorClass();
        Timer tmrAnim = new Timer();
        public long nTime = 0;

        public MainWindow()
        {
            InitializeComponent();
          
            calculatorClass1.OnChange += new EventHandler(onCalcChange);
            tmrAnim.Interval = 100;
            tmrAnim.Elapsed += new ElapsedEventHandler(onAnimTimer);
        }

        private void onAnimTimer(object sender, ElapsedEventArgs e)
        {
            nTime = DateTime.Now.Ticks / 1000;
        }

        private void onCalcChange(object sender, EventArgs e)
        {
            lblMemory.Content = calculatorClass1.Memory.ToString();
            lblAkkumulator.Content = calculatorClass1.Akkumulator.ToString();
            lblOperation.Content = calculatorClass1.OperationText;
            lblTest.Content = new Binding("nTime.ToString()"); 
        }

        private void btnOne_Click(object sender, RoutedEventArgs e)
        {
            int nTag;
            if (int.TryParse((string)((Button)sender).Tag, out nTag))
            {
                calculatorClass1.Button(nTag);
            }
        }

        private void frmCalc32Main_Initialized(object sender, EventArgs e)
        {
            tmrAnim.Start();
        }

        private void frmCalc32Main_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void btnOperation_Click(object sender, RoutedEventArgs e)
        {
            int nTag;
            if (int.TryParse((string)((Button)sender).Tag, out nTag))
            {
                calculatorClass1.Operation(-nTag);
            }
        }
    }
}
