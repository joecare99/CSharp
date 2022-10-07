using ConsoleDisplay.View;
using TestConsole;

namespace TestConsoleDemo.View
{
    /// <summary>
    /// Class TextConsoleDemoForm.
    /// Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class TextConsoleDemoForm : Form
    {
        TestConsole.TstConsole console = new TestConsole.TstConsole();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextConsoleDemoForm"/> class.
        /// </summary>
        public TextConsoleDemoForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (console.KeyAvailable)
            {
                var ch = console.ReadKey()?.KeyChar ?? '\0';
                if (ch == '\r') textBox1.Text = ""; else textBox1.Text += ch;
            }
        }

        private void btnHello_Click(object sender, EventArgs e)
        {
            console.Clear();
            console.ForegroundColor= ConsoleColor.White;
            console.SetCursorPosition(20, 12);
            console.WriteLine("Hello World");
        }

        private void btnLongText_Click(object sender, EventArgs e)
        {
            console.Clear();
            var rnd =new Random();
            var newPara = true;
            for(int i = 0; i < 3000; i++)
            {
                var word = "";
                for (int j = 0; j <rnd.Next(3,14); j++)
                {
                    word +=  (j == 0) && (rnd.Next(5) == 0||newPara) ? (char)rnd.Next(65, 91) : (char)rnd.Next(97, 123);
                }
                newPara = false;
                if (console.GetCursorPosition().Left + word.Length+2 > console.WindowWidth)
                    console.WriteLine();
                console.Write(word);
                switch (rnd.Next(8))
                {
                    case 0: console.Write(". "); newPara = true; break;
                    case 1: console.Write(", "); break;
                    default:
                        console.Write(" "); break;
                }
                Thread.Sleep(0);
            }
        }

        private void btnColText_Click(object sender, EventArgs e)
        {
            console.Clear();
            var rnd = new Random();
            var newPara = true;
            for (int i = 0; i < 3000; i++)
            {
                var word = "";
                for (int j = 0; j < rnd.Next(3, 14); j++)
                {
                    word += (j == 0) && (rnd.Next(5) == 0 || newPara) ? (char)rnd.Next(65, 91) : (char)rnd.Next(97, 123);
                }
                newPara = false;
                if (console.GetCursorPosition().Left + word.Length + 2 > console.WindowWidth)
                    console.WriteLine();
                console.ForegroundColor = (ConsoleColor)rnd.Next(8, 16);
                console.BackgroundColor = (ConsoleColor)rnd.Next(0, 8);
                console.Write(word);
                switch (rnd.Next(8))
                {
                    case 0: console.Write(". "); newPara = true; break;
                    case 1: console.Write(", "); break;
                    default:
                        console.Write(" "); break;
                }
                Thread.Sleep(0);
            }

        }

        private void btnDisplayTest_Click(object sender, EventArgs e)
        {
            Display.myConsole = console;
            Display_Test.Program.Main(new string[] { });
        }
    }
}