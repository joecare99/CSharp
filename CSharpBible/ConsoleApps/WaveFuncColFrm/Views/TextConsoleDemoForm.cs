using BaseLib.Interfaces;
using ConsoleDisplay.View;
using TestConsoleDemo.ViewModels.Interfaces;
using Views;
using WaveFunCollapse.Views;

namespace TestConsoleDemo.Views
{
    /// <summary>
    /// Class TextConsoleDemoForm.
    /// Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class TextConsoleDemoForm : Form
    {
        private IConsole _console;

        public ITextConsoleDemoViewModel DataContext { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextConsoleDemoForm"/> class.
        /// </summary>
        public TextConsoleDemoForm(ITextConsoleDemoViewModel viewModel, IConsole console, IView view)
        {
            InitializeComponent();
            DataContext = viewModel;
            this._console = console;
            viewModel.console = console;
            CommandBindingAttribute.Commit(this, viewModel);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (_console.KeyAvailable)
            {
                var ch = _console.ReadKey()?.KeyChar ?? '\0';
                if (ch == '\r') textBox1.Text = ""; else textBox1.Text += ch;
            }
        }
    }
}