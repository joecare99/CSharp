using System;
using System.Timers;
using System.Windows;

namespace Calc32WPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer tmrAnim = new Timer();
        /// <summary>The time</summary>
        public long nTime = 0;

        /// <summary>Initializes a new instance of the <see cref="T:Calc32WPF.MainWindow" /> class.</summary>
        public MainWindow()
        {
            InitializeComponent();
          
            tmrAnim.Interval = 100;
            tmrAnim.Elapsed += new ElapsedEventHandler(onAnimTimer);
        }

        private void onAnimTimer(object sender, ElapsedEventArgs e)
        {
            nTime = DateTime.Now.Ticks / 1000;
        }
          
        private void frmCalc32Main_Initialized(object sender, EventArgs e)
        {
            tmrAnim.Start();
        }

        private void frmCalc32Main_GotFocus(object sender, RoutedEventArgs e)
        {
        }

    }
}
